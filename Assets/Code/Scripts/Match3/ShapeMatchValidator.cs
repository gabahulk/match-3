using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Match3
{
    public struct Match
    {
        public int OffsetX;
        public int OffsetY;
        public Shape Shape;
        public readonly bool Valid;

        public Match(int offsetX, int offsetY, bool valid, Shape shape)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            Valid = valid;
            Shape = shape;
        }

        public Match(bool valid)
        {
            Valid = valid;
            OffsetX = 0;
            OffsetY = 0;
            Shape = null;
        }
    }
    
    public static class ShapeMatchValidator
    {
        public static Match CheckIfShapeAContainsShapeB(Shape shapeA, Shape shapeB)
        {
            if (!FitCheck(shapeA, shapeB))
                return new Match(valid:false);
            var numberOfRotations = 0;

            while (numberOfRotations <= 3)
            {
                Match match = IsMatch(shapeA, shapeB);
                if (match.Valid)
                    return match;
                numberOfRotations += 1;
                shapeB = shapeB.RotateShapeCounterClockwise();
                if (FitCheck(shapeA, shapeB)) continue;
                numberOfRotations += 1;
                shapeB = shapeB.RotateShapeCounterClockwise();
            }


            return new Match(valid:false);
        }

        private static Match IsMatch(Shape shapeA, Shape shapeB)
        {
            var headMaxX = shapeA.ShapeSizeX - shapeB.ShapeSizeX;
            var headMaxY = shapeA.ShapeSizeY - shapeB.ShapeSizeY;
            
            // I have no idea how to reduce this complexity, if you know let me know!
            for (var i = 0; i <= headMaxX; i++)
            {
                for (var j = 0; j <= headMaxY; j++)
                {
                    if (MatchWindow(i, j, shapeA, shapeB))
                        return new Match(i,j, true, shapeB);
                }
            }
            return new Match(valid:false);
        }

        private static bool MatchWindow(int startI, int startJ, Shape shapeTarget, Shape shapeTest)
        {
            for (var i = 0; i < shapeTest.ShapeSizeX; i++)
            {
                for (var j = 0; j < shapeTest.ShapeSizeY; j++)
                {
                    if (shapeTarget[startI+i,startJ+j] - shapeTest[i,j] < 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool FitCheck(Shape shapeA, Shape shapeB)
        {
            if (ShapeBFitsInShapeA(shapeA, shapeB)) return true;
            shapeB = shapeB.RotateShapeCounterClockwise();
            return ShapeBFitsInShapeA(shapeA, shapeB);
        }

        private static bool ShapeBFitsInShapeA(Shape shapeA, Shape shapeB)
        {
            return shapeA.ShapeSizeX >= shapeB.ShapeSizeX && shapeA.ShapeSizeY >= shapeB.ShapeSizeY;
        }
    }
}