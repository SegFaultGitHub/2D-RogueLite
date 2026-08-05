using System;
using System.Collections.Generic;
using System.Linq;
using Code.Characters;
using Code.Characters.Enemies;
using Code.Enhancements;
using Code.Serializer;
using MyBox;
using UnityEngine;

namespace Code.Managers {
    public class MB_StatsManager : MonoBehaviour {
        [Serializable]
        public class C_Stats {
            [Serializable]
            public class C_BestiaryData {
                [ReadOnly][SerializeField] private E_Enemy m_Enemy;
                [ReadOnly][SerializeField] private string m_EnemyVerbose;
                [ReadOnly][SerializeField] private int m_Killed;
                [ReadOnly][SerializeField] private int m_DamageDealt;
                [ReadOnly][SerializeField] private int m_KilledBy;
                [ReadOnly][SerializeField] private int m_DamageReceived;

                public E_Enemy Enemy { get => this.m_Enemy; set => this.m_Enemy = value; }
                private string EnemyVerbose { get => this.m_EnemyVerbose; set => this.m_EnemyVerbose = value; }
                public int Killed { get => this.m_Killed; set => this.m_Killed = value; }
                public int DamageDealt { get => this.m_DamageDealt; set => this.m_DamageDealt = value; }
                public int KilledBy { get => this.m_KilledBy; set => this.m_KilledBy = value; }
                public int DamageReceived { get => this.m_DamageReceived; set => this.m_DamageReceived = value; }

                public C_BestiaryData(E_Enemy enemy) {
                    this.Enemy = enemy;
                    this.EnemyVerbose = enemy.ToFriendlyString();
                    this.Killed = 0;
                    this.DamageDealt = 0;
                    this.KilledBy = 0;
                    this.DamageReceived = 0;
                }
            }

            [Serializable]
            public class C_EnhancementData {
                [ReadOnly][SerializeField] private E_Enhancement m_Enhancement;
                [ReadOnly][SerializeField] private string m_EnhancementVerbose;
                [ReadOnly][SerializeField] private int m_Taken;
                [ReadOnly][SerializeField] private int m_MaxLevelReached;
                [ReadOnly][SerializeField] private int m_MaxOwned;
                [ReadOnly][SerializeField] private int m_OwnedMaxLevel;

                public E_Enhancement Enhancement { get => this.m_Enhancement; set => this.m_Enhancement = value; }
                private string EnhancementVerbose { get => this.m_EnhancementVerbose; set => this.m_EnhancementVerbose = value; }
                public int Taken { get => this.m_Taken; set => this.m_Taken = value; }
                public int MaxLevelReached { get => this.m_MaxLevelReached; set => this.m_MaxLevelReached = value; }
                public int MaxOwned { get => this.m_MaxOwned; set => this.m_MaxOwned = value; }
                public int OwnedMaxLevel { get => this.m_OwnedMaxLevel; set => this.m_OwnedMaxLevel = value; }

                public C_EnhancementData(E_Enhancement enhancement) {
                    this.Enhancement = enhancement;
                    this.EnhancementVerbose = enhancement.ToFriendlyString();
                    this.Taken = 0;
                    this.MaxLevelReached = 0;
                    this.MaxOwned = 0;
                    this.OwnedMaxLevel = 0;
                }
            }

            [Serializable]
            public class C_DamageData {
                [ReadOnly][SerializeField] private E_DamageSource m_Source;
                [ReadOnly][SerializeField] private string m_SourceVerbose;
                [ReadOnly][SerializeField] private int m_Dealt;
                [ReadOnly][SerializeField] private int m_Received;
                [ReadOnly][SerializeField] private int m_MaxDealt;
                [ReadOnly][SerializeField] private int m_MaxReceived;

                public E_DamageSource Source { get => this.m_Source; set => this.m_Source = value; }
                private string SourceVerbose { get => this.m_SourceVerbose; set => this.m_SourceVerbose = value; }
                public int Dealt { get => this.m_Dealt; set => this.m_Dealt = value; }
                public int Received { get => this.m_Received; set => this.m_Received = value; }
                public int MaxDealt { get => this.m_MaxDealt; set => this.m_MaxDealt = value; }
                public int MaxReceived { get => this.m_MaxReceived; set => this.m_MaxReceived = value; }

                public C_DamageData(E_DamageSource source) {
                    this.Source = source;
                    this.SourceVerbose = source.ToFriendlyString();
                    this.Dealt = 0;
                    this.Received = 0;
                    this.MaxDealt = 0;
                    this.MaxReceived = 0;
                }
            }

