using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Enemy
{
    public class RequirementsUI : MonoBehaviour
    {

        [SerializeField] private GameObject requirementUIPrefab;
    
        private Dictionary<TileType, int> requirements = new();
        private Dictionary<TileType, RequirementUI> requirementUIs = new();
        
        public void Setup(Dictionary<TileType, int> enemyRequirements)
        {
            requirements = enemyRequirements;
            foreach (var item in requirements)
            {
                var requirementUI = Instantiate(requirementUIPrefab, transform).GetComponent<RequirementUI>();
                requirementUI.Setup(item.Key, item.Value);
                requirementUIs.Add(item.Key, requirementUI);
            }
        }
    
        public void UpdateRequirements()
        {
            foreach (var item in requirements)
            {
                requirementUIs[item.Key].UpdateRequirementUI(item.Value);
            }
        }
    }
}
