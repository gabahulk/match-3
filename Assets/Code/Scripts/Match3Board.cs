using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3Board : MonoBehaviour, IMatch3Board
{
    public int boardSize;
    public float margin;
    public float spacing;
    public Match3Tile tilePrefab;
    private Match3Tile[,] _tileGrid;

    private void Start()
    {
        InitializeBoard();
    }


    public void ClearBoard()
    {
        throw new System.NotImplementedException();
    }

    public void InitializeBoard()
    {
        Vector2 tileSize = tilePrefab.GetComponent<RectTransform>().rect.size;
        Vector2 tileSpacing = new Vector2(spacing, spacing);
        Vector2 totalSpacing = tileSpacing * boardSize; 
        GetComponent<RectTransform>().sizeDelta = tileSize * boardSize + new Vector2(margin, margin) + totalSpacing;
        _tileGrid = new Match3Tile[boardSize, boardSize];
        Vector2 initialPosition = tileSize * new Vector2(-1, 1) * boardSize/2 + tileSize/2 * new Vector2(1, -1) + (tileSpacing * new Vector2(-1,1));
        for (var i = 0; i < _tileGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _tileGrid.GetLength(1); j++)
            {
                var tile = Instantiate(tilePrefab, transform, false);
                _tileGrid[j,i] = tile;
                tile.Setup(new Vector2(j,i));
                var indexPosition = new Vector3(j, -i, 0);
                tile.transform.localPosition = initialPosition + indexPosition * tileSize +
                                               tileSpacing/2 * indexPosition;
                tile.name = "Tile (" + j + "," + i + ")";
            } 
        }
    }

    public void CheckMatches()
    {
        throw new System.NotImplementedException();
    }
}

public interface IMatch3Board
{
    void ClearBoard();
    void InitializeBoard();
    void CheckMatches();
}
