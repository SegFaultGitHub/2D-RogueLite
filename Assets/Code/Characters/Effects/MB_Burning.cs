using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace Code.Characters.Effects {
    public class MB_Burning : AMB_DamageOverTime {
        #region Members
        [Foldout("AMB_Burning", true)]
        [SerializeField] private protected float m_DamageModifier;
        #endregion

        #region Getters / Setters
        private float DamageModifier { get => this.m_DamageModifier; }

        protected override E_DamageSource DamageSource { get => E_DamageSource.Burning; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override float GetComputedDamageModifier(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, HashSet<Type> appliedTypes) {
            if (!appliedTypes.Add(typeof(MB_Burning))) return 0;

            return this.DamageModifier;
        }

        // public override float ApplyOnDamageComputed(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, float value) {
        //     value = base.ApplyOnDamageComputed(dealer, receiver, damageSource, value);
        //
        //     return value * this.DamageModifier;
        // }
    }
}
