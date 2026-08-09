using System;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_MinSpecificEnhancementsOwned : C_MinEnhancementsOwned {
        [field: SerializeReference] public E_Enhancement Enhancement { get; set; }

        public C_MinSpecificEnhancementsOwned(E_Mode mode, int count, E_Enhancement enhancement) : base(mode, count) {
            this.Enhancement = enhancement;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsMaxOwned(this.Enhancement) >= this.Count,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsMaxOwned(this.Enhancement) >= this.Count,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
