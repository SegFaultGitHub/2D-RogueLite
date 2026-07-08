using Code.Characters.AI;
using Code.Characters.Players;
using Code.Spells;
using Code.Spells.Enemies;
using MyBox;
using UnityEngine;

namespace Code.Characters.Enemies {
    public class MB_Necromancer : AMB_Enemy {
        #region Members
        [Foldout("MB_Skeleton", true)]
        [SerializeField] private protected MB_Summon m_Summon;
        [SerializeField] private protected MB_NecromancerScream m_NecromancerScream;
        [SerializeField] private protected float m_NecromancerScreamThreshold;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected float m_SummonSpellCastAt;
        [ReadOnly][SerializeField] private protected float m_SummonSpellAvailableAt;
        [ReadOnly][SerializeField] private protected float m_ScreamSpellCastAt;
        [ReadOnly][SerializeField] private protected float m_ScreamSpellAvailableAt;
        #endregion

        #region Getters / Setters
        private MB_Summon Summon { get => this.m_Summon; }
        private MB_NecromancerScream NecromancerScream { get => this.m_NecromancerScream; }
        private float NecromancerScreamThreshold { get => this.m_NecromancerScreamThreshold; }

        private float SummonSpellCastAt { get => this.m_SummonSpellCastAt; set => this.m_SummonSpellCastAt = value; }
        private float SummonSpellAvailableAt { get => this.m_SummonSpellAvailableAt; set => this.m_SummonSpellAvailableAt = value; }
        private float ScreamSpellCastAt { get => this.m_ScreamSpellCastAt; set => this.m_ScreamSpellCastAt = value; }
        private float ScreamSpellAvailableAt { get => this.m_ScreamSpellAvailableAt; set => this.m_ScreamSpellAvailableAt = value; }

        public override E_Enemy Enemy { get => E_Enemy.Necromancer; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void OnEnable() {
            base.OnEnable();
            this.SummonSpellCastAt = Time.time;
            this.SummonSpellAvailableAt = Time.time + 2;
        }
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

            if (from is AMB_Player && this.AI.Behaviour != E_Behaviour.Fleeing) this.AI.SetBehaviour(E_Behaviour.Aggressive, true);
            return damageTaken;
        }

        public bool CanUseSummonSpell() => this.SummonSpellAvailableAt <= Time.time;
        public bool CanUseScreamSpell() => this.ScreamSpellAvailableAt <= Time.time && this.CharacterStats.HealthRatio <= this.NecromancerScreamThreshold;

        public void UseSummonSpell(Vector2 position1, Vector2 position2) {
            if (!this.CanUseSummonSpell()) return;

            AMB_Spell spell1 = this.UseSpell(this.Summon, position1);
            AMB_Spell spell2 = this.UseSpell(this.Summon, position2);

            if (spell1 == null && spell2 == null) return;

            float cooldown = Mathf.Max(spell1?.Cooldown ?? -1, spell2?.Cooldown ?? -1);

            this.PlayHopAnimation();
            this.SummonSpellCastAt = Time.time;
            this.SummonSpellAvailableAt = this.SummonSpellCastAt + cooldown;
        }

        public void UseScreamSpell(Vector2 position) {
            if (!this.CanUseScreamSpell()) return;

            AMB_Spell spell = this.UseSpell(this.NecromancerScream, position);

            if (spell == null) return;

            float cooldown = spell.Cooldown;

            this.PlayHopAnimation();
            this.ScreamSpellCastAt = Time.time;
            this.ScreamSpellAvailableAt = this.ScreamSpellCastAt + cooldown;
        }

        public void Focus(bool focusing) {
            // if (focusing) this.ObjectsManager.AudioManager.PlaySkeletonFocusing();
            this.PlayFocusingAnimation(focusing);
        }
    }
}