            [Serializable]
            public class C_GeneralData {
                [ReadOnly][SerializeField] private int m_Dashes;

                public int Dashes { get => this.m_Dashes; set => this.m_Dashes = value; }

                public C_GeneralData() {
                    this.Dashes = 0;
                }
            }

            #region Members
            [ReadOnly][SerializeField] public List<C_BestiaryData> m_BestiaryData;
            [ReadOnly][SerializeField] public List<C_EnhancementData> m_EnhancementData;
            [ReadOnly][SerializeField] public List<C_DamageData> m_DamageData;
            [ReadOnly][SerializeField] public C_GeneralData m_GeneralData;
            #endregion

            #region Getters/Setters
            public List<C_BestiaryData> BestiaryData { get => this.m_BestiaryData; set => this.m_BestiaryData = value; }
            public List<C_EnhancementData> EnhancementData { get => this.m_EnhancementData; set => this.m_EnhancementData = value; }
            public List<C_DamageData> DamageData { get => this.m_DamageData; set => this.m_DamageData = value; }
            public C_GeneralData GeneralData { get => this.m_GeneralData; set => this.m_GeneralData = value; }
            #endregion

            public C_Stats() {
                this.BestiaryData = new List<C_BestiaryData>();
                this.EnhancementData = new List<C_EnhancementData>();
                this.DamageData = new List<C_DamageData>();
                this.GeneralData = new C_GeneralData();
            }

            #region Write
            public void AddKilled(AMB_Enemy enemy) {
                C_BestiaryData bestiaryData = this.BestiaryData.FirstOrDefault(bestiaryData => bestiaryData.Enemy == enemy.Enemy);
                if (bestiaryData == null) {
                    bestiaryData = new C_BestiaryData(enemy.Enemy);
                    this.BestiaryData.Add(bestiaryData);
                }

                bestiaryData.Killed++;
            }

            public void AddKilledBy(AMB_Enemy enemy) {
                C_BestiaryData bestiaryData = this.BestiaryData.FirstOrDefault(bestiaryData => bestiaryData.Enemy == enemy.Enemy);
                if (bestiaryData == null) {
                    bestiaryData = new C_BestiaryData(enemy.Enemy);
                    this.BestiaryData.Add(bestiaryData);
                }

                bestiaryData.KilledBy++;
            }

            public void AddDamageDealt(AMB_Enemy enemy, int value, E_DamageSource source) {
                C_BestiaryData bestiaryData = this.BestiaryData.FirstOrDefault(bestiaryData => bestiaryData.Enemy == enemy.Enemy);
                if (bestiaryData == null) {
                    bestiaryData = new C_BestiaryData(enemy.Enemy);
                    this.BestiaryData.Add(bestiaryData);
                }

                bestiaryData.DamageDealt += value;

                C_DamageData damageData = this.DamageData.FirstOrDefault(damageData => damageData.Source == source);
                if (damageData == null) {
                    damageData = new C_DamageData(source);
                    this.DamageData.Add(damageData);
                }

                damageData.Dealt += value;
                damageData.MaxDealt = Mathf.Max(damageData.MaxDealt, value);
            }

            public void AddDamageReceived(AMB_Enemy enemy, int value, E_DamageSource source) {
                if (enemy != null) {
                    C_BestiaryData bestiaryData = this.BestiaryData.FirstOrDefault(bestiaryData => bestiaryData.Enemy == enemy.Enemy);
                    if (bestiaryData == null) {
                        bestiaryData = new C_BestiaryData(enemy.Enemy);
                        this.BestiaryData.Add(bestiaryData);
                    }

                    bestiaryData.DamageReceived += value;
                }

                C_DamageData damageData = this.DamageData.FirstOrDefault(damageData => damageData.Source == source);
                if (damageData == null) {
                    damageData = new C_DamageData(source);
                    this.DamageData.Add(damageData);
                }

                damageData.Received += value;
                damageData.MaxReceived = Mathf.Max(damageData.MaxReceived, value);
            }

            public void AddEnhancementTaken(AMB_Enhancement enhancement, int level, int owned, bool reachedMaxLevel) {
                C_EnhancementData enhancementData =
                    this.EnhancementData.FirstOrDefault(enhancementData => enhancementData.Enhancement == enhancement.Enhancement);
                if (enhancementData == null) {
                    enhancementData = new C_EnhancementData(enhancement.Enhancement);
                    this.EnhancementData.Add(enhancementData);
                }

                enhancementData.Taken++;
                enhancementData.MaxLevelReached = Mathf.Max(enhancementData.MaxLevelReached, level);
                enhancementData.MaxOwned = Mathf.Max(enhancementData.MaxOwned, owned);
                if (reachedMaxLevel) enhancementData.OwnedMaxLevel++;
            }

