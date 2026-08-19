using System;
using Code.Managers;
using Code.UI.Text;
using Code.Utils;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_CriticalHitsDealt : C_Predicate {
        [field: SerializeReference] public int Count { get; set; }

        public C_CriticalHitsDealt(E_Mode mode, int count) : base(mode) {
            this.Count = count;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetCriticalHitsDealt() >= this.Count,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetCriticalHitsDealt() >= this.Count,
                _ => throw new ArgumentOutOfRangeException()
            };

        public override string GetVerbose(MB_ObjectsManager objectsManager, int indent) {
            string indentString = new(' ', indent);
            string current = SC_Utils.FormatNumber(objectsManager.StatsManager.GlobalStats.GetCriticalHitsDealt()).Yellow();
            string total = $"{SC_Utils.FormatNumber(this.Count)}".Brown();
            string goal = $"{current} / {total}" //
                .NoBreak()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);

            return $"{indentString}- Deal {goal} critical hits";
        }
    }
}
