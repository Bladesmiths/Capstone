using System.Collections.Generic;
using Bladesmiths.Capstone.Enums;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Provides a generic template for all the damageable classes.
    /// </summary>
    public interface IIdentified
    {
        // Property declaration
        GameObject GameObject { get; set; }
        int ID { get; set; }
        Team ObjectTeam { get; set; }

        ObjectController ObjectController { get; set; }

        public delegate void OnDestructionDelegate(int id);

        // Event declaration
        public event OnDestructionDelegate OnDestruction;
    }
}