            public void AddDash() {
                this.GeneralData.Dashes++;
            }
            #endregion

            #region Read
            public int GetKilled() => this.BestiaryData.Sum(bestiaryData => bestiaryData.Killed);

            public int GetKilled(E_Enemy enemy) {
                C_BestiaryData bestiaryData = this.BestiaryData.FirstOrDefault(bestiaryData => bestiaryData.Enemy == enemy);
                if (bestiaryData == null) {
                    bestiaryData = new C_BestiaryData(enemy);
                    this.BestiaryData.Add(bestiaryData);
                }

                return bestiaryData.Killed;
            }

            public int GetKilledBy() => this.BestiaryData.Sum(bestiaryData => bestiaryData.KilledBy);

            public int GetKilledBy(E_Enemy enemy) {
                C_BestiaryData bestiaryData = this.BestiaryData.FirstOrDefault(bestiaryData => bestiaryData.Enemy == enemy);
                if (bestiaryData == null) {
                    bestiaryData = new C_BestiaryData(enemy);
                    this.BestiaryData.Add(bestiaryData);
                }

                return bestiaryData.KilledBy;
            }

            public int GetDamageDealt() => this.DamageData.Sum(damageData => damageData.Dealt);

            public int GetDamageReceived() => this.DamageData.Sum(damageData => damageData.Received);

            public int GetDamageDealt(E_Enemy enemy) {
                C_BestiaryData bestiaryData = this.BestiaryData.FirstOrDefault(bestiaryData => bestiaryData.Enemy == enemy);
                if (bestiaryData == null) {
                    bestiaryData = new C_BestiaryData(enemy);
                    this.BestiaryData.Add(bestiaryData);
                }

                return bestiaryData.DamageDealt;
            }

            public int GetDamageReceived(E_Enemy enemy) {
                C_BestiaryData bestiaryData = this.BestiaryData.FirstOrDefault(bestiaryData => bestiaryData.Enemy == enemy);
                if (bestiaryData == null) {
                    bestiaryData = new C_BestiaryData(enemy);
                    this.BestiaryData.Add(bestiaryData);
                }

                return bestiaryData.DamageReceived;
            }

            public int GetDamageDealt(E_DamageSource source) {
                C_DamageData damageData = this.DamageData.FirstOrDefault(damageData => damageData.Source == source);
                if (damageData == null) {
                    damageData = new C_DamageData(source);
                    this.DamageData.Add(damageData);
                }

                return damageData.Dealt;
            }

            public int GetDamageReceived(E_DamageSource source) {
                C_DamageData damageData = this.DamageData.FirstOrDefault(damageData => damageData.Source == source);
                if (damageData == null) {
                    damageData = new C_DamageData(source);
                    this.DamageData.Add(damageData);
                }

                return damageData.Received;
            }

            public int GetEnhancementsTaken() => this.EnhancementData.Sum(e => e.Taken);

            public int GetEnhancementsTaken(E_Enhancement enhancement) {
                C_EnhancementData enhancementData =
                    this.EnhancementData.FirstOrDefault(enhancementData => enhancementData.Enhancement == enhancement);
                if (enhancementData == null) {
                    enhancementData = new C_EnhancementData(enhancement);
                    this.EnhancementData.Add(enhancementData);
                }

                return enhancementData.Taken;
            }

            public int GetEnhancementsMaxLevelReached() => this.EnhancementData.Max(enhancementData => enhancementData.MaxLevelReached);

            public int GetEnhancementsMaxLevelReached(E_Enhancement enhancement) {
                C_EnhancementData enhancementData =
                    this.EnhancementData.FirstOrDefault(enhancementData => enhancementData.Enhancement == enhancement);
                if (enhancementData == null) {
                    enhancementData = new C_EnhancementData(enhancement);
                    this.EnhancementData.Add(enhancementData);
                }

                return enhancementData.MaxLevelReached;
            }

            public int GetEnhancementsMaxOwned() => this.EnhancementData.Max(enhancementData => enhancementData.MaxOwned);

