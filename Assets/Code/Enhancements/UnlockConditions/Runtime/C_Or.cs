using System;
using System.Collections.Generic;
using System.Linq;
using Code.Enhancements.UnlockConditions.Editor_.Predicates;
using Code.Managers;
using Code.UI.EnhancementList;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    [Serializable]
    public class C_Or : C_Condition {
        [field: SerializeReference] public List<C_Condition> Or { get; set; } = new();

        public override bool Check(MB_ObjectsManager objectsManager) => this.Or.Any(c => c.Check(objectsManager));
        public override bool ShouldSkip() => this.Or.All(c => c.ShouldSkip());

        public override void GetVerbose(MB_ObjectsManager objectsManager, List<C_UnlockCondition> unlockConditions, int indent, bool b) {
            //List<string> lines = new();
            bool completed = this.Check(objectsManager);
            foreach (C_Condition condition in this.Or) {
                if (condition.ShouldSkip()) continue;

                condition.GetVerbose(objectsManager, unlockConditions, indent + 2, completed);
            }

            //return string.Join($"\n{new string(' ', indent + 2)}OR\n", lines);
        }
    }
}
