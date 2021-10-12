using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Provides a generic template for all class that damage other objects
    /// </summary>
    public interface IDamaging
    {
        // Property declaration
        int ID { get; set; }
        float Damage { get; }

        ObjectController ObjectController { get; set; }

        public delegate void OnDamagingFinishedDelegate(int id);

        // Event declaration
        public event OnDamagingFinishedDelegate DamagingFinished;
    }
}

