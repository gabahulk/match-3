using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Enemy
{
    [CreateAssetMenu (menuName = "GameConfigs/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public List<TileTally> enemyRequirements = new();
        public int damage;
    }
}