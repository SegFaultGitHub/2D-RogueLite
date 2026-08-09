using System;
using System.Collections.Generic;
using System.Linq;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    [Serializable]
    public class C_Or : C_Condition {
        [field: SerializeReference] public List<C_Condition> Or { get; set; } = new();

        public override bool Check(MB_ObjectsManager objectManager) => this.Or.Any(c => c.Check(objectManager));
    }
}
