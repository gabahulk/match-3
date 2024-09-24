using System.Collections.Generic;
using Code.Scripts.Configs;
using Code.Scripts.Enemy;
using Code.Scripts.SOArchitecture;
using UnityEngine;

namespace Code.Scripts.Game.Player
{
    [CreateAssetMenu(menuName = "GameConfigs/PlayerTileInventory")]
    public class PlayerTileInventory : ScriptableObjectVariable
    {
        public GameConfig config;

        public readonly Dictionary<TileType, int> Inventory = new();
        //This is just for editor visibility
        [SerializeField] private List<TileTally> _tileTallies = new();
        public void Init()
        {
            Inventory.Clear();
            _tileTallies.Clear();
            foreach (var tileType in config.TileTypes)
            {
                Inventory.Add(tileType, 0);
                _tileTallies.Add(new TileTally(tileType, 0));
            }
        }

        public void UpdateTiles(TileType tileType, int amount, bool shouldCallEvent = true)
        {
            Inventory[tileType] += amount;
            _tileTallies.Find(x => x.tileType == tileType).quantity += amount;
            if (shouldCallEvent)
                OnChanged();
        }
        
        public void SetTiles(TileType tileType, int amount, bool shouldCallEvent = true)
        {
            Inventory[tileType] = amount;
            _tileTallies.Find(x => x.tileType == tileType).quantity = amount;
            if (shouldCallEvent)
                OnChanged();
        }

        public void ClearInventory()
        {
            foreach (var tileType in config.TileTypes)
            {
                SetTiles(tileType, 0, false);
            }
        }

        public int this[TileType tileType]
        {
            get => Inventory[tileType];
            set => UpdateTiles(tileType, value);
        }
        
    }
}