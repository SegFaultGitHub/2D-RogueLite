using System;
using System.Collections.Generic;
using Code.Managers;
using Code.UI.EnhancementList;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    public class C_UnlockConditionsRuntimeGraph : ScriptableObject {
        [SerializeReference] public C_Root m_Root;

        public C_Root Root { get => this.m_Root; set => this.m_Root = value; }

        public bool Check(MB_ObjectsManager objectsManager) => this.Root.Condition.Check(objectsManager);
        public bool CheckGlobal(MB_ObjectsManager objectsManager) => this.Root.GlobalCondition.Check(objectsManager);

        public void GetVerbose(MB_ObjectsManager objectsManager, List<C_UnlockCondition> unlockConditions) {
            int initialIndent = this.Root.GlobalCondition switch {
                C_And => -2,
                C_Or => -2,
                _ => 0
            };
            this.Root.GlobalCondition.GetVerbose(objectsManager, unlockConditions, initialIndent);
        }
    }
}
