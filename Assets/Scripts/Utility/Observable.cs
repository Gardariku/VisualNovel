using System;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class Observable<T>
    {
        [SerializeField] private T _value;

        public Observable(T value) => _value = value;

        public delegate void ValueChangedEventHandler(T oldValue);

        public event ValueChangedEventHandler ValueChanged;
        public event Action ValueChangedCommon;

        public T Value
        {
            get => _value;
            set => Set(value);
        }

        public void Set(T value)
        {
            T oldValue = _value;
            _value = value;
            ValueChangedCommon?.Invoke();
            ValueChanged?.Invoke(oldValue);
        }

        public static implicit operator T(Observable<T> value) => value.Value;
        public static implicit operator Observable<T>(T value) => new Observable<T>(value);
    }
}
