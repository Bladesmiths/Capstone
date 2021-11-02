using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Provides a generic template for all class that damage other objects
    /// </summary>
    public interface IDamaging : IIdentified
    {
        // Property declaration
        float Damage { get; }

        // This should probably get removed as soon as objects call their
        // Damaging Finished event at the end of an animation or something instead of time-based
        bool Damaging { get; set; }

        public delegate void OnDamagingFinishedDelegate(int id);

        // Event declaration
        public event OnDamagingFinishedDelegate DamagingFinished;
    }
}

