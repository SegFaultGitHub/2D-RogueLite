using System;
using System.Collections.Generic;
using System.Linq;
using Code.Enhancements.UnlockConditions.Editor_.Predicates;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    [Serializable]
    public class C_Or : C_Condition {
        [field: SerializeReference] public List<C_Condition> Or { get; set; } = new();

        public override bool Check(MB_ObjectsManager objectsManager) => this.Or.Any(c => c.Check(objectsManager));
        public override bool ShouldSkip() => this.Or.All(c => c.ShouldSkip());

        public override string GetVerbose(MB_ObjectsManager objectsManager, int indent) {
            List<string> lines = new();
            foreach (C_Condition condition in this.Or) {
                if (condition.ShouldSkip()) continue;

                lines.Add(condition.GetVerbose(objectsManager, indent + 2));
            }

            return string.Join($"\n{new string(' ', indent + 2)}AND\n", lines);
        }
    }
}
