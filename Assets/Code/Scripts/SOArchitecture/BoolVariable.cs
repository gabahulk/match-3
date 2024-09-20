using UnityEngine;

namespace Code.Scripts.SOArchitecture
{
    [CreateAssetMenu(menuName = "Code/Variables/Bool Variable")]
    public class BoolVariable: ScriptableObject
    {
        
#if UNITY_EDITOR
            [Multiline]
            public string developerDescription = "";
#endif
            public bool value;

            public void SetValue(bool newValue)
            {
                value = newValue;
            }

            public void SetValue(GameObjectVariable newValue)
            {
                value = newValue.value;
            }
        }
}