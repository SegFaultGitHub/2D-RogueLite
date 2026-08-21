using System;
using System.Collections.Generic;
using Code.Characters;
using Code.Managers;
using Code.UI.EnhancementList;
using Code.UI.Text;
using Code.Utils;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_MaxDamageDealtFromSpecificSource : C_MaxDamageDealt {
        [field: SerializeReference] public E_DamageSource Source { get; set; }

        public C_MaxDamageDealtFromSpecificSource(E_Mode mode, float damage, E_DamageSource source) : base(mode, damage) {
            this.Source = source;
        }

        public override bool Check(MB_ObjectsManager objectsManager) =>
            this.Mode switch {
                E_Mode.CurrentRun => objectsManager.StatsManager.CurrentRunStats.GetMaxDamageDealt(this.Source) >= this.Damage,
                E_Mode.Global => objectsManager.StatsManager.GlobalStats.GetMaxDamageDealt(this.Source) >= this.Damage,
                _ => throw new ArgumentOutOfRangeException()
            };

        public override void GetVerbose(
            MB_ObjectsManager objectsManager,
            List<C_UnlockCondition> unlockConditions,
            int indent,
            bool completed = false
        ) {
            //string indentString = new(' ', indent);
            string current = SC_Utils.FormatNumber(objectsManager.StatsManager.GlobalStats.GetMaxDamageDealt(this.Source));
            current = this.Check(objectsManager)
                ? current.Green()
                : current.Yellow();
            string total = $"{SC_Utils.FormatNumber(this.Damage)}".Brown();
            string goal = $"{current} / {total}" //
                .NoBreak()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);

            //return $"{indentString}- Deal {goal} {this.Source} damage at once";

            unlockConditions.Add(
                new C_UnlockCondition {
                    Indent = indent,
                    Text = $"Deal {goal} {this.Source} damage at once",
                    Unlocked = completed || this.Check(objectsManager)
                }
            );
        }
    }
}
