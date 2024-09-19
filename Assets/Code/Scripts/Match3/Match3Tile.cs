using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Scripts.SOArchitecture;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Match3Tile : MonoBehaviour, IDragHandler
{

   [SerializeField] private Image tileImage;
   [SerializeField] private GameObjectVariable selectedTileVariable;
   [SerializeField] private GameEvent moveCurrentTileUpEvent;
   [SerializeField] private GameEvent moveCurrentTileDownEvent;
   [SerializeField] private GameEvent moveCurrentTileLeftEvent;
   [SerializeField] private GameEvent moveCurrentTileRightEvent;
   
   private const float DRAG_MARGIN = 0.7f;
   private Vector2Int _gridPosition;
   private bool _isSelected = false;
   private TileType _tileType;

   public void Setup(Vector2Int gridPos, TileType tileType)
   {
      _gridPosition = gridPos;
      _tileType = tileType;
      tileImage.sprite = tileType.tileSprite;
   }

   public Vector2Int GetTilePositionInGrid()
   {
      return new Vector2Int(_gridPosition.x,_gridPosition.y);
   }
   
   public void SetTilePositionInGrid(Vector2Int gridPos)
   {
      _gridPosition = gridPos;
   }

   public void OnDrag(PointerEventData eventData)
   {
      var direction = (eventData.position - new Vector2(transform.position.x, transform.position.y));
      if (direction.magnitude < 25f || _isSelected)
         return;
      HandleSwipe(direction.normalized);
      SwapCooldown();
   }

   private async void SwapCooldown()
   {
      _isSelected = true;
      await Task.Delay(500);
      _isSelected = false;
   }

   private void HandleSwipe(Vector2 normalizedDirection)
   {
      _isSelected = true;
      selectedTileVariable.SetValue(transform.gameObject);
      switch (normalizedDirection.x)
      {
         case > DRAG_MARGIN:
            moveCurrentTileRightEvent.Raise();
            break;
         case < -DRAG_MARGIN:
            moveCurrentTileLeftEvent.Raise();
            break;
         default:
            switch (normalizedDirection.y)
            {
               case > DRAG_MARGIN:
                  moveCurrentTileUpEvent.Raise();
                  break;
               case < -DRAG_MARGIN:
                  moveCurrentTileDownEvent.Raise();
                  break;
            }
            break;
      }
      _isSelected = false;
   }

   public TileType GetTileType()
   {
      return _tileType;
   }
}
