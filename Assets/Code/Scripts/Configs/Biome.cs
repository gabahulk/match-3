using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Configs
{
    [CreateAssetMenu(fileName = "new Biome", menuName = "GameConfigs/Biome")]
    public class Biome : ScriptableObject
    {
        public Sprite[] backgroundSprites;
        public List<Game.Enemy.Enemy> enemies;
        public Vector2Int enemyQuantity;
    }
}
