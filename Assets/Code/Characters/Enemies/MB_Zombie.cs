using System;
using Code.Characters.Players;
using Code.Spells;
using Code.Spells.Enemies;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Characters.Enemies {
    public class MB_Zombie : AMB_Enemy {
        #region Members
        [Foldout("MB_Zombie", true)]
        [SerializeField] private protected MB_ZombieAttack m_ZombieAttack;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected float m_SpellCastAt;
        [ReadOnly][SerializeField] private protected float m_SpellAvailableAt;

        [ReadOnly][SerializeField] private protected bool m_InitialKnockbackable;
        #endregion

        #region Getters / Setters
        private MB_ZombieAttack ZombieAttack { get => this.m_ZombieAttack; }

        private float SpellCastAt { get => this.m_SpellCastAt; set => this.m_SpellCastAt = value; }
        private float SpellAvailableAt { get => this.m_SpellAvailableAt; set => this.m_SpellAvailableAt = value; }

        private bool InitialKnockbackable { get => this.m_InitialKnockbackable; set => this.m_InitialKnockbackable = value; }

        public override E_Enemy Enemy { get => E_Enemy.Zombie; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        override protected void Awake() {
            base.Awake();

            this.InitialKnockbackable = this.Knockbackable;
        }
        #endregion

        public bool CanUseSpell() => this.SpellAvailableAt <= Time.time;

        public void UseSpell() {
            if (!this.CanUseSpell()) return;

            AMB_Spell spell = this.UseSpell(this.ZombieAttack);

            if (spell == null) return;

            float cooldown = spell.Cooldown;

            this.PlayHopAnimation();
            this.SpellCastAt = Time.time;
            this.SpellAvailableAt = this.SpellCastAt + cooldown;
        }

        public void Focus(bool focusing) {
            // ReSharper disable once SimplifyConditionalTernaryExpression
            this.Knockbackable = focusing
                ? false
                : this.InitialKnockbackable;
            this.PlayFocusingAnimation(focusing);
        }

        protected override void PlayHurtSoundEffect() => this.ObjectsManager.AudioManager.PlayZombieHurt();
    }
}
