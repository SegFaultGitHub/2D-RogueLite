using System;
using Code.Characters.Enemies;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_DamageReceivedFromSpecificEnemies : C_DamageDealt {
        [field: SerializeReference] public E_Enemy Enemy { get; set; }

        public C_DamageReceivedFromSpecificEnemies(E_Mode mode, float damage, E_Enemy enemy) : base(mode, damage) {
            this.Enemy = enemy;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetDamageReceived(this.Enemy) >= this.Damage,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetDamageReceived(this.Enemy) >= this.Damage,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
