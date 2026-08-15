using System;
using Code.Characters;
using Code.UI.HUD;
using Code.UI.Text;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Enhancements {
    public class MB_HuntersInstinct : AMB_Enhancement {
        [Serializable]
        private protected class C_EnhancementData {
            [SerializeField] private protected float m_Ratio;
            public float Ratio { get => this.m_Ratio; }
        }

        #region Members
        [Foldout("MB_HuntersInstinct", true)]
        [SerializeField] private protected C_EnhancementData[] m_Data;
        #endregion

        #region Getters / Setters
        private C_EnhancementData[] Data { get => this.m_Data; }

        public override E_Enhancement Enhancement { get => E_Enhancement.HuntersInstinct; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override string GetFullDescription() {
            string[] ratios = new string[this.MaxLevel];
            for (int i = 0; i < this.MaxLevel; i++) {
                ratios[i] = this.Data[i].Ratio.Percentage(false);
            }

            string ratioString = ratios.AsList(this.EffectiveLevel, "%", s => s.Green());

            return this.Description.Replace("<ratio>", ratioString);
        }

        public override string GetDescriptionWithUpgrade(int currentLevel, int newLevel) {
            float currentRatio = this.Data[currentLevel - 1].Ratio;
            float newRatio = this.Data[newLevel - 1].Ratio;
            string before = currentRatio.Percentage().PositiveEffect();
            string after = newRatio.Percentage().PositiveEffect();
            string ratioString = before == after
                ? before
                : $"{before} > {after}";

            return this.Description.Replace("<ratio>", ratioString);
        }

        public override string GetDescription() {
            float ratio = this.GetData().Ratio;
            string ratioString = ratio.Percentage().PositiveEffect();

            return this.Description.Replace("<ratio>", ratioString);
        }

        public override float GetCriticalRateModifier(AMB_Character character, E_DamageSource damageSource) {
            return damageSource == E_DamageSource.Spell
                ? this.GetData().Ratio
                : 0;
        }

        private C_EnhancementData GetData() => this.Data[this.EffectiveLevel - 1];
    }
}
