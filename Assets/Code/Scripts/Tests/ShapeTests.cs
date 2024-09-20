using Code.Scripts.Match3;
using NUnit.Framework;

namespace Code.Scripts.Tests
{
    public class ShapeTests
    {
        [Test]
        public void ValidatorChecksIfShapeFitsAndIsAMatch()
        {
            //Arrange
            var threeTilesShapeMock = new [,]
            {
                { 1, 1, 1 },
                { 1, 0, 1 }
            };
            var shapeA = new Shape(threeTilesShapeMock);
            var threeTilesValidationShape = new [,]
            {
                { 1, 1, 1 },
            };
            var shapeB = new Shape(threeTilesValidationShape);
            
            //Act
            var result = ShapeMatchValidator.CheckIfShapeAContainsShapeB(shapeA, shapeB);
            
            //Assert
            Assert.IsTrue(result.Valid);
        }
        
        [Test]
        public void ValidatorChecksIfShapeDoesntFit()
        {
            //Arrange
            var threeTilesShapeMock = new [,]
            {
                { 1, 1, 1 },
                { 1, 0, 1 }
            };
            var shapeA = new Shape(threeTilesShapeMock);
            var threeTilesValidationShape = new [,]
            {
                { 1, 1, 1 ,1 },
            };
            var shapeB = new Shape(threeTilesValidationShape);
            
            //Act
            var result = ShapeMatchValidator.CheckIfShapeAContainsShapeB(shapeA, shapeB);
            
            //Assert
            Assert.IsFalse(result.Valid);
        }
        
        [Test]
        public void ValidatorDoesntMatchShapeWhenShapeDoesntFit()
        {
            //Arrange
            var threeTilesShapeMock = new [,]
            {
                { 1, 1, 0 },
                { 1, 0, 1 }
            };
            var shapeA = new Shape(threeTilesShapeMock);
            var threeTilesValidationShape = new [,]
            {
                { 1, 1, 1 },
            };
            var shapeB = new Shape(threeTilesValidationShape);
            
            //Act
            var result = ShapeMatchValidator.CheckIfShapeAContainsShapeB(shapeA, shapeB);
            
            //Assert
            Assert.IsFalse(result.Valid);
        }
        
        [Test]
        public void ValidatorChecksIfDifficultShapeFitsAndIsAMatch()
        {
            //Arrange
            var threeTilesShapeMock = new [,]
            {
                { 1, 1, 1 },
                { 1, 0, 1 },
                { 1, 0, 1 },
            };
            var shapeA = new Shape(threeTilesShapeMock);
            var threeTilesValidationShape = new [,]
            {
                { 1, 0, 0 },
                { 1, 0, 0 },
                { 1, 1, 1 },
            };
            var shapeB = new Shape(threeTilesValidationShape);
            
            //Act
            var result = ShapeMatchValidator.CheckIfShapeAContainsShapeB(shapeA, shapeB);
            
            //Assert
            Assert.IsTrue(result.Valid);
        }
        
        [Test]
        public void ValidatorMatchHardestCase()
        {
            //Arrange
            var threeTilesShapeMock = new [,]
            {
                { 1, 0, 0 ,0 ,1},
                { 1, 0, 1 ,0 ,0},
                { 0, 0, 1 ,1 ,1},
                { 1, 0, 1 ,0 ,0},
                { 1, 0, 1 ,0 ,0},
            };
            var shapeA = new Shape(threeTilesShapeMock);
            var threeTilesValidationShape = new [,]
            {
                { 1, 0, 0 },
                { 1, 0, 0 },
                { 1, 1, 1 },
            };
            var shapeB = new Shape(threeTilesValidationShape);
            
            //Act
            var result = ShapeMatchValidator.CheckIfShapeAContainsShapeB(shapeA, shapeB);
            
            //Assert
            Assert.IsTrue(result.Valid);
        }

    }
}
