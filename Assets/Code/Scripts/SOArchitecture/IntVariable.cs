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
        public int value;

        public void SetValue(int newValue)
        {
            value = newValue;
            OnChanged();
        }

        public void SetValue(IntVariable newValue)
        {
            SetValue(newValue.value);
        }
    }
}
