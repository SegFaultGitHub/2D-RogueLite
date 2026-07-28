using System;
using System.Collections.Generic;
using System.Linq;
using Code.Characters.Controllers;
using Code.Characters.Effects;
using Code.Characters.Stats;
using Code.Managers;
using Code.Spells;
using Code.UI.HUD;
using Code.Utils;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;

namespace Code.Characters {
    [SelectionBase]
    [RequireComponent(typeof(MB_CharacterStats))]
    public abstract class AMB_Character : MonoBehaviour {
        [Serializable]
        public class C_SpeedRatioRef {
            [SerializeField] private protected float m_Ratio;

            public float Ratio { get => this.m_Ratio; set => this.m_Ratio = value; }
        }

        #region Members
        [Foldout("AMB_Character", true)]
        [SerializeField] private protected Transform m_Center;
        [SerializeField] private protected Transform m_EffectsParent;

        [SerializeField] private protected bool m_Knockbackable;

        [SerializeField] private protected Transform m_SpellSource;
        [SerializeField] private protected float m_CastDistance;

        [SerializeField] private protected MB_VisualEffects m_VisualEffects;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;

        [ReadOnly][SerializeField] private protected Animator m_Animator;
        [ReadOnly][SerializeField] private protected AMB_BaseController m_BaseController;
        [ReadOnly][SerializeField] private protected MB_CharacterStats m_CharacterStats;

        [ReadOnly][SerializeField] private protected CollectionWrapperList<AMB_Effect> m_Effects;

        [ReadOnly][SerializeField] private protected bool m_Dead;
        [ReadOnly][SerializeField] private protected bool m_Invulnerable;
        [ReadOnly][SerializeField] private protected CollectionWrapperList<C_SpeedRatioRef> m_SpeedRatio = new();
        #endregion

        #region Getters / Setters
        public Transform Center { get => this.m_Center; }
        private Transform EffectsParent { get => this.m_EffectsParent; }

        protected bool Knockbackable { get => this.m_Knockbackable; set => this.m_Knockbackable = value; }

        private Transform SpellSource { get => this.m_SpellSource; }
        private float CastDistance { get => this.m_CastDistance; }

        public MB_VisualEffects VisualEffects { get => this.m_VisualEffects; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }

        private Animator Animator { get => this.m_Animator; set => this.m_Animator = value; }
        public AMB_BaseController BaseController { get => this.m_BaseController; private set => this.m_BaseController = value; }
        public MB_CharacterStats CharacterStats { get => this.m_CharacterStats; private set => this.m_CharacterStats = value; }

        public CollectionWrapperList<AMB_Effect> Effects { get => this.m_Effects; }

        protected bool Dead { get => this.m_Dead; set => this.m_Dead = value; }
        public bool Invulnerable { get => this.m_Invulnerable; private set => this.m_Invulnerable = value; }
        public CollectionWrapperList<C_SpeedRatioRef> SpeedRatio { get => this.m_SpeedRatio; }

        public abstract IEnumerable<I_Effect> AllEffects { get; }
        #endregion

        #region Static / Readonly / Const
        public const float INVULNERABILITY_DURATION = .233f;
        private static readonly int HIT = Animator.StringToHash("Hit");
        private static readonly int HOP = Animator.StringToHash("Hop");
        private static readonly int FOCUSING = Animator.StringToHash("Focusing");
        #endregion

        #region Unity methods
        protected virtual void Awake() {
            this.Animator = this.GetComponent<Animator>();
            this.BaseController = this.GetComponent<AMB_BaseController>();
            this.CharacterStats = this.GetComponent<MB_CharacterStats>();
            this.VisualEffects.UpdateVisuals(this);
        }

        protected virtual void FixedUpdate() { }

        protected virtual void OnTriggerEnter2D(Collider2D other) {
            other.GetComponentInParent<AMB_PositionalSpell>()?.TriggerEnter(this);
            other.GetComponentInParent<AMB_MovingSpell>()?.Collide(this);
        }

        protected virtual void OnTriggerStay2D(Collider2D other) {
            other.GetComponentInParent<AMB_PositionalSpell>()?.TriggerStay(this);
        }

        protected virtual void OnTriggerExit2D(Collider2D other) {
            other.GetComponentInParent<AMB_PositionalSpell>()?.TriggerExit(this);
        }
        #endregion

        public virtual void Initialize() {
            this.CharacterStats = this.GetComponent<MB_CharacterStats>();
        }

        public virtual void PostInitialize() { }

        public virtual void Aim(Vector2 direction) { }

        public void Knockback(Vector2 direction, float force) {
            if (!this.Knockbackable || direction.sqrMagnitude == 0) return;
            this.BaseController.Knockback(direction.normalized, force);
        }

        // `effect` must be an instance!
        public void AddEffect(AMB_Effect effect, AMB_Character from, float? duration = null) {
            if (effect.Unique) {
                AMB_Effect currentEffect = this.Effects.Value.Find(currentEffect => currentEffect.GetType() == effect.GetType());
                if (currentEffect != null) {
                    if (currentEffect.IsPermanent) return;
                    if (currentEffect.TTL > duration) return;

                    this.Effects.Remove(currentEffect);
                    Destroy(currentEffect.gameObject);
                }
            }

            effect.Initialize(this, from, duration);
            effect.OnApply(this);

            this.Effects.Add(effect);
            effect.transform.SetParent(this.EffectsParent);
            this.VisualEffects.UpdateVisuals(this);
        }

        public void RemoveEffect(AMB_Effect effect) {
            effect.OnRemove(this);

            this.Effects.Remove(effect);
            Destroy(effect.gameObject);
            this.VisualEffects.UpdateVisuals(this);
        }

