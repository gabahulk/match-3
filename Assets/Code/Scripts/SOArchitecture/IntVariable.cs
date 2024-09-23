using UnityEngine;

namespace Code.Scripts.SOArchitecture
{
    [CreateAssetMenu(menuName = "Code/Variables/Int Variable")]
    public class IntVariable : ScriptableObjectVariable
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        [SerializeField] private int _value;

        public int Value
        {
            get => _value;
            set => SetValue(value);
        }

        private void SetValue(int newValue)
        {
            _value = newValue;
            OnChanged();
        }

        private void SetValue(IntVariable newValue)
        {
            SetValue(newValue.Value);
        }
    }
}
