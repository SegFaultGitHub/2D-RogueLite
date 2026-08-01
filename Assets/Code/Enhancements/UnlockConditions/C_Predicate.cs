using System;
using System.Linq;
using Code.Managers;
using MyBox;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions {
    [Serializable]
    public class C_Predicate {
        [SerializeField] private E_Predicate m_Predicate;
        [ConditionalField(nameof(m_Predicate), false, E_Predicate.EnemiesKilled)]
        [SerializeField] private int m_EnemiesKilled;
        [ConditionalField(nameof(m_Predicate), false, E_Predicate.SpecificEnemiesKilled)]
        [SerializeField] private int m_SpecificEnemiesKilled;
        [ConditionalField(nameof(m_Predicate), false, E_Predicate.DamageDealt)]
        [SerializeField] private int m_DamageDealt;
        [ConditionalField(nameof(m_Predicate), false, E_Predicate.DamageReceived)]
        [SerializeField] private int m_DamageReceived;
        [ConditionalField(nameof(m_Predicate), false, E_Predicate.MinEnhancements)]
        [SerializeField] private int m_MinEnhancements;
        [ConditionalField(nameof(m_Predicate), false, E_Predicate.MaxEnhancements)]
        [SerializeField] private int m_MaxEnhancements;
        [ConditionalField(nameof(m_Predicate), false, E_Predicate.MinSpecificEnhancements)]
        [SerializeField] private int m_MinSpecificEnhancements;
        [ConditionalField(nameof(m_Predicate), false, E_Predicate.MaxSpecificEnhancements)]
        [SerializeField] private int m_MaxSpecificEnhancements;
        [ConditionalField(nameof(m_Predicate), false, E_Predicate.MinSpecificEnhancements, E_Predicate.MaxSpecificEnhancements)]
        [SerializeField] private E_Enhancement m_Enhancement;

        private E_Predicate Predicate { get => this.m_Predicate; }

        private int EnemiesKilled { get => this.m_EnemiesKilled; }
        private int SpecificEnemiesKilled { get => this.m_SpecificEnemiesKilled; }
        private int DamageDealt { get => this.m_DamageDealt; }
        private int DamageReceived { get => this.m_DamageReceived; }
        private int MinEnhancements { get => this.m_MinEnhancements; }
        private int MaxEnhancements { get => this.m_MaxEnhancements; }
        private int MinSpecificEnhancements { get => this.m_MinSpecificEnhancements; }
        private int MaxSpecificEnhancements { get => this.m_MaxSpecificEnhancements; }
        private E_Enhancement Enhancement { get => this.m_Enhancement; }

        public bool Check(MB_ObjectsManager objectManager) {
            return this.Predicate switch {
                E_Predicate.True => true,
                E_Predicate.False => false,
                E_Predicate.EnemiesKilled => throw new ArgumentOutOfRangeException(),
                E_Predicate.SpecificEnemiesKilled => throw new ArgumentOutOfRangeException(),
                E_Predicate.DamageDealt => throw new ArgumentOutOfRangeException(),
                E_Predicate.DamageReceived => throw new ArgumentOutOfRangeException(),
                E_Predicate.MinEnhancements => this.CheckMinEnhancements(objectManager),
                E_Predicate.MaxEnhancements => this.CheckMaxEnhancements(objectManager),
                E_Predicate.MinSpecificEnhancements => this.CheckMinSpecificEnhancements(objectManager),
                E_Predicate.MaxSpecificEnhancements => this.CheckMaxSpecificEnhancements(objectManager),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool CheckMinEnhancements(MB_ObjectsManager objectManager) {
            return objectManager.Player.Enhancements.Count >= this.MinEnhancements;
        }

        private bool CheckMaxEnhancements(MB_ObjectsManager objectManager) {
            return objectManager.Player.Enhancements.Count < this.MaxEnhancements;
        }

        private bool CheckMinSpecificEnhancements(MB_ObjectsManager objectManager) {
            return objectManager.Player.Enhancements //
                       .Value //
                       .Select(enhancement => enhancement.Enhancement == this.Enhancement) //
                       .Count()
                   >= this.MinSpecificEnhancements;
        }

        private bool CheckMaxSpecificEnhancements(MB_ObjectsManager objectManager) {
            return objectManager.Player.Enhancements //
                       .Value //
                       .Select(enhancement => enhancement.Enhancement == this.Enhancement) //
                       .Count()
                   < this.MaxSpecificEnhancements;
        }
    }
}
