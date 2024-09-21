using UnityEngine;

namespace Code.Scripts.MainMenu
{
   public class BackButtonHideBehavior : MonoBehaviour
   {
      public void Hide()
      {
         gameObject.SetActive(false);
      }
   }
}
