using System;
using System.Collections.Generic;
using Code.Managers;
using Code.UI.EnhancementList;
using Code.UI.Text;
using Code.Utils;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_MinSpecificEnhancementsTaken : C_MinEnhancementsTaken {
        [field: SerializeReference] public E_Enhancement Enhancement { get; set; }

        public C_MinSpecificEnhancementsTaken(E_Mode mode, int count, E_Enhancement enhancement) : base(mode, count) {
            this.Enhancement = enhancement;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetEnhancementsTaken(this.Enhancement) >= this.Count,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetEnhancementsTaken(this.Enhancement) >= this.Count,
                _ => throw new ArgumentOutOfRangeException()
            };

        public override void GetVerbose(
            MB_ObjectsManager objectsManager,
            List<C_UnlockCondition> unlockConditions,
            int indent,
            bool completed = false
        ) {
            //string indentString = new(' ', indent);
            string current = SC_Utils.FormatNumber(objectsManager.StatsManager.GlobalStats.GetEnhancementsTaken(this.Enhancement));
            current = this.Check(objectsManager)
                ? current.Green()
                : current.Yellow();
            string total = $"{SC_Utils.FormatNumber(this.Count)}".Brown();
            string goal = $"{current} / {total}" //
                .NoBreak()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);
            string enhancementName = objectsManager.EnhancementsManager.GetEnhancementName(this.Enhancement) //
                .NoBreak()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false).Brown();

            //return $"{indentString}- Take {this.Enhancement} {goal} times";

            unlockConditions.Add(
                new C_UnlockCondition {
                    Indent = indent,
                    Text = $"Take {enhancementName} {goal} times",
                    Unlocked = completed || this.Check(objectsManager)
                }
            );
        }
    }
}
