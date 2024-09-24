using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Scripts.Configs;
using Code.Scripts.Game.Player;
using Code.Scripts.SOArchitecture;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Scripts.Match3
{
    public class Match3Board : MonoBehaviour, IMatch3Board
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private PlayerTileInventory playerTileInventory;
        [SerializeField] private int boardSize;
        [SerializeField] private float margin;
        [SerializeField] private float spacing;
        [SerializeField] private GameObjectVariable currentTileObject;
        [SerializeField] private Match3BoardAnimationsConfig animationsConfig;
        [SerializeField] private BoolVariable isBoardLocked;
        [SerializeField] private RectTransform container;
        [SerializeField] private GameEvent tilePopEvent;

        public Match3Tile tilePrefab;
        private Match3Tile[,] _tileGrid;
        private Vector2 _tileSize;
        private Vector2 _tileSpacing;
        private Vector2 _initialPosition;
        private Vector2 _containerSize;

        private async void Start()
        {
            _containerSize = new Vector2(container.rect.width, container.rect.height);
            var tileSize = (Math.Min(_containerSize.x, _containerSize.y) - (margin * 4) ) / boardSize;
            _tileSize = new Vector2(tileSize, tileSize);
            tilePrefab.GetComponent<RectTransform>().sizeDelta  = _tileSize;
            _tileSpacing = new Vector2(spacing, spacing);
            _initialPosition = _tileSize * new Vector2(-1, 1) * boardSize/2 + _tileSize/2 * new Vector2(1, -1) + (_tileSpacing * new Vector2(-1,1));
            isBoardLocked.Value = false;
            InitializeBoard();
            var tileClusters = CheckMatches();
            await HandlePopBehavior(tileClusters);
        }

        public void InitializeBoard()
        {
            _tileGrid = new Match3Tile[boardSize, boardSize];
            var posVector = new Vector2Int(0, 0);
            for (var i = 0; i < _tileGrid.GetLength(0); i++)
            {
                for (var j = 0; j < _tileGrid.GetLength(1); j++)
                {
                    posVector.x = i;
                    posVector.y = j;
                    var pos = posVector;
                    _tileGrid[pos.x,pos.y] = CreateTile(pos);
                    PositionTile(_tileGrid[pos.x,pos.y], posVector);
                } 
            }
        }

        private Match3Tile CreateTile(Vector2Int position)
        {
            var tile = Instantiate(tilePrefab, transform, false);
            tile.Setup(position, gameConfig.TileTypes[Random.Range(0, gameConfig.TileTypes.Length)]);
            return tile;
        }

        private Task PositionTile(Match3Tile tile, Vector2Int position)
        {
            var indexPosition = new Vector3(position.x, -position.y, 0);
            tile.SetTilePositionInGrid(position);
            tile.name = "Tile (" + position.x + "," + position.y + ")";
            return tile.transform.DOLocalMove(_initialPosition + indexPosition * _tileSize +
                                              _tileSpacing / 2 * indexPosition,animationsConfig.TileFallDuration).AsyncWaitForCompletion();
        
        }
    
        private Task PositionTile(Match3Tile tile, Vector2Int finalPosition, Vector2Int initialPosition)
        {
            var indexPosition = GetIndexPosition(initialPosition);
            tile.transform.localPosition = _initialPosition + indexPosition * _tileSize +
                                           _tileSpacing / 2 * indexPosition;
                                       
            indexPosition = GetIndexPosition(finalPosition);
            tile.SetTilePositionInGrid(finalPosition);
            tile.name = "Tile (" + finalPosition.x + "," + finalPosition.y + ")";
        
            return tile.transform.DOLocalMove(_initialPosition + indexPosition * _tileSize +
                                              _tileSpacing / 2 * indexPosition, animationsConfig.TileFallDuration).AsyncWaitForCompletion();
        
        }

        private Vector3 GetIndexPosition(Vector2Int position)
        {
            return new Vector3(position.x, -position.y, 0);
        }


        public List<TileCluster> CheckMatches()
        {
            var excludedTiles = new Dictionary<Match3Tile, bool>();
            var tileClusters = new List<TileCluster>();
            var posVector = new Vector2Int(0, 0);
            for (var i = 0; i < _tileGrid.GetLength(0); i++)
            {
                for (var j = 0; j < _tileGrid.GetLength(1); j++)
                {
                    posVector.x = i;
                    posVector.y = j;
                    var tileType = _tileGrid[i, j].GetTileType();
                    var tileCluster = GetTileCluster(posVector, tileType, excludedTiles);
                    if (!tileCluster.CanPop()) continue;
                    tileClusters.Add(tileCluster);
                    foreach (var item in tileCluster.GetTiles())
                    {
                        excludedTiles[item.Value] = true;
                    }

                } 
            }

            return tileClusters;
        }

        private async Task PopTiles(List<TileCluster> connections)
        {
            var tasks = new List<Task>();
            foreach (var cluster in connections)
            {
                var matchType = cluster.FilterClusterToMatchType(gameConfig.MatchTypes);
                //TODO: Add hook for creating special tiles based on the match here

                ScoreCluster(cluster);
                foreach (var item in cluster.GetTiles())
                {
                    tasks.Add(PopTile(item.Key, item.Value));
                }
            }
            await Task.WhenAll(tasks);
        }

        private void ScoreCluster(TileCluster cluster)
        {
            var score = cluster.GetClusterScore();
            playerTileInventory[score.Item1] = score.Item2;
        }

        private Task PopTile(Vector2Int pos, Match3Tile tile)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(tile.transform.DOShakePosition(
                animationsConfig.ShakeDurations, 
                animationsConfig.ShakeStrength,
                animationsConfig.ShakeVibrato,
                animationsConfig.ShakeRandomness,
                animationsConfig.ShakeSnapping,
                animationsConfig.ShakeFadeOut,
                animationsConfig.ShakeRandomnessMode));
            sequence.Append(tile.transform.DOScale(animationsConfig.ScaleTargetValue, animationsConfig.ScaleDuration));
            sequence.Join(tile.GetComponent<Image>().DOColor(animationsConfig.FadeColor,animationsConfig.FadeDuration));
            sequence.AppendCallback(Callback);
            tilePopEvent.Raise();

            return sequence.AsyncWaitForCompletion();

            void Callback()
            {
                Destroy(tile.gameObject);
                _tileGrid[pos.x, pos.y] = null;
            }
        }
    
        private async Task RefillBoard()
        {
            var tasks = new List<Task>();

            for (int i = 0; i < _tileGrid.GetLength(0); i++)
            {
                var numberOfEmptySlots = 0;
                for (int j = _tileGrid.GetLength(1) - 1; j >= 0; j--)
                {
                    if (_tileGrid[i,j] == null)
                    {
                        numberOfEmptySlots++;
                    }
                    else if(numberOfEmptySlots > 0)
                    {
                        _tileGrid[i, j + numberOfEmptySlots] = _tileGrid[i, j];
                        _tileGrid[i, j] = null;
                    }
                }
                if (numberOfEmptySlots > 0)
                    tasks.AddRange(CreateNewTiles(numberOfEmptySlots, i));
            }
        
            var posVector = new Vector2Int(0, 0);
            for (int i = 0; i < _tileGrid.GetLength(0); i++)
            {
                for (var j = 0; j < _tileGrid.GetLength(1); j++)
                {
                    if (_tileGrid[i, j] == null) continue;
                    posVector.x = i;
                    posVector.y = j;
                    tasks.Add(PositionTile(_tileGrid[i, j], posVector));
                } 
            }
            await Task.WhenAll(tasks);
        }

        private List<Task> CreateNewTiles(int numberOfEmptySlots, int columnIndex)
        {
            var tasks = new List<Task>();
            var pos = new Vector2Int(columnIndex, 0);
            var originalPos = new Vector2Int(columnIndex, 0);
            for (var i = numberOfEmptySlots; i > 0; i--)
            {
                pos.y = i - 1;
                originalPos.y = pos.y - numberOfEmptySlots;
                _tileGrid[pos.x, pos.y] = CreateTile(pos);
                tasks.Add(PositionTile(_tileGrid[pos.x,pos.y], pos, originalPos));
            }
            return tasks;
        }

        private TileCluster GetTileCluster(Vector2Int position, TileType typeToMatch, Dictionary<Match3Tile, bool> excludedTiles)
        {
            var connectedTiles = new TileCluster();
            if (IsOutOfBounds(position))
            {
                return connectedTiles;
            }
            var tile = _tileGrid[position.x, position.y];

            if (!IsSameTileType(typeToMatch, position))
            {
                return connectedTiles;
            }

            excludedTiles ??= new Dictionary<Match3Tile, bool>();
            if (excludedTiles.ContainsKey(tile))
            {
                return connectedTiles;
            }
            excludedTiles.Add(tile, true);
            connectedTiles.AddTile(position, tile);
        
            var directions = new []
            {
                Vector2Int.down, Vector2Int.up, Vector2Int.left, Vector2Int.right
            }; 
        
            foreach (var direction in directions)
            {
                var newPosition = position + direction;
                foreach (var item in GetTileCluster(newPosition, typeToMatch, excludedTiles).GetTiles())
                {
                    connectedTiles.SetTile(item.Key, item.Value);
                }
            }

            return connectedTiles;
        }

        private bool IsOutOfBounds(Vector2Int position)
        {
            return position.x < 0 || position.y < 0 || position.x >= boardSize || position.y >= boardSize;
        }
    
        private bool IsSameTileType(TileType tileType, Vector2Int position)
        {
            return _tileGrid[position.x, position.y].GetTileType() == tileType;
        }
    

        public async void MoveSelectedTile(Vector2Int direction)
        {
            isBoardLocked.Value = true;
            var currentTile = currentTileObject.Value.GetComponent<Match3Tile>();
            var positionA = currentTile.GetTilePositionInGrid();
            var positionB = positionA + direction;
            if (IsOutOfBounds(positionB))
            {
                isBoardLocked.Value = false;
                return;
            }
            var auxTile = _tileGrid[positionB.x, positionB.y];
            _tileGrid[positionB.x, positionB.y] = currentTile;
            _tileGrid[positionA.x, positionA.y] = auxTile;
            var tasks = new List<Task>
            {
                PositionTile(currentTile, positionB),
                PositionTile(auxTile, positionA)
            };
            await Task.WhenAll(tasks.ToArray());
            tasks.Clear();
            var tileClusters = CheckMatches();
            if (tileClusters.Count <= 0)
            {
                _tileGrid[positionB.x, positionB.y] = auxTile;
                _tileGrid[positionA.x, positionA.y] = currentTile;
                tasks.Add(PositionTile(currentTile, positionA));;
                tasks.Add(PositionTile(auxTile, positionB));
                await Task.WhenAll(tasks.ToArray());
            }
            else
            {
                await HandlePopBehavior(tileClusters);
            }

            var suggestedMatch = CheckIfBoardHasPossibleMatches();
            while (suggestedMatch == null)
            {
                await ShuffleBoard();
                tileClusters = CheckMatches();
                await HandlePopBehavior(tileClusters);
                suggestedMatch = CheckIfBoardHasPossibleMatches();
            }
            isBoardLocked.Value = false;
        }

        private async Task HandlePopBehavior(List<TileCluster> tileClusters)
        {
            while (tileClusters.Count > 0)
            {
                await PopTiles(tileClusters);
                await RefillBoard();
                tileClusters = CheckMatches();
            }
        }

        public TileCluster CheckIfBoardHasPossibleMatches()
        {
            var directions = new []
            {
                Vector2Int.down, Vector2Int.up, Vector2Int.left, Vector2Int.right
            };
        
            var auxPosition = new Vector2Int(-1, -1);
            var donePositions = new HashSet<Vector2Int>();
            for (var i = 0; i < _tileGrid.GetLength(0); i++)
            {
                for (var j = 0; j < _tileGrid.GetLength(1); j++)
                {
                    foreach (var direction in directions)
                    {
                        auxPosition.x = i + direction.x;
                        auxPosition.y = j + direction.y;
                        if (IsOutOfBounds(auxPosition) || donePositions.Contains(auxPosition))
                        {
                            continue;
                        }

                        (_tileGrid[auxPosition.x, auxPosition.y], _tileGrid[i, j]) = (_tileGrid[i, j], _tileGrid[auxPosition.x, auxPosition.y]);
                        var matches = CheckMatches();
                        (_tileGrid[auxPosition.x, auxPosition.y], _tileGrid[i, j]) = (_tileGrid[i, j], _tileGrid[auxPosition.x, auxPosition.y]);
                        donePositions.Add(new Vector2Int(i, j));
                        if (matches.Count > 0)
                        {
                            return matches[0];
                        }
                    } 
                }
            }

            return null;
        }

        private Task ShuffleBoard()
        {
            ShuffleMatrix();
            List<Task> tasks = new List<Task>();
            var rows = _tileGrid.GetLength(0);
            var cols = _tileGrid.GetLength(1);
            var pos = Vector2Int.zero;
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    pos.x = i;
                    pos.y = j;
                    tasks.Add(PositionTile(_tileGrid[i, j],pos));
                }
            }
            return Task.WhenAll(tasks.ToArray());
        }
    
        private void ShuffleMatrix()
        {
            var rows = _tileGrid.GetLength(0);
            var cols = _tileGrid.GetLength(1);

            var flatMatrix = new Match3Tile[rows * cols];
            var index = 0;

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    flatMatrix[index++] = _tileGrid[i, j];
                }
            }

            for (int i = flatMatrix.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (flatMatrix[i], flatMatrix[j]) = (flatMatrix[j], flatMatrix[i]);
            }

            index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    _tileGrid[i, j] = flatMatrix[index++];
                }
            }
        }
    }
}