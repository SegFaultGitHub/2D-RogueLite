using System;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_EnemiesKilled : C_Predicate {
        [field: SerializeReference] public int Count { get; set; }

        public C_EnemiesKilled(E_Mode mode, int count) : base(mode) {
            this.Count = count;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetKilled() >= this.Count,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetKilled() >= this.Count,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
