using System.Collections.Generic;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Provides a generic template for all the damageable classes.
    /// </summary>
    public interface IDamageable
    {
        // Property declaration
        int ID { get; set; }
        bool IsAlive { get; set; }
        float Health { get; set; }
        float MaxHealth { get; set; }
        List<int> DamagingObjectIDs { get; }
        Team DamageableObjectTeam { get; set; }
        ObjectController ObjectController { get; set; }

        // Method declaration
        bool TakeDamage(int damagingID, float damage);
        void RemoveDamagingID(int damagingID); 
    }
}
