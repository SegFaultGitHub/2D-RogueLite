using System;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_MaxDamagePerSecondReached : C_Predicate {
        [field: SerializeReference] public float Damage { get; set; }

        public C_MaxDamagePerSecondReached(E_Mode mode, float damage) : base(mode) {
            this.Damage = damage;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetMaxDamagePerSecond() >= this.Damage,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetMaxDamagePerSecond() >= this.Damage,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
