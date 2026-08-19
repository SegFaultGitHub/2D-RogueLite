using System;

namespace Code.Characters {
    public enum E_DamageSource {
        Direct,
        Melee,
        Burning,
        Poison,
        Passive,
        Traps,
        SummonDeath
    }

    public static class SC_DamageSourceEnumExtension {
        public static string ToFriendlyString(this E_DamageSource damageSource) {
            return damageSource switch {
                E_DamageSource.Direct => "Direct",
                E_DamageSource.Melee => "Melee",
                E_DamageSource.Burning => "Burning",
                E_DamageSource.Poison => "Poison",
                E_DamageSource.Passive => "Passive",
                E_DamageSource.Traps => "Traps",
                E_DamageSource.SummonDeath => "SummonDeath",
                _ => throw new ArgumentOutOfRangeException(nameof(damageSource), damageSource, null)
            };
        }
    }
}
