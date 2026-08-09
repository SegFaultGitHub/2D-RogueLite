using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    public class C_UnlockConditionsRuntimeGraph : ScriptableObject {
        [SerializeReference] public C_Root m_Root;

        public C_Root Root { get => this.m_Root; set => this.m_Root = value; }

        public bool Check(MB_ObjectsManager objectsManager) => this.Root.Condition.Check(objectsManager);
        public bool CheckGlobal(MB_ObjectsManager objectsManager) => this.Root.GlobalCondition.Check(objectsManager);
    }
}
