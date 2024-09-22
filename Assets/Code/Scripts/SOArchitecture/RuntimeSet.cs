using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.SOArchitecture
{
    public abstract class RuntimeSet<T> : ScriptableObjectVariable
    {
        public List<T> items = new ();

        public void Add(T item)
        {
            if (items.Contains(item)) return;
            items.Add(item);
            OnChanged();
        }

        public void Remove(T item)
        {
            if (!items.Contains(item)) return;
            items.Remove(item);
            OnChanged();
        }

        public T this[int index]
        {
            get => items[index];
            set => items[index] = value;
        }
    }
}