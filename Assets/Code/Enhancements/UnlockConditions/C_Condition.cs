using System;
using Code.Managers;
using MyBox;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions {
    [Serializable]
    public class C_Condition {
        [SerializeField] private E_Condition m_Condition;

        [ConditionalField(nameof(m_Condition), false, E_Condition.Predicate)]
        [SerializeField] private C_Predicate m_Predicate;
        [ConditionalField(nameof(m_Condition), false, E_Condition.Or)]
        [SerializeField] private C_Or m_Or;
        [ConditionalField(nameof(m_Condition), false, E_Condition.And)]
        [SerializeField] private C_And m_And;

        private E_Condition Condition { get => this.m_Condition; }

        private C_Predicate Predicate { get => this.Condition == E_Condition.Predicate ? this.m_Predicate : null; }
        private C_Or Or { get => this.Condition == E_Condition.Or ? this.m_Or : null; }
        private C_And And { get => this.Condition == E_Condition.And ? this.m_And : null; }

        public bool Check(MB_ObjectsManager objectManager) {
            return this.Condition switch {
                E_Condition.Predicate => this.Predicate.Check(objectManager),
                E_Condition.And => this.And.Check(objectManager),
                E_Condition.Or => this.Or.Check(objectManager),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
