using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace Code.Characters.Effects {
    public class MB_Berserk : AMB_Effect {
        #region Members
        [Foldout("MB_Berserk", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected float m_DamageModifier;
        #endregion

        #region Getters / Setters
        private float DamageModifier { get => this.m_DamageModifier; set => this.m_DamageModifier = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void SetDamageAndMaxStacks(float damage, int stackCount) {
            this.DamageModifier = damage;
            this.MaxOccurence = stackCount;
        }

        public override float GetComputedDamageModifier(
            AMB_Character dealer,
            AMB_Character receiver,
            E_DamageSource damageSource,
            HashSet<Type> appliedTypes
        ) {
            return damageSource == E_DamageSource.Direct
                ? this.DamageModifier
                : 0;
        }
    }
}
