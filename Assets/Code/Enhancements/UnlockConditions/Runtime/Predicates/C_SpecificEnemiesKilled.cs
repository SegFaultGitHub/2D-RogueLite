using System;
using System.Collections.Generic;
using Code.Characters.Enemies;
using Code.Managers;
using Code.UI.EnhancementList;
using Code.UI.Text;
using Code.Utils;
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

        public override void GetVerbose(
            MB_ObjectsManager objectsManager,
            List<C_UnlockCondition> unlockConditions,
            int indent,
            bool completed = false
        ) {
            //string indentString = new(' ', indent);
            string current = SC_Utils.FormatNumber(objectsManager.StatsManager.GlobalStats.GetKilled(this.Enemy));
            current = this.Check(objectsManager)
                ? current.Green()
                : current.Yellow();
            string total = $"{SC_Utils.FormatNumber(this.Count)}".Brown();
            string goal = $"{current} / {total}" //
                .NoBreak()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);

            //return $"{indentString}- Kill {goal} {this.Enemy}";

            unlockConditions.Add(
                new C_UnlockCondition {
                    Indent = indent,
                    Text = $"Kill {goal} {this.Enemy}",
                    Unlocked = completed || this.Check(objectsManager)
                }
            );
        }
    }
}
