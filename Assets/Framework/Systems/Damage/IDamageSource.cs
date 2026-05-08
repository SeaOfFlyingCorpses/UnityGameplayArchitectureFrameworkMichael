namespace Framework.Systems.Damage
{
    // =========================================
    // IDamageSource
    // Any object that can deal damage.
    // Implement on weapons, abilities, projectiles,
    // hazards, status effects.
    //
    // Usage:
    //   public class SwordWeapon : IDamageSource
    //   {
    //       public DamageInfo GetDamage()
    //           => new DamageInfo(15, DamageType.Slash);
    //   }
    // =========================================
    public interface IDamageSource
    {
        DamageInfo GetDamage();
    }
}
