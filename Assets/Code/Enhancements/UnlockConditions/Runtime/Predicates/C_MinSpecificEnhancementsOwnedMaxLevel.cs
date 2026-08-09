using System;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_MinSpecificEnhancementsOwnedMaxLevel : C_MinEnhancementsOwned {
        [field: SerializeReference] public E_Enhancement Enhancement { get; set; }

        public C_MinSpecificEnhancementsOwnedMaxLevel(E_Mode mode, int count, E_Enhancement enhancement) : base(mode, count) {
            this.Enhancement = enhancement;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsOwnedMaxLevel(this.Enhancement) >= this.Count,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsOwnedMaxLevel(this.Enhancement) >= this.Count,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
