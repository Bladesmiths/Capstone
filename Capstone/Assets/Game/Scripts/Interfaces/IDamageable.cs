namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Provides a generic template for all the damageable classes.
    /// </summary>
    public interface IDamageable
    {
        // Property declaration
        bool IsAlive { get; set; }
        float Health { get; set; }
        float MaxHealth { get; set; }

        // Method declaration
        void TakeDamage(float damage);
    }
}
