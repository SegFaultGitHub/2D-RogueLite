using System;
using System.Collections.Generic;
using System.Linq;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    [Serializable]
    public class C_And : C_Condition {
        [field: SerializeReference] public List<C_Condition> And { get; set; } = new();

        public override bool Check(MB_ObjectsManager objectsManager) => this.And.All(c => c.Check(objectsManager));
        public override bool ShouldSkip() => this.And.All(c => c.ShouldSkip());

        public override string GetVerbose(MB_ObjectsManager objectsManager, int indent) {
            List<string> lines = new();
            foreach (C_Condition condition in this.And) {
                if (condition.ShouldSkip()) continue;

                lines.Add(condition.GetVerbose(objectsManager, indent + 2));
            }

            return string.Join($"\n{new string(' ', indent + 2)}AND\n", lines);
        }
    }
}
