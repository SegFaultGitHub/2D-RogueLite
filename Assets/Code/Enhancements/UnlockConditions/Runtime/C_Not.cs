using System;
using System.Collections.Generic;
using Code.Managers;
using Code.UI.EnhancementList;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    [Serializable]
    public class C_Not : C_Condition {
        [field: SerializeReference] public C_Condition Not { get; set; }

        public override bool Check(MB_ObjectsManager objectsManager) => !this.Not.Check(objectsManager);
        public override bool ShouldSkip() => false;

        public override void GetVerbose(
            MB_ObjectsManager objectsManager,
            List<C_UnlockCondition> unlockConditions,
            int indent,
            bool completed = false
        ) { }
    }
}
