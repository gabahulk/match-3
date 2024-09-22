using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Enemy
{
    public class RequirementUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text requirementText;
        [SerializeField] private  Image iconImage;
        

        public void Setup(TileType tileType, int quantity)
        {
            iconImage.sprite = tileType.tileSprite;
            UpdateRequirementUI(quantity);
        }

        public void UpdateRequirementUI(int quantity)
        {
            gameObject.SetActive(quantity > 0);
            requirementText.text = quantity.ToString();
        }
    }
}