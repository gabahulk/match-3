using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Enemy;
using Code.Scripts.Game.Player;
using UnityEngine;

namespace Code.Scripts.Game.Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyData data;
        [SerializeField] private PlayerTileInventory playerTileInventory;
        [SerializeField] private RequirementsUI requirementsUI;
        [SerializeField] private EnemyRuntimeSet enemyRuntimeSet;


        private readonly Dictionary<TileType, int> enemyRequirements = new();
        
        void Start()
        {
            foreach (var tally in data.enemyRequirements)
            {
                enemyRequirements.Add(tally.tileType, tally.quantity);
            }

            requirementsUI.Setup(enemyRequirements);
            playerTileInventory.Changed += UpdateTally;
            UpdateTally();
        }

        public void OnDestroy()
        {
            playerTileInventory.Changed -= UpdateTally;
        }

        private void UpdateTally()
        {
            var isRequirementCompleted = true;
            foreach (var tileType in enemyRequirements.Keys.ToList())
            {
                if(enemyRequirements[tileType] <= 0) continue;
                if (playerTileInventory.Inventory[tileType] >= enemyRequirements[tileType])
                {
                    playerTileInventory.UpdateTiles(tileType, -enemyRequirements[tileType], false);
                    enemyRequirements[tileType] = 0;
                }
                else if(playerTileInventory.Inventory[tileType] > 0)
                {
                    var aux = enemyRequirements[tileType];
                    enemyRequirements[tileType] -= playerTileInventory.Inventory[tileType];
                    playerTileInventory.UpdateTiles(tileType, -aux, false);
                }

                isRequirementCompleted &= enemyRequirements[tileType] == 0;
            }
            
            requirementsUI.UpdateRequirements();

            if (isRequirementCompleted)
            {
                Die();
            }
        }

        private void Die()
        {
            enemyRuntimeSet.Remove(this);
            Destroy(gameObject);
        }
    }
}