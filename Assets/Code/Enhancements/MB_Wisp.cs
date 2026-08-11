using System;
using System.Collections.Generic;
using Code.Characters;
using Code.UI.HUD;
using Code.UI.Text;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Enhancements {
    public class MB_Wisp : AMB_Enhancement {
        [Serializable]
        private protected class C_EnhancementData {
            [SerializeField] private protected float m_SpeedRatio;
            [SerializeField] private protected float m_AttackRatio;
            public float SpeedRatio { get => this.m_SpeedRatio; }
            public float AttackRatio { get => this.m_AttackRatio; }
        }

        #region Members
        [Foldout("MB_Wisp", true)]
        [SerializeField] private protected C_EnhancementData[] m_Data;
        #endregion

        #region Getters / Setters
        private C_EnhancementData[] Data { get => this.m_Data; }

        public override E_Enhancement Enhancement { get => E_Enhancement.Wisp; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override string GetFullDescription() {
            string[] speedRatios = new string[this.MaxLevel];
            string[] attackRatios = new string[this.MaxLevel];
            for (int i = 0; i < this.MaxLevel; i++) {
                speedRatios[i] = SC_Utils.FormatNumber(this.Data[i].SpeedRatio * 100f, decimals: 0);
                attackRatios[i] = SC_Utils.FormatNumber(Mathf.Abs(this.Data[i].AttackRatio * 100f), decimals: 0);
            }

            string speedRatioString = speedRatios.AsList(this.EffectiveLevel, "%", s => s.Green());
            string attackRatioString = attackRatios.AsList(this.EffectiveLevel, "%", s => s.Red());

            return this.Description
                .Replace("<speedRatio>", speedRatioString)
                .Replace("<attackRatio>", attackRatioString);
        }

        public override string GetDescriptionWithUpgrade(int currentLevel, int newLevel) {
            float currentSpeedRatio = this.Data[currentLevel - 1].SpeedRatio;
            float newSpeedRatio = this.Data[newLevel - 1].SpeedRatio;
            string speedBefore = $"{SC_Utils.FormatNumber(currentSpeedRatio * 100f, decimals: 0)}%".PositiveEffect();
            string speedAfter = $"{SC_Utils.FormatNumber(newSpeedRatio * 100f, decimals: 0)}%".PositiveEffect();
            string speedRatioString = speedBefore == speedAfter
                ? speedBefore
                : $"{speedBefore} > {speedAfter}";
            
            float currentAttackRatio = Mathf.Abs(this.Data[currentLevel - 1].AttackRatio);
            float newAttackRatio = Mathf.Abs(this.Data[newLevel - 1].AttackRatio);
            string attackBefore = $"{SC_Utils.FormatNumber(currentAttackRatio * 100f, decimals: 0)}%".NegativeEffect();
            string attackAfter = $"{SC_Utils.FormatNumber(newAttackRatio * 100f, decimals: 0)}%".NegativeEffect();
            string attackRatioString = attackBefore == attackAfter
                ? attackBefore
                : $"{attackBefore} > {attackAfter}";
            
            return this.Description
                .Replace("<speedRatio>", speedRatioString)
                .Replace("<attackRatio>", attackRatioString);
        }

        public override string GetDescription() {
            float speedRatio = this.GetData().SpeedRatio;
            string speedRatioString = $"{SC_Utils.FormatNumber(speedRatio * 100f, decimals: 0)}%".PositiveEffect();

            float attackRatio = Mathf.Abs(this.GetData().AttackRatio);
            string attackRatioString = $"{SC_Utils.FormatNumber(attackRatio * 100f, decimals: 0)}%".NegativeEffect();

            return this.Description
                .Replace("<speedRatio>", speedRatioString)
                .Replace("<attackRatio>", attackRatioString);
        }

        public override float GetComputedDamageModifier(
            AMB_Character character,
            AMB_Character receiver,
            E_DamageSource damageSource,
            HashSet<Type> appliedTypes
        ) {
            return damageSource == E_DamageSource.Spell
                ? this.GetData().AttackRatio
                : 0;
        }

        public override float GetCooldownModifier(AMB_Character character) {
            return this.GetData().SpeedRatio;
        }

        private C_EnhancementData GetData() => this.Data[this.EffectiveLevel - 1];
    }
}
