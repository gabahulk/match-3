using UnityEngine;

namespace Code.Scripts.Match3
{
    public class TileMovementEventHandler : MonoBehaviour
    {
        [SerializeField] private Match3Board board;

        public void OnMoveUp()
        {
            // up is reversed because of grid position logic being different from unity's position logic
            board.MoveSelectedTile(Vector2Int.down);
        }
    
        public void OnMoveDown()
        {
            // down is reversed because of grid position logic being different from unity's position logic
            board.MoveSelectedTile(Vector2Int.up);
        }
    
        public void OnMoveLeft()
        {
            board.MoveSelectedTile(Vector2Int.left);
        }
    
        public void OnMoveRight()
        {
            board.MoveSelectedTile(Vector2Int.right);
        }
    
    }
}
