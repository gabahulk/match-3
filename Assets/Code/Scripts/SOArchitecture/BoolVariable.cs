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
            [SerializeField] private bool _value;
            public bool Value
            {
                get => _value;
                set => SetValue(value);
            }
            private void SetValue(bool newValue)
            {
                _value = newValue;
                OnChanged();
            }

            public void SetValue(GameObjectVariable newValue)
            {
                SetValue(newValue.Value);
            }
        }
}