using System;
using System.Collections.Generic;
using Code.Managers;
using Code.UI.EnhancementList;
using Code.UI.Text;
using Code.Utils;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_CriticalHitsReceived : C_Predicate {
        [field: SerializeReference] public int Count { get; set; }

        public C_CriticalHitsReceived(E_Mode mode, int count) : base(mode) {
            this.Count = count;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetCriticalHitsReceived() >= this.Count,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetCriticalHitsReceived() >= this.Count,
                _ => throw new ArgumentOutOfRangeException()
            };

        public override void GetVerbose(
            MB_ObjectsManager objectsManager,
            List<C_UnlockCondition> unlockConditions,
            int indent,
            bool completed = false
        ) {
            //string indentString = new(' ', indent);
            string current = SC_Utils.FormatNumber(objectsManager.StatsManager.GlobalStats.GetCriticalHitsReceived());
            current = this.Check(objectsManager)
                ? current.Green()
                : current.Yellow();
            string total = $"{SC_Utils.FormatNumber(this.Count)}".Brown();
            string goal = $"{current} / {total}" //
                .NoBreak()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);

            //return $"{indentString}- Receive {goal} critical hits";

            unlockConditions.Add(
                new C_UnlockCondition {
                    Indent = indent,
                    Text = $"Receive {goal} critical hits",
                    Unlocked = completed || this.Check(objectsManager)
                }
            );
        }
    }
}
