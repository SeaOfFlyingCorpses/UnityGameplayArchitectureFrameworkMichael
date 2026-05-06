using UnityEngine;
using Framework.Core;
using Gameplay.Systems.Projectiles;

namespace Gameplay.Abilities.Definitions
{
    // =========================================
    // ProjectileAbility
    // Creates an Ability that fires a pooled
    // projectile toward the target.
    //
    // Usage — register on an AI agent:
    //   abilities.Register(ProjectileAbility.Create(
    //       poolKey:  "Bullet",
    //       damage:   15,
    //       cooldown: 2f,
    //       id:       "RangedAttack"
    //   ));
    //
    // Usage — register on a player:
    //   abilities.Register(ProjectileAbility.Create(
    //       poolKey:  "Arrow",
    //       damage:   25,
    //       cooldown: 0.5f,
    //       id:       "Shoot"
    //   ));
    // =========================================
    public static class ProjectileAbility
    {
        public static Ability Create(
            string poolKey  = "Bullet",
            int    damage   = 10,
            float  cooldown = 1.5f,
            string id       = "RangedAttack")
        {
            return new Ability
            {
                Id       = id,
                Cooldown = cooldown,
                Execute  = (ctx) =>
                {
                    if (ctx.Target == null || ctx.Self == null)
                        return;

                    var registry = ServiceLocator.Get<PoolRegistry>();

                    if (registry == null || !registry.HasPool(poolKey))
                    {
                        Debug.LogWarning(
                            $"[ProjectileAbility] Pool '{poolKey}' not found. " +
                            $"Register it in GameBootstrap.");
                        return;
                    }

                    // Spawn from pool
                    Vector3 spawnPos  = ctx.Self.position + Vector3.up * 1.2f;
                    Vector3 direction = (ctx.Target.position + Vector3.up * 1f
                                       - spawnPos).normalized;

                    var obj = registry.Get(poolKey, spawnPos, Quaternion.identity);

                    if (obj == null)
                        return;

                    // Launch
                    var projectile = obj.GetComponent<Projectile>();

                    if (projectile != null)
                        projectile.Launch(spawnPos, direction, damage);
                }
            };
        }
    }
}
