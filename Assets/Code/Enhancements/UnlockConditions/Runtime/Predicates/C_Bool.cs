using System;
using System.Collections.Generic;
using Code.Managers;
using Code.UI.EnhancementList;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime.Predicates {
    [Serializable]
    public class C_Bool : C_Predicate {
        [field: SerializeReference] public bool Bool { get; set; }

        public C_Bool(E_Mode mode, bool b) : base(mode) {
            this.Bool = b;
        }

        public override bool Check(MB_ObjectsManager objectsManager) => this.Bool;
        public override bool ShouldSkip() => true;
    }
}
