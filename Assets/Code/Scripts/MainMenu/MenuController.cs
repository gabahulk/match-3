using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Code.Scripts.MainMenu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject optionsMenu;
        [SerializeField] private GameObject creditsMenu;
        
        
        public void StartGame()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void OpenOptions()
        {
            OpenMenu(optionsMenu);
        }

        public void OpenCredits()
        {
            OpenMenu(creditsMenu);
        }

        private static void OpenMenu(GameObject menu)
        {
            menu.SetActive(true);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
