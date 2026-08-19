using System;

namespace Code.Enhancements {
    public enum E_Enhancement {
        AttackPower,
        AttackSpeed,
        HerculesStamina,
        Hypochondria,
        PlagueStudy,
        PoisonMaster,
        Vampire,
        Venom,
        ViolentDash,
        Inhibition,
        EnhancedPotential,
        Wisp,
        BrambleArmor,
        HuntersInstinct,
        WeakPoint,
        BluntHit,
        Berserk,
        Focus
    }

    public static class SC_EnhancementEnumExtension {
        public static string ToFriendlyString(this E_Enhancement enhancement) {
            return enhancement switch {
                E_Enhancement.AttackPower => "AttackPower",
                E_Enhancement.AttackSpeed => "AttackSpeed",
                E_Enhancement.HerculesStamina => "HerculesStamina",
                E_Enhancement.Hypochondria => "Hypochondria",
                E_Enhancement.PlagueStudy => "PlagueStudy",
                E_Enhancement.PoisonMaster => "PoisonMaster",
                E_Enhancement.Vampire => "Vampire",
                E_Enhancement.Venom => "Venom",
                E_Enhancement.ViolentDash => "ViolentDash",
                E_Enhancement.Inhibition => "Inhibition",
                E_Enhancement.EnhancedPotential => "EnhancedPotential",
                E_Enhancement.Wisp => "Wisp",
                E_Enhancement.BrambleArmor => "BrambleArmor",
                E_Enhancement.HuntersInstinct => "HuntersInstinct",
                E_Enhancement.WeakPoint => "WeakPoint",
                E_Enhancement.BluntHit => "BluntHit",
                E_Enhancement.Berserk => "Berserk",
                E_Enhancement.Focus => "Focus",
                _ => throw new ArgumentOutOfRangeException(nameof(enhancement), enhancement, null)
            };
        }
    }
}
