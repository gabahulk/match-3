using Code.Scripts.Game.Player;
using Code.Scripts.SOArchitecture;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private PlayerTileInventory playerTileInventory;
        [SerializeField] private PlayerData playerData;
        [SerializeField] private IntVariable playerHealth;
        [SerializeField] private IntVariable enemyIncomingDamage;
        [SerializeField] private GameObject gameOverScreen;


        private void Start()
        {
            gameOverScreen.SetActive(false);
            playerTileInventory.Init();
            playerHealth.Value = playerData.startingHealth;
            playerHealth.Changed += OnDamageTaken;
        }

        private void OnDamageTaken()
        {
            if (playerHealth.Value <= 0)
            {
                GameOver();
            }
        }

        public void GameOver()
        {
            gameOverScreen.SetActive(true);
            playerHealth.Changed -= OnDamageTaken;
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
