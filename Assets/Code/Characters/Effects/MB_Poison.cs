using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace Code.Characters.Effects {
    public class MB_Poison : AMB_DamageOverTime {
        #region Members
        [Foldout("MB_Poison", true)]
        [SerializeField] private protected float m_DamageModifier;
        #endregion

        #region Getters / Setters
        private float DamageModifier { get => this.m_DamageModifier; }

        public override float TickInterval { get => TICK_INTERVAL; }
        protected override E_DamageSource DamageSource { get => E_DamageSource.Poison; }
        #endregion

        #region Static / Readonly / Const
        public static readonly float TICK_INTERVAL = .25f;
        #endregion

        #region Unity methods
        #endregion

        public override float GetReceivedDamageModifier(
            AMB_Character dealer,
            AMB_Character receiver,
            E_DamageSource damageSource,
            HashSet<Type> appliedTypes
        ) {
            if (!appliedTypes.Add(typeof(MB_Poison))) return 0;

            return damageSource == E_DamageSource.Direct
                ? this.DamageModifier
                : 0;
        }
    }
}
