using System;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_DamageReceived : C_Predicate {
        [field: SerializeReference] public int Value { get; set; }

        public C_DamageReceived(E_Mode mode, int value) : base(mode) {
            this.Value = value;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetDamageReceived() >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetDamageReceived() >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
