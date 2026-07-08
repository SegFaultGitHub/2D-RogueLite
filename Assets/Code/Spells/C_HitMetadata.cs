using System;
using MyBox;
using UnityEngine;

namespace Code.Spells {
    [Serializable]
    public class C_HitMetadata {
        #region Members
        [Separator("C_HitMetadata")]
        [SerializeField] private protected float m_Damage;
        [SerializeField] private protected float m_KnockbackForce;
        #endregion

        #region Getters / Setters
        public float Damage { get => this.m_Damage; set => this.m_Damage = value; }
        public float KnockbackForce { get => this.m_KnockbackForce; set => this.m_KnockbackForce = value; }
        #endregion
    }
}
