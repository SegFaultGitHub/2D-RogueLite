using System;
using Code.Characters;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_CriticalHitsFromSpecificSourceDealt : C_CriticalHitsDealt {
        [field: SerializeReference] public E_DamageSource Source { get; set; }

        public C_CriticalHitsFromSpecificSourceDealt(E_Mode mode, int count, E_DamageSource source) : base(mode, count) {
            this.Source = source;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetCriticalHitsDealt(this.Source) >= this.Count,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetCriticalHitsDealt(this.Source) >= this.Count,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
