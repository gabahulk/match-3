using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Scripts.Configs;
using Code.Scripts.Match3;
using Code.Scripts.SOArchitecture;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Match3Board : MonoBehaviour, IMatch3Board
{
    [SerializeField] private TileType[] tileTypes;
    [SerializeField] private MatchType[] matchTypes;
    [SerializeField] private int boardSize;
    [SerializeField] private float margin;
    [SerializeField] private float spacing;
    [SerializeField] private GameObjectVariable currentTileObject;
    [SerializeField] private Match3BoardAnimationsConfig animationsConfig;
    [SerializeField] private BoolVariable isBoardLocked;

    public Match3Tile tilePrefab;
    private Match3Tile[,] _tileGrid;
    private Vector2 _tileSize;
    private Vector2 _tileSpacing;
    private Vector2 _totalSpacing; 
    private Vector2 _initialPosition;


    private void Start()
    {
        _tileSize = tilePrefab.GetComponent<RectTransform>().rect.size;
        _tileSpacing = new Vector2(spacing, spacing);
        _totalSpacing = _tileSpacing * boardSize; 
        GetComponent<RectTransform>().sizeDelta = _tileSize * boardSize + new Vector2(margin, margin) + _totalSpacing;
        _initialPosition = _tileSize * new Vector2(-1, 1) * boardSize/2 + _tileSize/2 * new Vector2(1, -1) + (_tileSpacing * new Vector2(-1,1));

        InitializeBoard();
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
        tile.Setup(position, tileTypes[Random.Range(0, tileTypes.Length)]);
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
            var matchType = cluster.FilterClusterToMatchType(matchTypes);
            print(matchType);
            foreach (var item in cluster.GetTiles())
            {
                tasks.Add(PopTile(item.Key, item.Value));
            }
        }
        await Task.WhenAll(tasks);
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

        return sequence.AsyncWaitForCompletion();

        void Callback()
        {
            Destroy(tile.gameObject);
            _tileGrid[pos.x, pos.y] = null;
        }
    }
    
    private async void RefillBoard()
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
        connectedTiles.AddTile(tile.GetTilePositionInGrid(), tile);
        
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
        var currentTile = currentTileObject.value.GetComponent<Match3Tile>();
        var positionA = currentTile.GetTilePositionInGrid();
        var positionB = positionA + direction;
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
        print("first clusters");
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
            while (tileClusters.Count > 0)
            {
                await PopTiles(tileClusters);
                RefillBoard();
                print("waiting refill");
                await Task.Delay(1000);
                print("rechecking clusters");
                tileClusters = CheckMatches();
                if (tileClusters.Count > 0)
                {
                    print("more clusters");
                }
            }
        }
        
    }
}

public interface IMatch3Board
{
    void InitializeBoard();
    List<TileCluster> CheckMatches();

    void MoveSelectedTile(Vector2Int direction);
}
