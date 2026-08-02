using System;

namespace Code.Characters.Enemies {
    public enum E_Enemy {
        Bat,
        Zombie,
        Skeleton,
        Spider,
        Cyclop,
        Mimic,
        Centaur,
        Necromancer,
        BigSlime,
        SmallSlime,
        Ghost
    }

    public static class SC_EnemyEnumExtension {
        public static string ToFriendlyString(this E_Enemy enemy) {
            return enemy switch {
                E_Enemy.Bat => "Bat",
                E_Enemy.Zombie => "Zombie",
                E_Enemy.Skeleton => "Skeleton",
                E_Enemy.Spider => "Spider",
                E_Enemy.Cyclop => "Cyclop",
                E_Enemy.Mimic => "Mimic",
                E_Enemy.Centaur => "Centaur",
                E_Enemy.Necromancer => "Necromancer",
                E_Enemy.BigSlime => "BigSlime",
                E_Enemy.SmallSlime => "SmallSlime",
                E_Enemy.Ghost => "Ghost",
                _ => throw new ArgumentOutOfRangeException(nameof(enemy), enemy, null)
            };
        }
    }
}
