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
            [SerializeField] private GameObject _value;

            public GameObject Value
            {
                get => _value;
                set => SetValue(value);
            }

            public void SetValue(GameObject newValue)
            {
                _value = newValue;
                OnChanged();
            }

            public void SetValue(GameObjectVariable newValue)
            {
                SetValue(newValue._value);
            }
    }
}