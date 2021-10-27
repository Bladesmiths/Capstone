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
        bool Damaging { get; set; }

        public delegate void OnDamagingFinishedDelegate(int id);

        // Event declaration
        public event OnDamagingFinishedDelegate DamagingFinished;
    }
}

