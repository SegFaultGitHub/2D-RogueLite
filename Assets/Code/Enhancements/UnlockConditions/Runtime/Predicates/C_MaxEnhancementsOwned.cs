using System;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_MaxEnhancementsOwned : C_Predicate {
        [field: SerializeReference] public int Count { get; set; }

        public C_MaxEnhancementsOwned(E_Mode mode, int count) : base(mode) {
            this.Count = count;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsMaxOwned() < this.Count,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsMaxOwned() < this.Count,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
