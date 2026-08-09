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

        public override float TickInterval { get => TICK_INTERVAL; }
        protected override E_DamageSource DamageSource { get => E_DamageSource.Burning; }
        #endregion

        #region Static / Readonly / Const
        public static readonly float TICK_INTERVAL = 0.5f;
        #endregion

        #region Unity methods
        #endregion

        public override float GetComputedDamageModifier(
            AMB_Character dealer,
            AMB_Character receiver,
            E_DamageSource damageSource,
            HashSet<Type> appliedTypes
        ) {
            return damageSource == E_DamageSource.Spell
                ? this.DamageModifier
                : 0;
        }

        // public override float ApplyOnDamageComputed(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, float value) {
        //     value = base.ApplyOnDamageComputed(dealer, receiver, damageSource, value);
        //
        //     return value * this.DamageModifier;
        // }
    }
}
