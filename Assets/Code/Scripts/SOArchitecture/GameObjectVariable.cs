using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.SOArchitecture
{
    [CreateAssetMenu(menuName = "Code/Variables/GameObject Variable")]
    public class GameObjectVariable: ScriptableObjectVariable
    {
        
#if UNITY_EDITOR
            [Multiline]
            public string developerDescription = "";
#endif
            public GameObject value;

            public void SetValue(GameObject newValue)
            {
                value = newValue;
                OnChanged();
            }

            public void SetValue(GameObjectVariable newValue)
            {
                SetValue(newValue.value);
            }
    }
}