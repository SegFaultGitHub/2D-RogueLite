using System;
using System.Collections.Generic;
using System.Linq;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    [Serializable]
    public class C_And : C_Condition {
        [field: SerializeReference] public List<C_Condition> And { get; set; } = new();

        public override bool Check(MB_ObjectsManager objectManager) => this.And.All(c => c.Check(objectManager));
    }
}
