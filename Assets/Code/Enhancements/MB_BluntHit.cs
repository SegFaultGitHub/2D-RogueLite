using System;
using System.Collections.Generic;
using Code.Characters;
using Code.Characters.Effects;
using Code.UI.HUD;
using Code.UI.Text;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Enhancements {
    public class MB_BluntHit : AMB_Enhancement {
        [Serializable]
        private protected class C_EnhancementData {
            [SerializeField] private protected float m_DamageRatio;
            [SerializeField] private protected float m_ConfusedRatio;
            [SerializeField] private float m_Duration;
            public float DamageRatio { get => this.m_DamageRatio; }
            public float ConfusedRatio { get => this.m_ConfusedRatio; }
            public float Duration { get => this.m_Duration; }
        }

        #region Members
        [Foldout("MB_BluntHit", true)]
        [SerializeField] private protected C_EnhancementData[] m_Data;
        [SerializeField] private MB_Confused m_ConfusedPrefab;
        #endregion

        #region Getters / Setters
        private C_EnhancementData[] Data { get => this.m_Data; }
        private MB_Confused ConfusedPrefab { get => this.m_ConfusedPrefab; }

        public override E_Enhancement Enhancement { get => E_Enhancement.BluntHit; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override string GetFullDescription() {
            string[] damageRatios = new string[this.MaxLevel];
            string[] confusedRatios = new string[this.MaxLevel];
            string[] durations = new string[this.MaxLevel];
            for (int i = 0; i < this.MaxLevel; i++) {
                damageRatios[i] = this.Data[i].DamageRatio.Percentage(false);
                confusedRatios[i] = this.Data[i].ConfusedRatio.Percentage(false);
                durations[i] = this.Data[i].Duration.Duration();
            }

            string damageRatioString = damageRatios.AsList(this.EffectiveLevel, "%", s => s.Green());
            string confusedRatioString = confusedRatios.AsList(this.EffectiveLevel, "%", s => s.Green());
            string durationString = durations.AsList(this.EffectiveLevel, "", s => s.Green());

            return this.Description
                .Replace("<damageRatio>", damageRatioString)
                .Replace("<confusedRatio>", confusedRatioString)
                .Replace("<duration>", durationString);
        }

        public override string GetDescriptionWithUpgrade(int currentLevel, int newLevel) {
            float currentConfusedRatio = this.Data[currentLevel - 1].ConfusedRatio;
            float newConfusedRatio = this.Data[newLevel - 1].ConfusedRatio;
            string confusedRatioBefore = currentConfusedRatio.Percentage().PositiveEffect();
            string confusedRatioAfter = newConfusedRatio.Percentage().PositiveEffect();
            string confusedRatioString = confusedRatioBefore == confusedRatioAfter
                ? confusedRatioBefore
                : $"{confusedRatioBefore} > {confusedRatioAfter}";
            
            float currentDamageRatio = this.Data[currentLevel - 1].DamageRatio;
            float newDamageRatio = this.Data[newLevel - 1].DamageRatio;
            string damageRatioBefore = currentDamageRatio.Percentage().PositiveEffect();
            string damageRatioAfter = newDamageRatio.Percentage().PositiveEffect();
            string damageRatioString = damageRatioBefore == damageRatioAfter
                ? damageRatioBefore
                : $"{damageRatioBefore} > {damageRatioAfter}";
            
            float currentDuration = this.Data[currentLevel - 1].Duration;
            float newDuration = this.Data[newLevel - 1].Duration;
            string durationBefore = currentDuration.Duration().PositiveEffect();
            string durationAfter = newDuration.Duration().PositiveEffect();
            string durationString = durationBefore == durationAfter
                ? durationBefore
                : $"{durationBefore} > {durationAfter}";

            return this.Description
                .Replace("<damageRatio>", damageRatioString)
                .Replace("<confusedRatio>", confusedRatioString)
                .Replace("<duration>", durationString);
        }

        public override string GetDescription() {
            float confusedRatio = this.GetData().ConfusedRatio;
            string confusedRatioString = confusedRatio.Percentage().PositiveEffect();

            float damageRatio = this.GetData().DamageRatio;
            string damageRatioString = damageRatio.Percentage().PositiveEffect();

            float duration = this.GetData().Duration;
            string durationString = duration.Duration().PositiveEffect();

            return this.Description
                .Replace("<damageRatio>", damageRatioString)
                .Replace("<confusedRatio>", confusedRatioString)
                .Replace("<duration>", durationString);
        }

        public override float GetComputedDamageModifier(
            AMB_Character character,
            AMB_Character receiver,
            E_DamageSource damageSource,
            HashSet<Type> appliedTypes
        ) {
            return damageSource == E_DamageSource.Direct
                ? this.GetData().DamageRatio
                : 0;
        }

        public override void ApplyOnDamageInflicted(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, float value) {
            if (SC_Utils.Rate(this.GetData().ConfusedRatio)) {
                MB_Confused confused = Instantiate(this.ConfusedPrefab);
                receiver.AddEffect(confused, dealer, this.GetData().Duration);
            }
        }

        private C_EnhancementData GetData() => this.Data[this.EffectiveLevel - 1];
    }
}
