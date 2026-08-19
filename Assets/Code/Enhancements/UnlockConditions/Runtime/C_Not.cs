using System;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    [Serializable]
    public class C_Not : C_Condition {
        [field: SerializeReference] public C_Condition Not { get; set; }

        public override bool Check(MB_ObjectsManager objectsManager) => !this.Not.Check(objectsManager);
        public override bool ShouldSkip() => false;

        public override string GetVerbose(MB_ObjectsManager objectsManager, int indent) => null;
    }
}
