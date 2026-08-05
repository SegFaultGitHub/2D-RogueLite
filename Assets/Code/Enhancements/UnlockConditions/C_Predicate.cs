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
            nameof(m_Predicate),
            false,
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
            E_Predicate.MaxSpecificEnhancementsTaken,
            E_Predicate.MinEnhancementsOwned,
            E_Predicate.MaxEnhancementsOwned,
            E_Predicate.MinSpecificEnhancementsOwned,
            E_Predicate.MaxSpecificEnhancementsOwned,
            E_Predicate.MinEnhancementsOwnedMaxLevel,
            E_Predicate.MaxEnhancementsOwnedMaxLevel,
            E_Predicate.MinSpecificEnhancementsOwnedMaxLevel,
            E_Predicate.MaxSpecificEnhancementsOwnedMaxLevel,
            E_Predicate.DashesPerformed
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
            E_Predicate.MaxSpecificEnhancementsTaken,
            E_Predicate.MinSpecificEnhancementsOwned,
            E_Predicate.MaxSpecificEnhancementsOwned,
            E_Predicate.MinSpecificEnhancementsOwnedMaxLevel,
            E_Predicate.MaxSpecificEnhancementsOwnedMaxLevel
        )]
        [SerializeField] private E_Enhancement m_Enhancement;

        private E_Mode Mode { get => this.m_Mode; }
        private E_Predicate Predicate { get => this.m_Predicate; }

        private int Value { get => this.m_Value; }

        private E_Enemy Enemy { get => this.m_Enemy; }
        private E_DamageSource Source { get => this.m_Source; }
        private E_Enhancement Enhancement { get => this.m_Enhancement; }

        public bool Check(MB_ObjectsManager objectsManager) {
            return this.Predicate switch {
                E_Predicate.True => true,
                E_Predicate.False => false,
                E_Predicate.EnemiesKilled => this.CheckEnemiesKilled(objectsManager),
                E_Predicate.SpecificEnemiesKilled => this.CheckSpecificEnemiesKilled(objectsManager),
                E_Predicate.DamageDealt => this.CheckDamageDealt(objectsManager),
                E_Predicate.DamageReceived => this.CheckDamageReceived(objectsManager),
                E_Predicate.DamageDealtToSpecificEnemies => this.CheckDamageDealtToSpecificEnemies(objectsManager),
                E_Predicate.DamageReceivedFromSpecificEnemies => this.CheckDamageReceivedFromSpecificEnemies(objectsManager),
                E_Predicate.DamageDealtFromSpecificSource => this.CheckDamageDealtFromSpecificSource(objectsManager),
                E_Predicate.DamageReceivedFromSpecificSource => this.CheckDamageReceivedFromSpecificSource(objectsManager),
                E_Predicate.MinEnhancementsTaken => this.CheckMinEnhancementsTaken(objectsManager),
                E_Predicate.MaxEnhancementsTaken => this.CheckMaxEnhancementsTaken(objectsManager),
                E_Predicate.MinSpecificEnhancementsTaken => this.CheckMinSpecificEnhancementsTaken(objectsManager),
                E_Predicate.MaxSpecificEnhancementsTaken => this.CheckMaxSpecificEnhancementsTaken(objectsManager),
                E_Predicate.MinEnhancementsOwned => this.CheckMinEnhancementsOwned(objectsManager),
                E_Predicate.MaxEnhancementsOwned => this.CheckMaxEnhancementsOwned(objectsManager),
                E_Predicate.MinSpecificEnhancementsOwned => this.CheckMinSpecificEnhancementsOwned(objectsManager),
                E_Predicate.MaxSpecificEnhancementsOwned => this.CheckMaxSpecificEnhancementsOwned(objectsManager),
                E_Predicate.DashesPerformed => this.CheckDashesPerformed(objectsManager),
                E_Predicate.MinEnhancementsOwnedMaxLevel => this.CheckMinEnhancementsOwnedMaxLevel(objectsManager),
                E_Predicate.MaxEnhancementsOwnedMaxLevel => this.CheckMaxEnhancementsOwnedMaxLevel(objectsManager),
                E_Predicate.MinSpecificEnhancementsOwnedMaxLevel => this.CheckMinSpecificEnhancementsOwnedMaxLevel(objectsManager),
                E_Predicate.MaxSpecificEnhancementsOwnedMaxLevel => this.CheckMaxSpecificEnhancementsOwnedMaxLevel(objectsManager),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool CheckEnemiesKilled(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetKilled() >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetKilled() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckSpecificEnemiesKilled(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetKilled(this.Enemy) >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetKilled(this.Enemy) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckDamageDealt(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetDamageDealt() >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetDamageDealt() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckDamageReceived(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetDamageReceived() >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetDamageReceived() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckDamageDealtToSpecificEnemies(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetDamageDealt() >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetDamageDealt() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckDamageReceivedFromSpecificEnemies(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetDamageReceived(this.Enemy) >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetDamageReceived(this.Enemy) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckDamageDealtFromSpecificSource(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetDamageDealt(this.Source) >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetDamageDealt(this.Source) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckDamageReceivedFromSpecificSource(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetDamageReceived(this.Source) >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetDamageReceived(this.Source) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckMinEnhancementsTaken(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsTaken() >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsTaken() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckMaxEnhancementsTaken(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsTaken() < this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsTaken() < this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckMinSpecificEnhancementsTaken(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsTaken(this.Enhancement) >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsTaken(this.Enhancement) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckMaxSpecificEnhancementsTaken(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsTaken(this.Enhancement) < this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsTaken(this.Enhancement) < this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckMinEnhancementsOwned(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsMaxOwned() >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsMaxOwned() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckMaxEnhancementsOwned(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsMaxOwned() < this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsMaxOwned() < this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckMinSpecificEnhancementsOwned(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsMaxOwned(this.Enhancement) >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsMaxOwned(this.Enhancement) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckMaxSpecificEnhancementsOwned(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsMaxOwned(this.Enhancement) < this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsMaxOwned(this.Enhancement) < this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckMinEnhancementsOwnedMaxLevel(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsOwnedMaxLevel() >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsOwnedMaxLevel() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckMaxEnhancementsOwnedMaxLevel(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsOwnedMaxLevel() < this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsOwnedMaxLevel() < this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckMinSpecificEnhancementsOwnedMaxLevel(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsOwnedMaxLevel(this.Enhancement)
                                     >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsOwnedMaxLevel(this.Enhancement) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckMaxSpecificEnhancementsOwnedMaxLevel(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsOwnedMaxLevel(this.Enhancement)
                                     < this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsOwnedMaxLevel(this.Enhancement) < this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        private bool CheckDashesPerformed(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetDashes() >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetDashes() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
