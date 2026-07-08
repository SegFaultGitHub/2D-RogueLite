using Code.Characters.AI;
using Code.Characters.Players;
using Code.Spells;
using Code.Spells.Enemies;
using MyBox;
using UnityEngine;

namespace Code.Characters.Enemies {
    public class MB_Bat : AMB_Enemy {
        #region Members
        [Foldout("MB_Bat", true)]
        [SerializeField] private protected MB_UltrasonicBoom m_UltrasonicBoom;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected float m_SpellCastAt;
        [ReadOnly][SerializeField] private protected float m_SpellAvailableAt;
        #endregion

        #region Getters / Setters
        private MB_UltrasonicBoom UltrasonicBoom { get => this.m_UltrasonicBoom; }

        private float SpellCastAt { get => this.m_SpellCastAt; set => this.m_SpellCastAt = value; }
        private float SpellAvailableAt { get => this.m_SpellAvailableAt; set => this.m_SpellAvailableAt = value; }

        public override E_Enemy Enemy { get => E_Enemy.Bat; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override float TakeDamage(
            bool becomeInvulnerable,
            bool freeze,
            float value,
            bool critical,
            AMB_Character from,
            E_DamageSource source
        ) {
            float damageTaken = base.TakeDamage(becomeInvulnerable, false, value, critical, from, source);
            if (damageTaken == 0) return 0;

            if (from is AMB_Player) this.AI.SetBehaviour(E_Behaviour.Fleeing, true);
            return damageTaken;
        }

        protected override void DealtDamage(float value, AMB_Character character, E_DamageSource source) {
            base.DealtDamage(value, character, source);

            this.AI.SetBehaviour(E_Behaviour.Fleeing, false);
        }

        public bool CanUseSpell() => this.SpellAvailableAt <= Time.time;

        public void UseSpell() {
            if (!this.CanUseSpell()) return;

            AMB_Spell spell = this.UseSpell(this.UltrasonicBoom, this.Center.position);

            if (spell == null) return;

            float cooldown = spell.Cooldown;

            this.PlayHopAnimation();
            this.SpellCastAt = Time.time;
            this.SpellAvailableAt = this.SpellCastAt + cooldown;
        }

        public void Focus(bool focusing) {
            // if (focusing) this.ObjectsManager.AudioManager.PlaySkeletonFocusing();
            this.PlayFocusingAnimation(focusing);
        }
    }
}
