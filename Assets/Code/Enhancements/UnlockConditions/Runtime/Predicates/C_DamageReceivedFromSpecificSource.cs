using System;
using Code.Characters;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_DamageReceivedFromSpecificSource : C_DamageDealt {
        [field: SerializeReference] public E_DamageSource Source { get; set; }

        public C_DamageReceivedFromSpecificSource(E_Mode mode, int value, E_DamageSource source) : base(mode, value) {
            this.Source = source;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetDamageReceived(this.Source) >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetDamageReceived(this.Source) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
