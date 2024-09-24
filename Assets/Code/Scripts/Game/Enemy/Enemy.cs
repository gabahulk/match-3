using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Enemy;
using Code.Scripts.Game.Player;
using Code.Scripts.SOArchitecture;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Game.Enemy
{
    public class Enemy : MonoBehaviour
    {
        private static readonly int Damage = Animator.StringToHash("Damage");
        private static readonly int AttackAnimation = Animator.StringToHash("Attack");
        [SerializeField] private EnemyData data;
        [SerializeField] private PlayerTileInventory playerTileInventory;
        [SerializeField] private RequirementsUI requirementsUI;
        [SerializeField] private EnemyRuntimeSet enemyRuntimeSet;
        [SerializeField] private IntVariable enemyIncomingDamageVariable;
        [SerializeField] private GameEvent enemyAttackEvent;
        [SerializeField] private GameEvent enemyRequirementMet;
        [SerializeField] private Animator animator;


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
            InvokeRepeating(nameof(Attack), data.timeBetweenAttacks, data.timeBetweenAttacks);
        }


        private void Attack()
        {
            animator.SetTrigger(AttackAnimation);
            enemyIncomingDamageVariable.Value = data.damage;
            enemyAttackEvent.Raise();
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
                    enemyRequirements[tileType] -= playerTileInventory.Inventory[tileType];
                    playerTileInventory.SetTiles(tileType, 0, false);
                }

                isRequirementCompleted &= enemyRequirements[tileType] == 0;
            }
            requirementsUI.UpdateRequirements();

            if (isRequirementCompleted)
            {
                enemyRequirementMet.Raise();
            }
        }

        public void Die()
        {
            animator.SetTrigger(Damage);
        }

        public void OnDamageAnimationEnded()
        {
            enemyRuntimeSet.Remove(this);
            Destroy(gameObject);
        }
    }
}