            public int GetEnhancementsMaxOwned(E_Enhancement enhancement) {
                C_EnhancementData enhancementData =
                    this.EnhancementData.FirstOrDefault(enhancementData => enhancementData.Enhancement == enhancement);
                if (enhancementData == null) {
                    enhancementData = new C_EnhancementData(enhancement);
                    this.EnhancementData.Add(enhancementData);
                }

                return enhancementData.MaxOwned;
            }

            public int GetEnhancementsOwnedMaxLevel() => this.EnhancementData.Max(enhancementData => enhancementData.OwnedMaxLevel);

            public int GetEnhancementsOwnedMaxLevel(E_Enhancement enhancement) {
                C_EnhancementData enhancementData =
                    this.EnhancementData.FirstOrDefault(enhancementData => enhancementData.Enhancement == enhancement);
                if (enhancementData == null) {
                    enhancementData = new C_EnhancementData(enhancement);
                    this.EnhancementData.Add(enhancementData);
                }

                return enhancementData.OwnedMaxLevel;
            }

            public int GetDashes() => this.GeneralData.Dashes;
            #endregion
        }

        #region Members
        [Foldout("MB_StatsManager", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;

        [ReadOnly][SerializeField] private protected C_Stats m_CurrentRunStats;
        [ReadOnly][SerializeField] private protected C_Stats m_GlobalStats;
        [ReadOnly][SerializeField] private protected bool m_SkipGlobalSave;
        #endregion

        #region Getters / Setters
        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }

        public C_Stats CurrentRunStats { get => this.m_CurrentRunStats; private set => this.m_CurrentRunStats = value; }
        public C_Stats GlobalStats { get => this.m_GlobalStats; private set => this.m_GlobalStats = value; }
        public bool SkipGlobalSave { get => this.m_SkipGlobalSave; set => this.m_SkipGlobalSave = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void Initialize() {
            this.CurrentRunStats = new C_Stats();
            this.GlobalStats = SC_Serializer.ReadGlobalStats();
        }

        public void PostInitialize() { }

        #region Bestiary data
        public void AddKilled(AMB_Enemy enemy) {
            this.CurrentRunStats.AddKilled(enemy);
            if (!this.SkipGlobalSave) {
                this.GlobalStats.AddKilled(enemy);
                SC_Serializer.WriteGlobalStats(this.GlobalStats);
                this.ObjectsManager.EnhancementsManager.CheckUnlocks();
            }
        }

        public void AddKilledBy(AMB_Enemy enemy) {
            this.CurrentRunStats.AddKilledBy(enemy);
            if (!this.SkipGlobalSave) {
                this.GlobalStats.AddKilledBy(enemy);
                SC_Serializer.WriteGlobalStats(this.GlobalStats);
                this.ObjectsManager.EnhancementsManager.CheckUnlocks();
            }
        }

        public void AddDamageDealt(AMB_Enemy enemy, int value, E_DamageSource source) {
            this.CurrentRunStats.AddDamageDealt(enemy, value, source);
            if (!this.SkipGlobalSave) {
                this.GlobalStats.AddDamageDealt(enemy, value, source);
                SC_Serializer.WriteGlobalStats(this.GlobalStats);
                this.ObjectsManager.EnhancementsManager.CheckUnlocks();
            }
        }

        public void AddDamageReceived(AMB_Enemy enemy, int value, E_DamageSource source) {
            this.CurrentRunStats.AddDamageReceived(enemy, value, source);
            if (!this.SkipGlobalSave) {
                this.GlobalStats.AddDamageReceived(enemy, value, source);
                SC_Serializer.WriteGlobalStats(this.GlobalStats);
                this.ObjectsManager.EnhancementsManager.CheckUnlocks();
            }
        }

        public void AddEnhancementTaken(AMB_Enhancement enhancement, int level, int owned, bool reachedMaxLevel) {
            this.CurrentRunStats.AddEnhancementTaken(enhancement, level, owned, reachedMaxLevel);
            if (!this.SkipGlobalSave) {
                this.GlobalStats.AddEnhancementTaken(enhancement, level, owned, reachedMaxLevel);
                SC_Serializer.WriteGlobalStats(this.GlobalStats);
                this.ObjectsManager.EnhancementsManager.CheckUnlocks();
            }
        }

        public void AddDash() {
            this.CurrentRunStats.AddDash();
            if (!this.SkipGlobalSave) {
                this.GlobalStats.AddDash();
                SC_Serializer.WriteGlobalStats(this.GlobalStats);
                this.ObjectsManager.EnhancementsManager.CheckUnlocks();
            }
        }
        #endregion
    }
}
