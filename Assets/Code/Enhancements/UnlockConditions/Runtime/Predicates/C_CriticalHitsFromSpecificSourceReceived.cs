using System;
using Code.Characters;
using Code.Managers;
using Code.UI.Text;
using Code.Utils;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_CriticalHitsFromSpecificSourceReceived : C_CriticalHitsReceived {
        [field: SerializeReference] public E_DamageSource Source { get; set; }

        public C_CriticalHitsFromSpecificSourceReceived(E_Mode mode, int count, E_DamageSource source) : base(mode, count) {
            this.Source = source;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetCriticalHitsReceived(this.Source) >= this.Count,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetCriticalHitsReceived(this.Source) >= this.Count,
                _ => throw new ArgumentOutOfRangeException()
            };

        public override string GetVerbose(MB_ObjectsManager objectsManager, int indent) {
            string indentString = new(' ', indent);
            string current = SC_Utils.FormatNumber(objectsManager.StatsManager.GlobalStats.GetCriticalHitsReceived(this.Source)).Yellow();
            string total = $"{SC_Utils.FormatNumber(this.Count)}".Brown();
            string goal = $"{current} / {total}" //
                .NoBreak()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);

            return $"{indentString}- Receive {goal} {this.Source} critical hits";
        }
    }
}
