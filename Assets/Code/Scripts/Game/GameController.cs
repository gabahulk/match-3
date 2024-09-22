using Code.Scripts.Game.Player;
using UnityEngine;

namespace Code.Scripts.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private PlayerTileInventory playerTileInventory;

        void Start()
        {
            playerTileInventory.Init();
        }
    }
}
