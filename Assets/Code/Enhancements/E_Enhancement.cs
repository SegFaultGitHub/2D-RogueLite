using System;

namespace Code.Enhancements {
    public enum E_Enhancement {
        AttackPower,
        AttackSpeed,
        HerculesStamina,
        Hypocondria,
        PlagueStudy,
        PoisonMaster,
        Vampire,
        Venom,
        ViolentDash
    }

    public static class SC_EnhancementEnumExtension {
        public static string ToFriendlyString(this E_Enhancement enhancement) {
            return enhancement switch {
                E_Enhancement.AttackPower => "AttackPower",
                E_Enhancement.AttackSpeed => "AttackSpeed",
                E_Enhancement.HerculesStamina => "HerculesStamina",
                E_Enhancement.Hypocondria => "Hypocondria",
                E_Enhancement.PlagueStudy => "PlagueStudy",
                E_Enhancement.PoisonMaster => "PoisonMaster",
                E_Enhancement.Vampire => "Vampire",
                E_Enhancement.Venom => "Venom",
                E_Enhancement.ViolentDash => "ViolentDash",
                _ => throw new ArgumentOutOfRangeException(nameof(enhancement), enhancement, null)
            };
        }
    }
}
