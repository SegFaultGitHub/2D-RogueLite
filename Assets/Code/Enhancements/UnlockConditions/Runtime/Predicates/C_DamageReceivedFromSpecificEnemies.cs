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

        public override void GetVerbose(
            MB_ObjectsManager objectsManager,
            List<C_UnlockCondition> unlockConditions,
            int indent,
            bool completed = false
        ) {
            //string indentString = new(' ', indent);
            string current = SC_Utils.FormatNumber(objectsManager.StatsManager.GlobalStats.GetDamageReceived(this.Enemy));
            current = this.Check(objectsManager)
                ? current.Green()
                : current.Yellow();
            string total = $"{SC_Utils.FormatNumber(this.Damage)}".Brown();
            string goal = $"{current} / {total}" //
                .NoBreak()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);

            //return $"{indentString}- Receive {goal} damage from {this.Enemy}";

            unlockConditions.Add(
                new C_UnlockCondition {
                    Indent = indent,
                    Text = $"Receive {goal} damage from {this.Enemy}",
                    Unlocked = completed || this.Check(objectsManager)
                }
            );
        }
    }
}
