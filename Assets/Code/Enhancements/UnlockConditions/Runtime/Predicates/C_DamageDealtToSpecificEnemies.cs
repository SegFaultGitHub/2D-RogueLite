using System;
using Code.Characters.Enemies;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_DamageDealtToSpecificEnemies : C_DamageDealt {
        [field: SerializeReference] public E_Enemy Enemy { get; set; }

        public C_DamageDealtToSpecificEnemies(E_Mode mode, int value, E_Enemy enemy) : base(mode, value) {
            this.Enemy = enemy;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetDamageDealt(this.Enemy) >= this.Value,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetDamageDealt(this.Enemy) >= this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
