using System;
using Code.Characters;
using Code.UI.HUD;
using Code.UI.Text;
using MyBox;
using UnityEngine;

namespace Code.Enhancements {
    public class MB_Berserk : AMB_Enhancement {
        [Serializable]
        private protected class C_EnhancementData {
            [SerializeField] private protected float m_Ratio;
            [SerializeField] private protected float m_Duration;
            [SerializeField] private protected int m_MaxStacks;
            public float Ratio { get => this.m_Ratio; }
            public float Duration { get => this.m_Duration; }
            public int MaxStacks { get => this.m_MaxStacks; }
        }

        #region Members
        [Foldout("MB_Berserk", true)]
        [SerializeField] private protected C_EnhancementData[] m_Data;
        [SerializeField] private protected Characters.Effects.MB_Berserk m_BerserkPrefab;
        #endregion

        #region Getters / Setters
        private C_EnhancementData[] Data { get => this.m_Data; }
        private Characters.Effects.MB_Berserk BerserkPrefab { get => this.m_BerserkPrefab; }

        public override E_Enhancement Enhancement { get => E_Enhancement.Berserk; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override string GetFullDescription() {
            string[] ratios = new string[this.MaxLevel];
            string[] durations = new string[this.MaxLevel];
            string[] maxStacks = new string[this.MaxLevel];
            for (int i = 0; i < this.MaxLevel; i++) {
                ratios[i] = this.Data[i].Ratio.Percentage(false);
                durations[i] = this.Data[i].Duration.Duration();
                maxStacks[i] = this.Data[i].MaxStacks.Count();
            }

            string ratioString = ratios.AsList(this.EffectiveLevel, "%", s => s.Green());
            string durationString = durations.AsList(this.EffectiveLevel, "", s => s.Green());
            string maxStacksString = maxStacks.AsList(this.EffectiveLevel, "", s => s.Green());

            return this.Description //
                .Replace("<ratio>", ratioString)
                .Replace("<duration>", durationString)
                .Replace("<maxStacks>", maxStacksString);
        }

        public override string GetDescriptionWithUpgrade(int currentLevel, int newLevel) {
            float currentRatio = this.Data[currentLevel - 1].Ratio;
            float newConfusedRatio = this.Data[newLevel - 1].Ratio;
            string ratioBefore = currentRatio.Percentage().PositiveEffect();
            string ratioAfter = newConfusedRatio.Percentage().PositiveEffect();
            string ratioString = ratioBefore == ratioAfter
                ? ratioBefore
                : $"{ratioBefore} > {ratioAfter}";

            float currentDuration = this.Data[currentLevel - 1].Duration;
            float newDuration = this.Data[newLevel - 1].Duration;
            string durationBefore = currentDuration.Duration().PositiveEffect();
            string durationAfter = newDuration.Duration().PositiveEffect();
            string durationString = durationBefore == durationAfter
                ? durationBefore
                : $"{durationBefore} > {durationAfter}";

            int currentMaxStacks = this.Data[currentLevel - 1].MaxStacks;
            int newMaxStacks = this.Data[newLevel - 1].MaxStacks;
            string maxStacksBefore = currentMaxStacks.Count().PositiveEffect();
            string maxStacksAfter = newMaxStacks.Count().PositiveEffect();
            string maxStacksString = maxStacksBefore == maxStacksAfter
                ? maxStacksBefore
                : $"{maxStacksBefore} > {maxStacksAfter}";

            return this.Description //
                .Replace("<ratio>", ratioString)
                .Replace("<duration>", durationString)
                .Replace("<maxStacks>", maxStacksString);
        }

        public override string GetDescription() {
            float ratio = this.GetData().Ratio;
            string ratioString = ratio.Percentage().PositiveEffect();

            float duration = this.GetData().Duration;
            string durationString = duration.Duration().PositiveEffect();

            int maxStacks = this.GetData().MaxStacks;
            string maxStacksString = maxStacks.Count().PositiveEffect();

            return this.Description //
                .Replace("<ratio>", ratioString)
                .Replace("<duration>", durationString)
                .Replace("<maxStacks>", maxStacksString);
        }

        public override void ApplyOnDamageTaken(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, float value) {
            if (damageSource != E_DamageSource.Direct) return;

            Characters.Effects.MB_Berserk berserk = Instantiate(this.BerserkPrefab);
            berserk.SetDamageAndMaxStacks(this.GetData().Ratio, this.GetData().MaxStacks);
            receiver.AddEffect(berserk, receiver, this.GetData().Duration);
        }

        private C_EnhancementData GetData() => this.Data[this.EffectiveLevel - 1];
    }
}
