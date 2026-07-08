using System;
using Code.Characters;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Enhancements {
    public class MB_AttackSpeed : AMB_Enhancement {
        [Serializable]
        private protected class SC_EnhancementData {
            [SerializeField] private protected float m_Ratio;
            public float Ratio { get => this.m_Ratio; }
        }

        #region Members
        [Foldout("MB_AttackSpeed", true)]
        [SerializeField] private protected SC_EnhancementData[] m_Data;
        #endregion

        #region Getters / Setters
        private SC_EnhancementData[] Data { get => this.m_Data; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override string GetDescription() {
            float ratio = this.GetData().Ratio;
            string ratioString = $"{SC_Utils.FormatNumber(ratio * 100f, decimals: 0)}%";
            return this.Description.Replace("<ratio>", ratioString);
        }

        public override float GetCooldownModifier(AMB_Character character) {
            return this.GetData().Ratio;
        }

        private SC_EnhancementData GetData() => this.Data[this.EffectiveLevel - 1];
    }
}
