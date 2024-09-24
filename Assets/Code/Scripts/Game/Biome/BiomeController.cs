using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Scripts.Game.Enemy;
using Code.Scripts.SOArchitecture;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Scripts.Game.Biome
{
    public class BiomeController : MonoBehaviour
    {
        public List<Configs.Biome> biomes;
        public Transform enemyContainer;
        public EnemyRuntimeSet enemyRuntimeSet;
        public Image biomeImage;
        public BoolVariable isBoardLockedVariable;
    
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
            biomeImage.sprite = currentBiome.backgroundSprites[Random.Range(0, currentBiome.backgroundSprites.Length)];
            numberOfEnemies = Random.Range(currentBiome.enemyQuantity.x, currentBiome.enemyQuantity.y);
            isBoardLockedVariable.Value = true;
            SpawnEnemy();
            isBoardLockedVariable.Value = false;
        }

        private async void SpawnEnemy()
        {
            await Task.Delay(1000);
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
