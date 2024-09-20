using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Scripts.Configs;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Match3
{
    public class TileCluster
    {
        private readonly Dictionary<Vector2Int, Match3Tile> _tiles = new();
        public Shape Shape { get; } = new();
        
        public void AddTile(Vector2Int position, Match3Tile tile)
        {
            _tiles.Add(position, tile);
        }

        public void SetTile(Vector2Int position, Match3Tile tile)
        {
            _tiles[position] = tile;
        }

        public Dictionary<Vector2Int, Match3Tile> GetTiles()
        {
            return _tiles;
        }
        
        
        public bool CanPop()
        {
            foreach (var item in _tiles)
            {
                var tilePos = item.Key;
                var hasVerticalNeighbours = _tiles.ContainsKey(tilePos + Vector2Int.down) &&
                                            _tiles.ContainsKey(tilePos + Vector2Int.up);
                var hasHorizontalNeighbours = _tiles.ContainsKey(tilePos + Vector2Int.left) &&
                                              _tiles.ContainsKey(tilePos + Vector2Int.right);
                if (hasVerticalNeighbours || hasHorizontalNeighbours)
                {
                    return true;
                }
            }
            return false;
        }
        
        public  MatchType FilterClusterToMatchType(IEnumerable<MatchType> matchTypes)
        {
            Shape.Update(_tiles);
            foreach (var matchType in matchTypes)
            {
                var match = matchType.IsMatch(this);
                if (!match.Valid) continue;
                PruneCluster(match);
                UpdateTiles();
                return matchType;
            }
            return null;
        }

        public void PruneCluster(Match match)
        {
            Shape.Clear();
            for (var i = 0; i < match.Shape.ShapeSizeX; i++)
            {
                for (var j = 0; j < match.Shape.ShapeSizeY; j++)
                {
                    Shape[match.OffsetX + i, match.OffsetY + j] = match.Shape[i, j];
                }
            }
        }

        private void UpdateTiles()
        {
            var posAux = new Vector2Int(-1, -1);
            for (var i = 0; i < Shape.ShapeSizeX; i++)
            {
                for (var j = 0; j < Shape.ShapeSizeY; j++)
                {
                    posAux.x = i + Shape.ShapeMinX;
                    posAux.y = j + Shape.ShapeMinY;
                    if (Shape[i, j] == 0 && _tiles.ContainsKey(posAux))
                    {
                        _tiles.Remove(posAux);
                    }
                }
            }
        }
        
    }
}