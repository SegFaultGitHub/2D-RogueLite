using System;
using Code.Managers;
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

        public override string GetVerbose(MB_ObjectsManager objectsManager, int indent) {
            string indentString = new(' ', indent);
            string current = SC_Utils.FormatNumber(objectsManager.StatsManager.GlobalStats.GetEnhancementsTaken(this.Enhancement)).Yellow();
            string total = $"{SC_Utils.FormatNumber(this.Count)}".Brown();
            string goal = $"{current} / {total}" //
                .NoBreak()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);

            return $"{indentString}- Take {this.Enhancement} {goal} times";
        }
    }
}
