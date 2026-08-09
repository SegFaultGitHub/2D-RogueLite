using System;
using Code.Managers;

namespace Code.Enhancements.UnlockConditions.Runtime {
    [Serializable]
    public abstract class C_Condition {
        public abstract bool Check(MB_ObjectsManager objectManager);
    }
}
