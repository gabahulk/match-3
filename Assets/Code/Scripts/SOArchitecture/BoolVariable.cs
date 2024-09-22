using UnityEngine;

namespace Code.Scripts.SOArchitecture
{
    [CreateAssetMenu(menuName = "Code/Variables/Bool Variable")]
    public class BoolVariable: ScriptableObjectVariable
    {
        
#if UNITY_EDITOR
            [Multiline]
            public string developerDescription = "";
#endif
            public bool value;

            public void SetValue(bool newValue)
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