using System;
using Code.Characters;
using Code.UI.HUD;
using Code.UI.Text;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Enhancements {
    public class MB_ViolentDash : AMB_Enhancement {
        [Serializable]
        public class C_EnhancementData {
            [SerializeField] private protected float m_Damage;
            [SerializeField] private protected float m_KnockbackForce;
            public float Damage { get => this.m_Damage; }
            public float KnockbackForce { get => this.m_KnockbackForce; }
        }

        #region Members
        [Foldout("MB_ViolentDash", true)]
        [SerializeField] private protected C_EnhancementData[] m_Data;
        [SerializeField] private protected Spells.Player.MB_ViolentDash m_ViolentDash;
        #endregion

        #region Getters / Setters
        private C_EnhancementData[] Data { get => this.m_Data; }
        private Spells.Player.MB_ViolentDash ViolentDash { get => this.m_ViolentDash; }

        private Spells.Player.MB_ViolentDash CastedViolentDash { get; set; }

        public override E_Enhancement Enhancement { get => E_Enhancement.ViolentDash; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override string GetFullDescription() {
            string[] damages = new string[this.MaxLevel];
            for (int i = 0; i < this.MaxLevel; i++) {
                damages[i] = this.Data[i].Damage.Damage();
            }

            string damageString = damages.AsList(this.EffectiveLevel, "", s => s.Green());

            return this.Description.Replace("<damage>", damageString);
        }

        public override string GetDescriptionWithUpgrade(int currentLevel, int newLevel) {
            float currentDamage = this.Data[currentLevel - 1].Damage;
            float newDamage = this.Data[newLevel - 1].Damage;
            string before = currentDamage.Damage().PositiveEffect();
            string after = newDamage.Damage().PositiveEffect();
            string damageString = before == after
                ? before
                : $"{before} > {after}";

            return this.Description.Replace("<damage>", damageString);
        }

        public override string GetDescription() {
            float damage = this.GetData().Damage;
            string damageString = damage.Damage().PositiveEffect();

            return this.Description.Replace("<damage>", damageString);
        }

        public override void ApplyOnDashStart(AMB_Character character) {
            if (this.CastedViolentDash != null) this.CastedViolentDash.Collide(null, true);

            this.CastedViolentDash = character.CastSpell(this.ViolentDash, character.transform) as Spells.Player.MB_ViolentDash;

            if (this.CastedViolentDash != null) {
                this.CastedViolentDash.SetData(this.GetData());
            }
        }

        public override void ApplyOnDashEnd(AMB_Character character) {
            if (this.CastedViolentDash != null) this.CastedViolentDash.Collide(null, true);

            this.CastedViolentDash = null;
        }

        private C_EnhancementData GetData() => this.Data[this.EffectiveLevel - 1];
    }
}
