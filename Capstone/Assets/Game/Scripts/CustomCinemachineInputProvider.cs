using UnityEngine;
using UnityEngine.InputSystem;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Source: https://forum.unity.com/threads/input-system-cinemachine-how-to-temporary-disable-axis.976209/
    /// </summary>
    public class CustomCinemachineInputProvider : Cinemachine.CinemachineInputProvider
    {
        [SerializeField]
        private bool inputEnabled;
        
        public bool InputEnabled { set { inputEnabled = value; } }

        public override float GetAxisValue(int axis)
        {
            if (!inputEnabled)
                return 0;
            return base.GetAxisValue(axis);
        }
    }
}
