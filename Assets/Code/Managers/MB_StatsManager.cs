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
                [ReadOnly][SerializeField] private string m_EnemyVerbose;
                [ReadOnly][SerializeField] private int m_Killed;
                [ReadOnly][SerializeField] private float m_DamageDealt;
                [ReadOnly][SerializeField] private int m_KilledBy;
                [ReadOnly][SerializeField] private float m_DamageReceived;

                private string EnemyVerbose { get => this.m_EnemyVerbose; set => this.m_EnemyVerbose = value; }
                public int Killed { get => this.m_Killed; set => this.m_Killed = value; }
                public float DamageDealt { get => this.m_DamageDealt; set => this.m_DamageDealt = value; }
                public int KilledBy { get => this.m_KilledBy; set => this.m_KilledBy = value; }
                public float DamageReceived { get => this.m_DamageReceived; set => this.m_DamageReceived = value; }

                public C_BestiaryData(E_Enemy enemy) {
                    this.EnemyVerbose = enemy.ToFriendlyString();
                    this.Killed = 0;
                    this.DamageDealt = 0;
                    this.KilledBy = 0;
                    this.DamageReceived = 0;
                }
            }

            [Serializable]
            public class C_EnhancementData {
                [ReadOnly][SerializeField] private string m_EnhancementVerbose;
                [ReadOnly][SerializeField] private int m_Taken;
                [ReadOnly][SerializeField] private int m_MaxLevelReached;
                [ReadOnly][SerializeField] private int m_MaxOwned;
                [ReadOnly][SerializeField] private int m_OwnedMaxLevel;

                private string EnhancementVerbose { get => this.m_EnhancementVerbose; set => this.m_EnhancementVerbose = value; }
                public int Taken { get => this.m_Taken; set => this.m_Taken = value; }
                public int MaxLevelReached { get => this.m_MaxLevelReached; set => this.m_MaxLevelReached = value; }
                public int MaxOwned { get => this.m_MaxOwned; set => this.m_MaxOwned = value; }
                public int OwnedMaxLevel { get => this.m_OwnedMaxLevel; set => this.m_OwnedMaxLevel = value; }

                public C_EnhancementData(E_Enhancement enhancement) {
                    this.EnhancementVerbose = enhancement.ToFriendlyString();
                    this.Taken = 0;
                    this.MaxLevelReached = 0;
                    this.MaxOwned = 0;
                    this.OwnedMaxLevel = 0;
                }
            }

            [Serializable]
            public class C_DamageData {
                [ReadOnly][SerializeField] private string m_SourceVerbose;
                [ReadOnly][SerializeField] private float m_Dealt;
                [ReadOnly][SerializeField] private float m_Received;
                [ReadOnly][SerializeField] private float m_MaxDealt;
                [ReadOnly][SerializeField] private float m_MaxReceived;
                [ReadOnly][SerializeField] private float m_MaxDamagePerSecond;


                private string SourceVerbose { get => this.m_SourceVerbose; set => this.m_SourceVerbose = value; }
                public float Dealt { get => this.m_Dealt; set => this.m_Dealt = value; }
                public float Received { get => this.m_Received; set => this.m_Received = value; }
                public float MaxDealt { get => this.m_MaxDealt; set => this.m_MaxDealt = value; }
                public float MaxReceived { get => this.m_MaxReceived; set => this.m_MaxReceived = value; }
                public float MaxDamagePerSecond { get => this.m_MaxDamagePerSecond; set => this.m_MaxDamagePerSecond = value; }

                public C_DamageData(E_DamageSource source) {
                    this.SourceVerbose = source.ToFriendlyString();
                    this.Dealt = 0;
                    this.Received = 0;
                    this.MaxDealt = 0;
                    this.MaxReceived = 0;
                    this.MaxDamagePerSecond = 0;
                }
            }

            [Serializable]
            public class C_GeneralData {
                [ReadOnly][SerializeField] private int m_Dashes;
                [ReadOnly][SerializeField] private float m_MaxDamagePerSecond;
                [ReadOnly][SerializeField] private Dictionary<E_DamageSource, int> m_CriticalHitsDealt;
                [ReadOnly][SerializeField] private Dictionary<E_DamageSource, int> m_CriticalHitsReceived;

                public int Dashes { get => this.m_Dashes; set => this.m_Dashes = value; }
                public float MaxDamagePerSecond { get => this.m_MaxDamagePerSecond; set => this.m_MaxDamagePerSecond = value; }
                public Dictionary<E_DamageSource, int> CriticalHitsDealt { get => this.m_CriticalHitsDealt; set => this.m_CriticalHitsDealt = value; }
                public Dictionary<E_DamageSource, int> CriticalHitsReceived { get => this.m_CriticalHitsReceived; set => this.m_CriticalHitsReceived = value; }

                public C_GeneralData() {
                    this.Dashes = 0;
                    this.MaxDamagePerSecond = 0;
                    this.CriticalHitsDealt = new Dictionary<E_DamageSource, int>();
                    this.CriticalHitsReceived = new Dictionary<E_DamageSource, int>();
                }
            }

            #region Members
            [ReadOnly][SerializeField] public Dictionary<E_Enemy, C_BestiaryData> m_BestiaryData;
            [ReadOnly][SerializeField] public Dictionary<E_Enhancement, C_EnhancementData> m_EnhancementData;
            [ReadOnly][SerializeField] public Dictionary<E_DamageSource, C_DamageData> m_DamageData;
            [ReadOnly][SerializeField] public C_GeneralData m_GeneralData;
            #endregion

            #region Getters/Setters
            public Dictionary<E_Enemy, C_BestiaryData> BestiaryData { get => this.m_BestiaryData; set => this.m_BestiaryData = value; }
            public Dictionary<E_Enhancement, C_EnhancementData> EnhancementData { get => this.m_EnhancementData; set => this.m_EnhancementData = value; }
            public Dictionary<E_DamageSource, C_DamageData> DamageData { get => this.m_DamageData; set => this.m_DamageData = value; }
            public C_GeneralData GeneralData { get => this.m_GeneralData; set => this.m_GeneralData = value; }
            #endregion

            public C_Stats() {
                this.BestiaryData = new Dictionary<E_Enemy, C_BestiaryData>();
                this.EnhancementData = new Dictionary<E_Enhancement, C_EnhancementData>();
                this.DamageData = new Dictionary<E_DamageSource, C_DamageData>();
                this.GeneralData = new C_GeneralData();
            }

            #region Write
            public void AddKilled(AMB_Enemy enemy) {
                this.BestiaryData.TryAdd(enemy.Enemy, new C_BestiaryData(enemy.Enemy));
                C_BestiaryData bestiaryData = this.BestiaryData[enemy.Enemy];

                bestiaryData.Killed++;
            }

            public void AddKilledBy(AMB_Enemy enemy) {
                this.BestiaryData.TryAdd(enemy.Enemy, new C_BestiaryData(enemy.Enemy));
                C_BestiaryData bestiaryData = this.BestiaryData[enemy.Enemy];

                bestiaryData.KilledBy++;
            }

            public void AddDamageDealt(AMB_Enemy enemy, float value, E_DamageSource source, bool critical) {
                this.BestiaryData.TryAdd(enemy.Enemy, new C_BestiaryData(enemy.Enemy));
                C_BestiaryData bestiaryData = this.BestiaryData[enemy.Enemy];

                bestiaryData.DamageDealt += value;

                this.DamageData.TryAdd(source, new C_DamageData(source));
                C_DamageData damageData = this.DamageData[source];

                damageData.Dealt += value;
                damageData.MaxDealt = Mathf.Max(damageData.MaxDealt, value);

                if (critical) {
                    this.GeneralData.CriticalHitsDealt.TryAdd(source, 0);
                    this.GeneralData.CriticalHitsDealt[source]++;
                }
            }

            public void AddDamageReceived(AMB_Enemy enemy, float value, E_DamageSource source, bool critical) {
                if (enemy != null) {
                    this.BestiaryData.TryAdd(enemy.Enemy, new C_BestiaryData(enemy.Enemy));
                    C_BestiaryData bestiaryData = this.BestiaryData[enemy.Enemy];

                    bestiaryData.DamageReceived += value;
                }

                this.DamageData.TryAdd(source, new C_DamageData(source));
                C_DamageData damageData = this.DamageData[source];

                damageData.Received += value;
                damageData.MaxReceived = Mathf.Max(damageData.MaxReceived, value);

                if (critical) {
                    this.GeneralData.CriticalHitsReceived.TryAdd(source, 0);
                    this.GeneralData.CriticalHitsReceived[source]++;
                }
            }

            public void SetMaxDamagePerSecondReached(float damagePerSecond, float damagePerSecondFromSource, E_DamageSource source) {
                this.DamageData.TryAdd(source, new C_DamageData(source));
                C_DamageData damageData = this.DamageData[source];

                damageData.MaxDamagePerSecond = Mathf.Max(damageData.MaxDamagePerSecond, damagePerSecondFromSource);

                this.GeneralData.MaxDamagePerSecond = Mathf.Max(this.GeneralData.MaxDamagePerSecond, damagePerSecond);
            }

            public void AddEnhancementTaken(AMB_Enhancement enhancement, int level, int owned, bool reachedMaxLevel) {
                this.EnhancementData.TryAdd(enhancement.Enhancement, new C_EnhancementData(enhancement.Enhancement));
                C_EnhancementData enhancementData = this.EnhancementData[enhancement.Enhancement];

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
            public int GetKilled() => this.BestiaryData.Values.Sum(bestiaryData => bestiaryData.Killed);

            public int GetKilled(E_Enemy enemy) {
                this.BestiaryData.TryAdd(enemy, new C_BestiaryData(enemy));
                C_BestiaryData bestiaryData = this.BestiaryData[enemy];

                return bestiaryData.Killed;
            }

            public int GetKilledBy() => this.BestiaryData.Values.Sum(bestiaryData => bestiaryData.KilledBy);

            public int GetKilledBy(E_Enemy enemy) {
                this.BestiaryData.TryAdd(enemy, new C_BestiaryData(enemy));
                C_BestiaryData bestiaryData = this.BestiaryData[enemy];

                return bestiaryData.KilledBy;
            }

            public float GetDamageDealt() => this.DamageData.Values.Sum(damageData => damageData.Dealt);

            public float GetDamageReceived() => this.DamageData.Values.Sum(damageData => damageData.Received);

            public float GetDamageDealt(E_Enemy enemy) {
                this.BestiaryData.TryAdd(enemy, new C_BestiaryData(enemy));
                C_BestiaryData bestiaryData = this.BestiaryData[enemy];

                return bestiaryData.DamageDealt;
            }

            public float GetDamageReceived(E_Enemy enemy) {
                this.BestiaryData.TryAdd(enemy, new C_BestiaryData(enemy));
                C_BestiaryData bestiaryData = this.BestiaryData[enemy];

                return bestiaryData.DamageReceived;
            }

            public float GetDamageDealt(E_DamageSource source) {
                this.DamageData.TryAdd(source, new C_DamageData(source));
                C_DamageData damageData = this.DamageData[source];

                return damageData.Dealt;
            }

            public float GetDamageReceived(E_DamageSource source) {
                this.DamageData.TryAdd(source, new C_DamageData(source));
                C_DamageData damageData = this.DamageData[source];

                return damageData.Received;
            }

            public int GetEnhancementsTaken() => this.EnhancementData.Values.Sum(e => e.Taken);

            public int GetEnhancementsTaken(E_Enhancement enhancement) {
                this.EnhancementData.TryAdd(enhancement, new C_EnhancementData(enhancement));
                C_EnhancementData enhancementData = this.EnhancementData[enhancement];

                return enhancementData.Taken;
            }

            public int GetEnhancementsMaxLevelReached() => this.EnhancementData.Values.Max(enhancementData => enhancementData.MaxLevelReached);

            public int GetEnhancementsMaxLevelReached(E_Enhancement enhancement) {
                this.EnhancementData.TryAdd(enhancement, new C_EnhancementData(enhancement));
                C_EnhancementData enhancementData = this.EnhancementData[enhancement];

                return enhancementData.MaxLevelReached;
            }

            public int GetEnhancementsMaxOwned() => this.EnhancementData.Values.Max(enhancementData => enhancementData.MaxOwned);

            public int GetEnhancementsMaxOwned(E_Enhancement enhancement) {
                this.EnhancementData.TryAdd(enhancement, new C_EnhancementData(enhancement));
                C_EnhancementData enhancementData = this.EnhancementData[enhancement];

                return enhancementData.MaxOwned;
            }

            public int GetEnhancementsOwnedMaxLevel() => this.EnhancementData.Values.Max(enhancementData => enhancementData.OwnedMaxLevel);

            public int GetEnhancementsOwnedMaxLevel(E_Enhancement enhancement) {
                this.EnhancementData.TryAdd(enhancement, new C_EnhancementData(enhancement));
                C_EnhancementData enhancementData = this.EnhancementData[enhancement];

                return enhancementData.OwnedMaxLevel;
            }

            public int GetDashes() => this.GeneralData.Dashes;

            public float GetMaxDamagePerSecond() => this.GeneralData.MaxDamagePerSecond;

            public float GetMaxDamagePerSecond(E_DamageSource source) {
                this.DamageData.TryAdd(source, new C_DamageData(source));
                C_DamageData damageData = this.DamageData[source];

                return damageData.MaxDamagePerSecond;
            }

            public int GetCriticalHitsDealt() => this.GeneralData.CriticalHitsDealt.Values.Sum();
            public int GetCriticalHitsDealt(E_DamageSource source) {
                this.GeneralData.CriticalHitsDealt.TryAdd(source, 0);

                return this.GeneralData.CriticalHitsDealt[source];
            }

            public int GetCriticalHitsReceived() => this.GeneralData.CriticalHitsReceived.Values.Sum();
            public int GetCriticalHitsReceived(E_DamageSource source) {
                this.GeneralData.CriticalHitsReceived.TryAdd(source, 0);

                return this.GeneralData.CriticalHitsReceived[source];
            }
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

        public void AddDamageDealt(AMB_Enemy enemy, float value, E_DamageSource source, bool critical) {
            // if (enemy?.Enemy == E_Enemy.Dummy) return;

            this.CurrentRunStats.AddDamageDealt(enemy, value, source, critical);
            if (!this.SkipGlobalSave) {
                this.GlobalStats.AddDamageDealt(enemy, value, source, critical);
                SC_Serializer.WriteGlobalStats(this.GlobalStats);
                this.ObjectsManager.EnhancementsManager.CheckUnlocks();
            }
        }

        public void AddDamageReceived(AMB_Enemy enemy, float value, E_DamageSource source, bool critical) {
            // if (enemy?.Enemy == E_Enemy.Dummy) return;

            this.CurrentRunStats.AddDamageReceived(enemy, value, source, critical);
            if (!this.SkipGlobalSave) {
                this.GlobalStats.AddDamageReceived(enemy, value, source, critical);
                SC_Serializer.WriteGlobalStats(this.GlobalStats);
                this.ObjectsManager.EnhancementsManager.CheckUnlocks();
            }
        }

        public void SetMaxDamagePerSecondReached(float damagePerSecond, float damagePerSecondFromSource, E_DamageSource source) {
            this.CurrentRunStats.SetMaxDamagePerSecondReached(damagePerSecond, damagePerSecondFromSource, source);
            if (!this.SkipGlobalSave) {
                this.GlobalStats.SetMaxDamagePerSecondReached(damagePerSecond, damagePerSecondFromSource, source);
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

        [ButtonMethod]
        public void Reset() {
            this.GlobalStats = new C_Stats();
            SC_Serializer.WriteGlobalStats(this.GlobalStats);
        }
    }
}
