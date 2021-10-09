using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public interface IDamaging
    {
        int ID { get; set; }
        ObjectController ObjectController { get; set; }

        public delegate void OnDamagingFinishedDelegate(int id); 

        public event OnDamagingFinishedDelegate DamagingFinished;  
    }
}

