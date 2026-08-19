using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace Code.Characters.Effects {
    public class MB_Focus : AMB_Effect {
        #region Members
        [Foldout("MB_Focus", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected float m_SpeedModifier;
        #endregion

        #region Getters / Setters
        private float SpeedModifier { get => this.m_SpeedModifier; set => this.m_SpeedModifier = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void SetSpeedAndMaxStacks(float speed, int stackCount) {
            this.SpeedModifier = speed;
            this.MaxOccurence = stackCount;
        }

        public override float GetCooldownModifier(AMB_Character character) => this.SpeedModifier;
    }
}
