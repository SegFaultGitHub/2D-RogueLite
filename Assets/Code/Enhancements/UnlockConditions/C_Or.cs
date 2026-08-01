using System;
using System.Linq;
using Code.Managers;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions {
    [Serializable]
    public class C_Or {
        [SerializeField] private protected C_Condition[] m_Conditions;

        private C_Condition[] Conditions { get => this.m_Conditions; }

        public bool Check(MB_ObjectsManager objectManager) {
            return this.Conditions.Any(condition => condition.Check(objectManager));
        }
    }
}
