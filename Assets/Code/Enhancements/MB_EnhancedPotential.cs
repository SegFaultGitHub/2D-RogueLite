using System;
using Code.Characters;
using Code.Characters.Players;
using Code.UI.HUD;
using Code.UI.Text;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Enhancements {
    public class MB_EnhancedPotential : AMB_Enhancement {
        [Serializable]
        private protected class C_EnhancementData {
            [SerializeField] private protected int m_Count;
            public int Count { get => this.m_Count; }
        }

        #region Members
        [Foldout("MB_EnhancedPotential", true)]
        [SerializeField] private protected C_EnhancementData[] m_Data;
        #endregion

        #region Getters / Setters
        private C_EnhancementData[] Data { get => this.m_Data; }

        public override E_Enhancement Enhancement { get => E_Enhancement.HerculesStamina; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override string GetFullDescription() {
            string[] counts = new string[this.MaxLevel];
            for (int i = 0; i < this.MaxLevel; i++) {
                counts[i] = SC_Utils.FormatNumber(this.Data[i].Count, decimals: 0);
            }

            string countString = counts.AsList(this.EffectiveLevel, "%", s => s.Green());

            return this.Description.Replace("<count>", countString);
        }

        public override string GetDescriptionWithUpgrade(int currentLevel, int newLevel) {
            int currentCount = this.Data[currentLevel - 1].Count;
            int newCount = this.Data[newLevel - 1].Count;
            string before = $"{SC_Utils.FormatNumber(currentCount, decimals: 0)}".PositiveEffect();
            string after = $"{SC_Utils.FormatNumber(newCount, decimals: 0)}".PositiveEffect();
            string countString = $"{before} > {after}";

            return this.Description.Replace("<count>", countString);
        }

        public override string GetDescription() {
            int count = this.GetData().Count;
            string countString = SC_Utils.FormatNumber(count, decimals: 0).PositiveEffect();

            return this.Description.Replace("<count>", countString);
        }

        public override void OnNew(AMB_Character character) {
            base.OnNew(character);
            int count = this.GetData().Count;
            for (int i = 0; i < count; i++) {
                (character as AMB_Player)!.AddEnhancementSlot();
            }
        }

        public override void OnUpgrade(AMB_Character character, int previousLevel) {
            base.OnUpgrade(character, previousLevel);
            int previousCount = this.Data[previousLevel - 1].Count;
            int currentCount = this.GetData().Count;
            for (int i = 0; i < currentCount - previousCount; i++) {
                (character as AMB_Player)!.AddEnhancementSlot();
            }
        }

        private C_EnhancementData GetData() => this.Data[this.EffectiveLevel - 1];
    }
}
