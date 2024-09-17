using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Match3Tile : MonoBehaviour, IBeginDragHandler , IDragHandler, IPointerClickHandler
{
   private const float DRAG_MARGIN = 0.7f;
   private Vector2 _gridPosition;
   private bool _isSelected = false;

   public void Setup(Vector2 gridPos)
   {
      _gridPosition = gridPos;
   }


   public void OnPointerClick(PointerEventData eventData)
   {
      _isSelected = true;
   }

   public void OnBeginDrag(PointerEventData eventData)
   {
      Debug.Log("OnBeginDrag " + name + " GameObject");
   }

   public void OnDrag(PointerEventData eventData)
   {
      var direction = (eventData.position - new Vector2(transform.position.x, transform.position.y));
      if (direction.magnitude < 25f)
         return;
      HandleSwipe(direction.normalized);
   }

   private void HandleSwipe(Vector2 normalizedDirection)
   {
      switch (normalizedDirection.x)
      {
         case > DRAG_MARGIN:
            print("Right!");
            break;
         case < -DRAG_MARGIN:
            print("Left!");
            break;
         default:
            switch (normalizedDirection.y)
            {
               case > DRAG_MARGIN:
                  print("Up!");
                  break;
               case < -DRAG_MARGIN:
                  print("Down!");
                  break;
            }
            break;
      }
   }
}