        public void SetInvulnerable() {
            this.Invulnerable = true;
            this.InSeconds(INVULNERABILITY_DURATION, () => this.Invulnerable = false);
        }

        public virtual float TakeDamage(
            bool becomeInvulnerable,
            bool freeze,
            float value,
            bool critical,
            AMB_Character from,
            E_DamageSource source
        ) {
            if (value <= 0) return 0;
            this.PlayHitAnimation();
            switch (source) {
                case E_DamageSource.Spell:
                case E_DamageSource.SummonDeath:
                    this.PlayHurtSoundEffect();
                    break;
                case E_DamageSource.Burning:
                case E_DamageSource.Poison:
                    this.PlayHurtFromDamageOverTimeSoundEffect();
                    break;
                case E_DamageSource.Melee:
                case E_DamageSource.Passive:
                case E_DamageSource.Traps:
                default:
                    break;
            }

            if (becomeInvulnerable) this.SetInvulnerable();

            float realDamageDealt = this.CharacterStats.TakeDamage(from, value, critical, source);
            this.AllEffects.ForEach(effect => effect.ApplyOnDamageTaken(from, this, source, realDamageDealt));

            from?.DealtDamage(realDamageDealt, this, source);
            this.ObjectsManager.DamageCanvas.DamageDealt(this, -realDamageDealt, critical);

            if (this.CharacterStats.IsDead()) {
                this.Die(from);
                from?.Kill(this);
            }

            return realDamageDealt;
        }

        public virtual float Heal(AMB_Character from, float value) {
            float healReceived = this.CharacterStats.Heal(from, value);

            this.ObjectsManager.DamageCanvas.Heal(this, healReceived);

            return healReceived;
        }

        protected virtual void DealtDamage(float damageDealt, AMB_Character character, E_DamageSource source) {
            this.AllEffects.ForEach(effect => effect.ApplyOnDamageInflicted(this, character, source, damageDealt));
        }

        protected abstract void Kill(AMB_Character character);

        protected virtual bool Die(AMB_Character killedBy) {
            Debug.Log("Die");

            if (this.Dead) return false;
            this.Dead = true;
            return true;
        }

        public virtual void OnDashStart() {
            this.PlayDashSoundEffect();
            this.AllEffects.ForEach(effect => effect.ApplyOnDashStart(this));
        }

        public virtual void OnDashEnd() {
            this.AllEffects.ForEach(effect => effect.ApplyOnDashEnd(this));
        }

        // Returns (damage, criticalHit)
        public virtual (float, bool) ComputeDamage(AMB_Character target, float baseDamage, E_DamageSource source) {
            (float damage, bool critical) = this.CharacterStats.ComputeDamage(target, baseDamage, source);

            return (damage, critical);
        }

        #region Spells
        [CanBeNull]
        protected AMB_DirectionalSpell UseSpell([CanBeNull] AMB_DirectionalSpell spell) {
            return this.CastSpell(spell, this.SpellSource, this.BaseController.AimDirection, this.CastDistance);
        }

        [CanBeNull]
        protected AMB_RotatingSpell UseSpell([CanBeNull] AMB_RotatingSpell spell) {
            float angle = this.BaseController.AimAngle;
            return this.CastSpell(spell, this.SpellSource, angle);
        }

        [CanBeNull]
        protected AMB_PositionalSpell UseSpell([CanBeNull] AMB_PositionalSpell spell, Vector2 position) {
            return this.CastSpell(spell, this.SpellSource.position, position);
        }

        [CanBeNull]
        protected AMB_FollowingSpell UseSpell([CanBeNull] AMB_FollowingSpell spell, Transform follow) {
            return this.CastSpell(spell, follow);
        }

        public virtual AMB_DirectionalSpell CastSpell(AMB_DirectionalSpell spell, Transform origin, Vector3 direction, float distance = 0) {
            AMB_DirectionalSpell spellObj = Instantiate(spell);
            spellObj.transform.position = origin.position + direction.normalized * distance;
            spellObj.SetCharacter(this);
            spellObj.SetDirection(direction);
            return spellObj;
        }

        public virtual AMB_RotatingSpell CastSpell(AMB_RotatingSpell spell, Transform origin, float angle) {
            AMB_RotatingSpell spellObj = Instantiate(spell);
            spellObj.transform.position = origin.position;
            spellObj.Follow = origin;
            spellObj.SetCharacter(this);
            spellObj.SetInitialAngle(angle, false);
            return spellObj;
        }

        public virtual AMB_PositionalSpell CastSpell(AMB_PositionalSpell spell, Vector3 origin, Vector3 position) {
            AMB_PositionalSpell spellObj = Instantiate(spell);
            spellObj.transform.position = position;
            spellObj.SetCharacter(this);
            spellObj.SetPositions(origin, position);
            return spellObj;
        }

        public virtual AMB_FollowingSpell CastSpell(AMB_FollowingSpell spell, Transform origin) {
            AMB_FollowingSpell spellObj = Instantiate(spell);
            spellObj.transform.position = origin.position;
            spellObj.Follow = origin;
            spellObj.SetCharacter(this);
            return spellObj;
        }
        #endregion

        #region Sound effects
        protected virtual void PlayHurtSoundEffect() { }
        protected virtual void PlayHurtFromDamageOverTimeSoundEffect() { }
        protected virtual void PlayDashSoundEffect() { }
        #endregion

        #region Animation
        public void PlayHopAnimation() => this.Animator.SetTrigger(HOP);
        public void PlayHitAnimation() => this.Animator.SetTrigger(HIT);
        public void PlayFocusingAnimation(bool focusing) => this.Animator.SetBool(FOCUSING, focusing);
        #endregion
    }
}
