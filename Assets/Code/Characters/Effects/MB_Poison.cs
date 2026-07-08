using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace Code.Characters.Effects {
    public class MB_Poison : AMB_DamageOverTime {
        #region Members
        [Foldout("AMB_Poison", true)]
        [SerializeField] private protected float m_DamageModifier;
        #endregion

        #region Getters / Setters
        private float DamageModifier { get => this.m_DamageModifier; }

        protected override E_DamageSource DamageSource { get => E_DamageSource.Poison; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override float GetReceivedDamageModifier(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, HashSet<Type> appliedTypes) {
            if (!appliedTypes.Add(typeof(MB_Poison))) return 0;

            return this.DamageModifier;
        }

        // public override float ApplyOnDamageReceived(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, float value) {
        //     value = base.ApplyOnDamageReceived(dealer, receiver, damageSource, value);
        //
        //     return value * this.DamageModifier;
        // }
    }
}
