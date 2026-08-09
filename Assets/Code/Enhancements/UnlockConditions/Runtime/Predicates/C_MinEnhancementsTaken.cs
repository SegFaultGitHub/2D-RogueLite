using System;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_MinEnhancementsTaken : C_Predicate {
        [field: SerializeReference] public int Count { get; set; }

        public C_MinEnhancementsTaken(E_Mode mode, int count) : base(mode) {
            this.Count = count;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsTaken() >= this.Count,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsTaken() >= this.Count,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
