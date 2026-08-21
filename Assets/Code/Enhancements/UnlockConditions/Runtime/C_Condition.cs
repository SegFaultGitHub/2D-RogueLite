using System;
using System.Collections.Generic;
using Code.Managers;
using Code.UI.EnhancementList;
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

        public virtual void GetVerbose(
            MB_ObjectsManager objectsManager,
            List<C_UnlockCondition> unlockConditions,
            int indent,
            bool completed = false
        ) { }
    }
}
