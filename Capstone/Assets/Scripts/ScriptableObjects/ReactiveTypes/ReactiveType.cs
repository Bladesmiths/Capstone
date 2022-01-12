using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Source: https://answers.unity.com/questions/1234096/is-it-possible-to-pick-reference-variables-from-on.html
/// </summary>

namespace Bladesmiths.Capstone
{
    public abstract class ReactiveType<T> : ScriptableObject
    {
        [SerializeField]
        private T initialDataValue;

        [SerializeField]
        private T currentDataValue;

        public T CurrentValue
        {
            get { return currentDataValue; }
            set
            {
                if (!value.Equals(currentDataValue))
                {
                    currentDataValue = value;
                    if (OnValueChanged != null)
                    {
                        OnValueChanged();
                    }
                }
            }
        }

        public event Action OnValueChanged;

        public void Reset()
        {
            currentDataValue = initialDataValue;
        }

        private void OnEnable()
        {
            Reset();
        }

        public override string ToString()
        {
            return currentDataValue.ToString();
        }
    }
}

