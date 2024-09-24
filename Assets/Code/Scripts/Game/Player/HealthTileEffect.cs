using System;
using Code.Scripts.SOArchitecture;
using UnityEngine;

namespace Code.Scripts.Game.Player
{
    public class HealthTileEffect : MonoBehaviour
    {
        public PlayerTileInventory playerTileInventory;
        public TileType heartTileType;
        public IntVariable health;


        public void ApplyTileEffect()
        {
            health.Value += playerTileInventory.Inventory[heartTileType] * 5;
            health.Value = Math.Max(health.Value, 100);
        }
    }
}
