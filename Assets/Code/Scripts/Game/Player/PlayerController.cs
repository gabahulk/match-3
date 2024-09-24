using Code.Scripts.SOArchitecture;
using UnityEngine;

namespace Code.Scripts.Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int Damage = Animator.StringToHash("Damage");
        private static readonly int Attack = Animator.StringToHash("Attack");
        [SerializeField] private PlayerTileInventory playerTileInventory;
        [SerializeField] private IntVariable playerHealth;
        [SerializeField] private IntVariable enemyIncomingDamage;
        [SerializeField] private Animator animator;
    
        public void OnEnemyAttackEvent()
        {
            playerHealth.Value -= enemyIncomingDamage.Value;
            animator.SetTrigger(Damage);
        }

        public void OnEnemyRequirementsMet()
        {
            animator.SetTrigger(Attack);
            playerTileInventory.ClearInventory();
        }
    }
}
