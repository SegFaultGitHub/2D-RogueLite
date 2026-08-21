using System;
using System.Collections.Generic;
using System.Linq;
using Code.Managers;
using Code.UI.EnhancementList;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    [Serializable]
    public class C_And : C_Condition {
        [field: SerializeReference] public List<C_Condition> And { get; set; } = new();

        public override bool Check(MB_ObjectsManager objectsManager) => this.And.All(c => c.Check(objectsManager));
        public override bool ShouldSkip() => this.And.All(c => c.ShouldSkip());

        public override void GetVerbose(
            MB_ObjectsManager objectsManager,
            List<C_UnlockCondition> unlockConditions,
            int indent,
            bool completed = false
        ) {
            //List<string> lines = new();
            foreach (C_Condition condition in this.And) {
                if (condition.ShouldSkip()) continue;

                condition.GetVerbose(objectsManager, unlockConditions, indent + 2);
            }

            //return string.Join($"\n{new string(' ', indent + 2)}AND\n", lines);
        }
    }
}
