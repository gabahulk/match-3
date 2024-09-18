using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.SOArchitecture;
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
        for (var i = 0; i < _tileGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _tileGrid.GetLength(1); j++)
            {
                var tile = Instantiate(tilePrefab, transform, false);
                _tileGrid[j,i] = tile;
                var pos = new Vector2Int(j,i);
                tile.Setup(pos, _tileTypes[Random.Range(0, _tileTypes.Length)]);
                PositionTile(tile, pos);
                tile.name = "Tile (" + j + "," + i + ")";
            } 
        }
    }

    private void PositionTile(Match3Tile tile, Vector2Int position)
    {
        var indexPosition = new Vector3(position.x, -position.y, 0);
        tile.transform.localPosition = _initialPosition + indexPosition * _tileSize +
                                       _tileSpacing/2 * indexPosition;
        tile.SetTilePositionInGrid(position);
    }

    public void CheckMatches()
    {
        throw new System.NotImplementedException();
    }

    public void MoveSelectedTile(Vector2Int direction)
    {
        var currentTile = currentTileObject.value.GetComponent<Match3Tile>();
        var positionA = currentTile.GetTilePositionInGrid();
        var positionB = positionA + direction;
        var auxTile = _tileGrid[positionB.x, positionB.y];
        _tileGrid[positionB.x, positionB.y] = currentTile;
        _tileGrid[positionA.x, positionA.y] = auxTile;
        PositionTile(currentTile, positionB);
        PositionTile(auxTile, positionA);
    }
}

public interface IMatch3Board
{
    void ClearBoard();
    void InitializeBoard();
    void CheckMatches();

    void MoveSelectedTile(Vector2Int direction);
}
