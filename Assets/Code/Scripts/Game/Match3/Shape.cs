using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Match3
{
    public class Shape
    {
        public Shape() { }

        public Shape(int[,] shapeMatrix)
        {
            ShapeMatrix = shapeMatrix;
            ShapeMaxX = shapeMatrix.GetLength(0) - 1;
            ShapeMaxY = shapeMatrix.GetLength(1) - 1;
            ShapeMinX = 0;
            ShapeMinY = 0;
            ShapeSizeX = ShapeMatrix.GetLength(0);
            ShapeSizeY = ShapeMatrix.GetLength(1);
        }

        public int[,] ShapeMatrix;
        public int ShapeMaxX;
        public int ShapeMinX;
        public int ShapeMaxY;
        public int ShapeMinY;
        public int ShapeSizeX;
        public int ShapeSizeY;
        
        public void Update(Dictionary<Vector2Int, Match3Tile> tiles)
        {
            ShapeMinX = int.MaxValue;
            ShapeMaxX = int.MinValue;
            ShapeMinY = int.MaxValue;
            ShapeMaxY = int.MinValue;
            ShapeSizeX = 0;
            ShapeSizeY = 0;
            foreach (var item in tiles)
            {
                if (item.Key.x < ShapeMinX) ShapeMinX = item.Key.x;
                if (item.Key.x > ShapeMaxX) ShapeMaxX = item.Key.x;
            
                if (item.Key.y < ShapeMinY) ShapeMinY = item.Key.y;
                if (item.Key.y > ShapeMaxY) ShapeMaxY = item.Key.y;
            }
            ShapeSizeX = ShapeMaxX - ShapeMinX + 1;
            ShapeSizeY = ShapeMaxY - ShapeMinY + 1;
            ShapeMatrix = new int[ShapeSizeX, ShapeSizeY];
        
            foreach (var item in tiles)
            {
                var i = ShapeSizeX - 1  - (ShapeMaxX - item.Key.x);
                var j = ShapeSizeY - 1 - (ShapeMaxY - item.Key.y);
            
                ShapeMatrix[i, j] = 1;
            }
        }

        public Shape RotateShapeCounterClockwise()
        {
            var newMatrix = new int[ShapeMatrix.GetLength(1), ShapeMatrix.GetLength(0)];
            var newRow = 0;
            for (var oldColumn = ShapeMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
            {
                var newColumn = 0;
                var oldRow = 0;
                for (; oldRow < ShapeMatrix.GetLength(0); oldRow++)
                {
                    newMatrix[newRow, newColumn] = ShapeMatrix[oldRow, oldColumn];
                    newColumn++;
                }
                newRow++;
            }

            return new Shape(newMatrix);
        }

        public void Clear()
        {
            for (var i = 0; i < ShapeMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < ShapeMatrix.GetLength(1); j++)
                {
                    ShapeMatrix[i, j] = 0;
                }
            }
        }
        
        public int this[int i, int j]
        {
            get => ShapeMatrix[i,j];
            set => ShapeMatrix[i,j] = value;
        }
    }
}