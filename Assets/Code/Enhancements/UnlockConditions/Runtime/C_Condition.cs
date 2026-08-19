using System;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    [Serializable]
    public abstract class C_Condition {
        [field: SerializeReference][field: HideInInspector] protected string Name { get; set; }

        protected C_Condition() {
            this.Name = this.GetType().Name;
        }

        public abstract bool Check(MB_ObjectsManager objectsManager);

        public abstract bool ShouldSkip();

        public abstract string GetVerbose(MB_ObjectsManager objectsManager, int indent);
    }
}
