using System;

namespace Code.Scripts.Enemy
{
    [Serializable]
    public class TileTally
    {
        public TileType tileType;
        public int quantity;

        public TileTally(TileType tileType, int quantity)
        {
            this.tileType = tileType;
            this.quantity = quantity;
        }
    }
}