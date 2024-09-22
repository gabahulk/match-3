using System;
using UnityEngine;

namespace Code.Scripts.SOArchitecture
{
    public class ScriptableObjectVariable : ScriptableObject
    {
        public delegate void OnChangedHandler();
        public event OnChangedHandler Changed;

        protected virtual void OnChanged()
        {
            Changed?.Invoke();
        }
    }
}
