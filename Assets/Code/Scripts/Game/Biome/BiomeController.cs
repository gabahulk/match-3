using System.Collections.Generic;
using Code.Scripts.Game.Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts.Game.Biome
{
    public class BiomeController : MonoBehaviour
    {
        public List<Configs.Biome> biomes;
        public Transform enemyContainer;
        public EnemyRuntimeSet enemyRuntimeSet;
    
        private Configs.Biome currentBiome;
        private int numberOfEnemies;
    
        private void Start()
        {
            CreateBiome();
            enemyRuntimeSet.Changed += OnEnemiesChanged;
        }

        private void CreateBiome()
        {
            enemyRuntimeSet.items.Clear();
            currentBiome = biomes[Random.Range(0, biomes.Count)];
            numberOfEnemies = Random.Range(currentBiome.enemyQuantity.x, currentBiome.enemyQuantity.y);
            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            var currentEnemy = Instantiate(currentBiome.enemies[Random.Range(0, currentBiome.enemies.Count)], enemyContainer);
            enemyRuntimeSet.Add(currentEnemy);
        }

        private void OnEnemiesChanged()
        {
            if (enemyRuntimeSet.items.Count > 0) return;

            numberOfEnemies--;
            if (numberOfEnemies <= 0)
            {
                CreateBiome();
            }
            else
            {
                SpawnEnemy();
            }
        }
    }
}
