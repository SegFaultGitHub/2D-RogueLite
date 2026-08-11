using System;
using System.Collections.Generic;
using Code.Characters;
using Code.UI.HUD;
using Code.UI.Text;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Enhancements {
    public class MB_Hypochondria : AMB_Enhancement {
        [Serializable]
        private protected class C_EnhancementData {
            [SerializeField] private protected float m_Ratio;
            public float Ratio { get => this.m_Ratio; }
        }

        #region Members
        [Foldout("MB_Hypochondria", true)]
        [SerializeField] private protected C_EnhancementData[] m_Data;
        #endregion

        #region Getters / Setters
        private C_EnhancementData[] Data { get => this.m_Data; }

        public override E_Enhancement Enhancement { get => E_Enhancement.Hypochondria; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override string GetFullDescription() {
            string[] ratios = new string[this.MaxLevel];
            for (int i = 0; i < this.MaxLevel; i++) {
                ratios[i] = SC_Utils.FormatNumber(this.Data[i].Ratio * 100f, decimals: 0);
            }

            string ratioString = ratios.AsList(this.EffectiveLevel, "%", s => s.Green());

            return this.Description.Replace("<ratio>", ratioString);
        }

        public override string GetDescriptionWithUpgrade(int currentLevel, int newLevel) {
            float currentRatio = this.Data[currentLevel - 1].Ratio;
            float newRatio = this.Data[newLevel - 1].Ratio;
            string before = $"{SC_Utils.FormatNumber(currentRatio * 100f, decimals: 0)}%".PositiveEffect();
            string after = $"{SC_Utils.FormatNumber(newRatio * 100f, decimals: 0)}%".PositiveEffect();
            string ratioString = before == after
                ? before
                : $"{before} > {after}";

            return this.Description.Replace("<ratio>", ratioString);
        }

        public override string GetDescription() {
            float ratio = this.GetData().Ratio;
            string ratioString = $"{SC_Utils.FormatNumber(ratio * 100f, decimals: 0)}%".PositiveEffect();

            return this.Description.Replace("<ratio>", ratioString);
        }

        public override float GetComputedDamageModifier(
            AMB_Character character,
            AMB_Character receiver,
            E_DamageSource damageSource,
            HashSet<Type> appliedTypes
        ) {
            return damageSource == E_DamageSource.Spell
                ? this.GetData().Ratio * (1 - character.CharacterStats.HealthRatio)
                : 0;
        }

        private C_EnhancementData GetData() => this.Data[this.EffectiveLevel - 1];
    }
}
