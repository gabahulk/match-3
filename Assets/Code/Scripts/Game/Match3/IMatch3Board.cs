using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Match3
{
    public interface IMatch3Board
    {
        void InitializeBoard();
        List<TileCluster> CheckMatches();

        void MoveSelectedTile(Vector2Int direction);
    }
}