using System;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    [Serializable]
    public abstract class C_Predicate : C_Condition {
        [field: SerializeReference][field: HideInInspector] protected string Predicate { get; set; }
        [field: SerializeReference] protected E_Mode Mode { get; set; }

        protected C_Predicate(E_Mode mode) {
            this.Predicate = this.GetType().Name;
            this.Mode = mode;
        }
    }
}
