using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Scripts.SOArchitecture;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Match3Board : MonoBehaviour, IMatch3Board
{
    [SerializeField] private TileType[] _tileTypes;
    [SerializeField] private int boardSize;
    [SerializeField] private float margin;
    [SerializeField] private float spacing;
    [SerializeField] private GameObjectVariable currentTileObject;
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


    public void ClearBoard()
    {
        throw new System.NotImplementedException();
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
        tile.Setup(position, _tileTypes[Random.Range(0, _tileTypes.Length)]);
        return tile;
    }

    private Task PositionTile(Match3Tile tile, Vector2Int position)
    {
        var indexPosition = new Vector3(position.x, -position.y, 0);
        tile.SetTilePositionInGrid(position);
        tile.name = "Tile (" + position.x + "," + position.y + ")";
        return tile.transform.DOLocalMove(_initialPosition + indexPosition * _tileSize +
                                   _tileSpacing / 2 * indexPosition,.5f).AsyncWaitForCompletion();
        
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
                                         _tileSpacing / 2 * indexPosition,.5f).AsyncWaitForCompletion();
        
    }

    private Vector3 GetIndexPosition(Vector2Int position)
    {
        return new Vector3(position.x, -position.y, 0);
    }


    public List<Dictionary<Vector2Int,Match3Tile>> CheckMatches()
    {
        var excludedTiles = new Dictionary<Match3Tile, bool>();
        var connections = new List<Dictionary<Vector2Int,Match3Tile>>();
        var posVector = new Vector2Int(0, 0);
        for (var i = 0; i < _tileGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _tileGrid.GetLength(1); j++)
            {
                posVector.x = i;
                posVector.y = j;
                var tileType = _tileGrid[i, j].GetTileType();
                var connectedTiles = GetConnectedTiles(posVector, tileType, excludedTiles);
                if (!CanPop(connectedTiles)) continue;
                connections.Add(connectedTiles);
                foreach (var item in connectedTiles)
                {
                    excludedTiles[item.Value] = true;
                }

            } 
        }

        return connections;
    }

    private void PopTiles(List<Dictionary<Vector2Int, Match3Tile>> connections)
    {
        foreach (var connection in connections)
        {
            var filteredConnection = FilterConnection(connection);
            foreach (var item in filteredConnection)
            {
                Destroy(item.Value.gameObject);
                _tileGrid[item.Key.x, item.Key.y] = null;
            }
            
        }
    }

    private Dictionary<Vector2Int, Match3Tile> FilterConnection(Dictionary<Vector2Int, Match3Tile> connection)
    {
        return connection;
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
            originalPos.y = -i;
            _tileGrid[pos.x, pos.y] = CreateTile(pos);
            tasks.Add(PositionTile(_tileGrid[pos.x,pos.y], pos, originalPos));
        }
        return tasks;
    }

    private bool CanPop(Dictionary<Vector2Int, Match3Tile> connections)
    {
        foreach (var item in connections)
        {
            var tilePos = item.Key;
            var hasVerticalNeighbours = connections.ContainsKey(tilePos + Vector2Int.down) &&
                                        connections.ContainsKey(tilePos + Vector2Int.up);
            var hasHorizontalNeighbours = connections.ContainsKey(tilePos + Vector2Int.left) &&
                                          connections.ContainsKey(tilePos + Vector2Int.right);
            if (hasVerticalNeighbours || hasHorizontalNeighbours)
            {
                return true;
            }
        }
        return false;
    }
    
    public Dictionary<Vector2Int, Match3Tile> GetConnectedTiles(Vector2Int position, TileType typeToMatch, Dictionary<Match3Tile, bool> excludedTiles)
    {
        var connectedTiles = new Dictionary<Vector2Int, Match3Tile>();
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
        connectedTiles.Add(tile.GetTilePositionInGrid(), tile);
        
        var directions = new []
        {
            Vector2Int.down, Vector2Int.up, Vector2Int.left, Vector2Int.right
        }; 
        
        foreach (var direction in directions)
        {
            var newPosition = position + direction;
            foreach (var item in GetConnectedTiles(newPosition, typeToMatch, excludedTiles))
            {
                connectedTiles[item.Key] = item.Value;
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
        var connections = CheckMatches();
        
        if (connections.Count <= 0)
        {
            _tileGrid[positionB.x, positionB.y] = auxTile;
            _tileGrid[positionA.x, positionA.y] = currentTile;
            tasks.Add(PositionTile(currentTile, positionA));;
            tasks.Add(PositionTile(auxTile, positionB));
            await Task.WhenAll(tasks.ToArray());
        }
        else
        {
            while (connections.Count > 0)
            {
                PopTiles(connections);
                RefillBoard();
                connections = CheckMatches();
            }
        }
        
    }
}

public interface IMatch3Board
{
    void ClearBoard();
    void InitializeBoard();
    List<Dictionary<Vector2Int,Match3Tile>> CheckMatches();

    void MoveSelectedTile(Vector2Int direction);
}
