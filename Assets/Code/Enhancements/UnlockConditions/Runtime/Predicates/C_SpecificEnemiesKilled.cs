using System;
using Code.Characters.Enemies;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_SpecificEnemiesKilled : C_EnemiesKilled {
        [field: SerializeReference] public E_Enemy Enemy { get; set; }

        public C_SpecificEnemiesKilled(E_Mode mode, int count, E_Enemy enemy) : base(mode, count) {
            this.Enemy = enemy;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetKilled(this.Enemy) >= this.Count,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetKilled(this.Enemy) >= this.Count,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
