using System;
using System.Linq;
using Code.Characters;
using Code.Characters.Enemies;
using Code.Managers;
using MyBox;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions {
    [Serializable]
    public class C_Predicate {
        [SerializeField] private E_Mode m_Mode;
        [SerializeField] private E_Predicate m_Predicate;

        [ConditionalField( //
            nameof(m_Predicate), false,
            E_Predicate.EnemiesKilled,
            E_Predicate.SpecificEnemiesKilled,
            E_Predicate.DamageDealt,
            E_Predicate.DamageReceived,
            E_Predicate.DamageDealtToSpecificEnemies,
            E_Predicate.DamageReceivedFromSpecificEnemies,
            E_Predicate.DamageDealtFromSpecificSource,
            E_Predicate.DamageReceivedFromSpecificSource,
            E_Predicate.MinEnhancementsTaken,
            E_Predicate.MaxEnhancementsTaken,
            E_Predicate.MinSpecificEnhancementsTaken,
            E_Predicate.MaxSpecificEnhancementsTaken
        )]
        [SerializeField] private int m_Value;

        [ConditionalField( //
            nameof(m_Predicate),
            false,
            E_Predicate.SpecificEnemiesKilled,
            E_Predicate.DamageDealtToSpecificEnemies,
            E_Predicate.DamageReceivedFromSpecificEnemies
        )]
        [SerializeField] private E_Enemy m_Enemy;
        [ConditionalField( //
            nameof(m_Predicate),
            false,
            E_Predicate.DamageDealtFromSpecificSource,
            E_Predicate.DamageReceivedFromSpecificSource
        )]
        [SerializeField] private E_DamageSource m_Source;
        [ConditionalField( //
            nameof(m_Predicate),
            false,
            E_Predicate.MinSpecificEnhancementsTaken,
            E_Predicate.MaxSpecificEnhancementsTaken
        )]
        [SerializeField] private E_Enhancement m_Enhancement;

        private E_Mode Mode { get => this.m_Mode; }
        private E_Predicate Predicate { get => this.m_Predicate; }

        private int Value { get => this.m_Value; }

        private E_Enemy Enemy { get => this.m_Enemy; }
        private E_DamageSource Source { get => this.m_Source; }
        private E_Enhancement Enhancement { get => this.m_Enhancement; }

        public bool Check(MB_ObjectsManager objectManager) {
            return this.Predicate switch {
                E_Predicate.True => true,
                E_Predicate.False => false,
                E_Predicate.EnemiesKilled => this.CheckEnemiesKilled(objectManager),
                E_Predicate.SpecificEnemiesKilled => this.CheckSpecificEnemiesKilled(objectManager),
                E_Predicate.DamageDealt => this.CheckDamageDealt(objectManager),
                E_Predicate.DamageReceived => this.CheckDamageReceived(objectManager),
                E_Predicate.DamageDealtToSpecificEnemies => this.CheckDamageDealtToSpecificEnemies(objectManager),
                E_Predicate.DamageReceivedFromSpecificEnemies => this.CheckDamageReceivedFromSpecificEnemies(objectManager),
                E_Predicate.DamageDealtFromSpecificSource => this.CheckDamageDealtFromSpecificSource(objectManager),
                E_Predicate.DamageReceivedFromSpecificSource => this.CheckDamageReceivedFromSpecificSource(objectManager),
                E_Predicate.MinEnhancementsTaken => this.CheckMinEnhancements(objectManager),
                E_Predicate.MaxEnhancementsTaken => this.CheckMaxEnhancements(objectManager),
                E_Predicate.MinSpecificEnhancementsTaken => this.CheckMinSpecificEnhancements(objectManager),
                E_Predicate.MaxSpecificEnhancementsTaken => this.CheckMaxSpecificEnhancements(objectManager),
                E_Predicate.MinEnhancementsOwned => throw new ArgumentOutOfRangeException(),
                E_Predicate.MaxEnhancementsOwned => throw new ArgumentOutOfRangeException(),
                E_Predicate.MinSpecificEnhancementsOwned => throw new ArgumentOutOfRangeException(),
                E_Predicate.MaxSpecificEnhancementsOwned => throw new ArgumentOutOfRangeException(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool CheckEnemiesKilled(MB_ObjectsManager objectManager) {
            return this.Mode switch {
                E_Mode.CurrentRun => objectManager.StatsManager.CurrentRunStats.GetKilled() >= this.Value,
                E_Mode.Global => objectManager.StatsManager.GlobalStats.GetKilled() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool CheckSpecificEnemiesKilled(MB_ObjectsManager objectManager) {
            return this.Mode switch {
                E_Mode.CurrentRun => objectManager.StatsManager.CurrentRunStats.GetKilled(this.Enemy) >= this.Value,
                E_Mode.Global => objectManager.StatsManager.GlobalStats.GetKilled(this.Enemy) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool CheckDamageDealt(MB_ObjectsManager objectManager) {
            return this.Mode switch {
                E_Mode.CurrentRun => objectManager.StatsManager.CurrentRunStats.GetDamageDealt() >= this.Value,
                E_Mode.Global => objectManager.StatsManager.GlobalStats.GetDamageDealt() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool CheckDamageReceived(MB_ObjectsManager objectManager) {
            return this.Mode switch {
                E_Mode.CurrentRun => objectManager.StatsManager.CurrentRunStats.GetDamageReceived() >= this.Value,
                E_Mode.Global => objectManager.StatsManager.GlobalStats.GetDamageReceived() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool CheckDamageDealtToSpecificEnemies(MB_ObjectsManager objectManager) {
            return this.Mode switch {
                E_Mode.CurrentRun => objectManager.StatsManager.CurrentRunStats.GetDamageDealt() >= this.Value,
                E_Mode.Global => objectManager.StatsManager.GlobalStats.GetDamageDealt() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool CheckDamageReceivedFromSpecificEnemies(MB_ObjectsManager objectManager) {
            return this.Mode switch {
                E_Mode.CurrentRun => objectManager.StatsManager.CurrentRunStats.GetDamageReceived(this.Enemy) >= this.Value,
                E_Mode.Global => objectManager.StatsManager.GlobalStats.GetDamageReceived(this.Enemy) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool CheckDamageDealtFromSpecificSource(MB_ObjectsManager objectManager) {
            return this.Mode switch {
                E_Mode.CurrentRun => objectManager.StatsManager.CurrentRunStats.GetDamageDealt(this.Source) >= this.Value,
                E_Mode.Global => objectManager.StatsManager.GlobalStats.GetDamageDealt(this.Source) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool CheckDamageReceivedFromSpecificSource(MB_ObjectsManager objectManager) {
            return this.Mode switch {
                E_Mode.CurrentRun => objectManager.StatsManager.CurrentRunStats.GetDamageReceived(this.Source) >= this.Value,
                E_Mode.Global => objectManager.StatsManager.GlobalStats.GetDamageReceived(this.Source) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool CheckMinEnhancements(MB_ObjectsManager objectManager) {
            return objectManager.Player.Enhancements.Count >= this.Value;
        }

        private bool CheckMaxEnhancements(MB_ObjectsManager objectManager) {
            return objectManager.Player.Enhancements.Count < this.Value;
        }

        private bool CheckMinSpecificEnhancements(MB_ObjectsManager objectManager) {
            return objectManager.Player.Enhancements //
                       .Value //
                       .Select(enhancement => enhancement.Enhancement == this.Enhancement) //
                       .Count()
                   >= this.Value;
        }

        private bool CheckMaxSpecificEnhancements(MB_ObjectsManager objectManager) {
            return objectManager.Player.Enhancements //
                       .Value //
                       .Select(enhancement => enhancement.Enhancement == this.Enhancement) //
                       .Count()
                   < this.Value;
        }
    }
}
