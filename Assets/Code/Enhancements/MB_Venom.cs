using System;
using System.Collections.Generic;
using System.Linq;
using Code.Characters;
using Code.Characters.Effects;
using Code.UI.HUD;
using Code.UI.Text;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Enhancements {
    public class MB_Venom : AMB_Enhancement {
        [Serializable]
        private protected class C_EnhancementData {
            [SerializeField] private protected float m_DamagePerTick;
            [SerializeField] private protected int m_TickCount;
            public float DamagePerTick { get => this.m_DamagePerTick; }
            public int TickCount { get => this.m_TickCount; }
        }

        #region Members
        [Foldout("MB_Venom", true)]
        [SerializeField] private protected C_EnhancementData[] m_Data;
        [SerializeField] private protected MB_Poison m_PoisonPrefab;
        #endregion

        #region Getters / Setters
        private C_EnhancementData[] Data { get => this.m_Data; }
        private MB_Poison PoisonPrefab { get => this.m_PoisonPrefab; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override string GetFullDescription() {
            string[] damages = new string[this.MaxLevel];
            for (int i = 0; i < this.MaxLevel; i++) {
                damages[i] = SC_Utils.FormatNumber(this.Data[i].DamagePerTick * this.Data[i].TickCount, decimals: 0);
            }

            string damageString = damages.AsList(this.EffectiveLevel, s => s.Green());
            return this.Description.Replace("<damage>", damageString);
        }

        public override string GetDescriptionWithUpgrade(int currentLevel, int newLevel) {
            float currentDamage = this.Data[currentLevel - 1].TickCount * this.Data[currentLevel - 1].DamagePerTick;
            float newDamage = this.Data[newLevel - 1].TickCount * this.Data[newLevel - 1].DamagePerTick;
            string damageString =
                $"{SC_Utils.FormatNumber(currentDamage, decimals: 0).PositiveEffect()} > {SC_Utils.FormatNumber(newDamage, decimals: 0).PositiveEffect()}";
            return this.Description.Replace("<damage>", damageString);
        }

        public override string GetDescription() {
            float damage = this.GetData().DamagePerTick * this.GetData().TickCount;
            string damageString = SC_Utils.FormatNumber(damage, decimals: 0).PositiveEffect();
            return this.Description.Replace("<damage>", damageString);
        }

        public override void ApplyOnDamageInflicted(
            AMB_Character dealer,
            AMB_Character receiver,
            E_DamageSource damageSource,
            float value
        ) {
            if (damageSource != E_DamageSource.Spell) return;

            MB_Poison poison = Instantiate(this.PoisonPrefab);
            poison.SetDamage(this.GetData().DamagePerTick, this.GetData().TickCount);
            receiver.AddEffect(poison, dealer);
        }

        private C_EnhancementData GetData() => this.Data[this.EffectiveLevel - 1];
    }
}
