using System.Collections.Generic;
using System.Linq;
using Code.Characters.Controllers.Players;
using Code.Enhancements;
using Code.Managers;
using Code.Spells;
using Code.UI.HUD;
using Code.Utils;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Characters.Players {
    public abstract class AMB_Player : AMB_Character {
        #region Members
        [Foldout("MB_Player", true)]
        [SerializeField] private protected float m_DashCooldown;
        [SerializeField] private protected GameObject m_DashReloadFrame;
        [SerializeField] private protected Image m_DashReloadImage;

        [SerializeField] private protected GameObject m_SpritesBox;

        [SerializeField] private protected Transform m_EnhancementsParent;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_PlayerController m_PlayerController;

        [ReadOnly][SerializeField] private protected bool m_UsingMainSpell;
        [ReadOnly][SerializeField] private protected float m_MainSpellCastAt;
        [ReadOnly][SerializeField] private protected float m_MainSpellAvailableAt;
        [ReadOnly][SerializeField] private protected bool m_UsingSecondarySpell;
        [ReadOnly][SerializeField] private protected float m_SecondarySpellCastAt;
        [ReadOnly][SerializeField] private protected float m_SecondarySpellAvailableAt;

        [ReadOnly][SerializeField] private protected float m_DashCastAt;
        [ReadOnly][SerializeField] private protected float m_DashAvailableAt;

        [ReadOnly][SerializeField] private protected CollectionWrapperList<AMB_Enhancement> m_Enhancements;

        [Separator("TEMP")]
        [SerializeField] private protected AMB_Spell m_TEMP_MainSpell;
        [SerializeField] private protected AMB_Spell m_TEMP_SecondarySpell;

        [SerializeField] private protected MB_AttackSpeed m_TEMP_AttackSpeed;
        [SerializeField] private protected MB_AttackPower m_TEMP_AttackPower;
        #endregion

        #region Getters / Setters
        private float DashCooldown { get => this.m_DashCooldown; }
        private GameObject DashReloadFrame { get => this.m_DashReloadFrame; }
        private Image DashReloadImage { get => this.m_DashReloadImage; }

        private GameObject SpritesBox { get => this.m_SpritesBox; }

        public Transform EnhancementsParent { get => this.m_EnhancementsParent; }

        public MB_PlayerController PlayerController { get => this.m_PlayerController; private set => this.m_PlayerController = value; }

        public bool UsingMainSpell { get => this.m_UsingMainSpell; set => this.m_UsingMainSpell = value; }
        private float MainSpellCastAt { get => this.m_MainSpellCastAt; set => this.m_MainSpellCastAt = value; }
        public float MainSpellAvailableAt { get => this.m_MainSpellAvailableAt; set => this.m_MainSpellAvailableAt = value; }
        public bool UsingSecondarySpell { get => this.m_UsingSecondarySpell; set => this.m_UsingSecondarySpell = value; }
        private float SecondarySpellCastAt { get => this.m_SecondarySpellCastAt; set => this.m_SecondarySpellCastAt = value; }
        public float SecondarySpellAvailableAt { get => this.m_SecondarySpellAvailableAt; set => this.m_SecondarySpellAvailableAt = value; }

        private float DashCastAt { get => this.m_DashCastAt; set => this.m_DashCastAt = value; }
        private float DashAvailableAt { get => this.m_DashAvailableAt; set => this.m_DashAvailableAt = value; }

        public CollectionWrapperList<AMB_Enhancement> Enhancements { get => this.m_Enhancements; }

        public override IEnumerable<I_Effect> AllEffects {
            get {
                IEnumerable<I_Effect> effects = this.Effects.Value.Select(effect => (I_Effect)effect);
                IEnumerable<I_Effect> enhancements = this.Enhancements.Value.Select(effect => (I_Effect)effect);
                return effects.Concat(enhancements);
            }
        }
        #endregion

        #region Static / Readonly / Const
        public const int DEFAULT_MAX_ENHANCEMENTS = 4;
        #endregion

        #region Unity methods
        protected override void FixedUpdate() {
            base.FixedUpdate();
            this.ObjectsManager.PlayerHUD.SetMainSpellRatio(
                this.m_TEMP_MainSpell == null
                    ? 0
                    : SC_Utils.MapFrom(this.MainSpellCastAt, this.MainSpellAvailableAt, 0, 1, Time.time)
            );
            this.ObjectsManager.PlayerHUD.SetSecondarySpellRatio(
                this.m_TEMP_SecondarySpell == null
                    ? 0
                    : SC_Utils.MapFrom(this.SecondarySpellCastAt, this.SecondarySpellAvailableAt, 0, 1, Time.time)
            );
            if (this.UsingMainSpell && this.PlayerController.Active) this.UseMainSpell();
            if (this.UsingSecondarySpell && this.PlayerController.Active) this.UseSecondarySpell();

            this.DashReloadFrame.SetActive(this.DashReloadImage.fillAmount < 1);
            this.DashReloadImage.fillAmount = SC_Utils.MapFrom(this.DashCastAt, this.DashAvailableAt, 0, 1, Time.time);
        }
        #endregion

        public override void Initialize() {
            base.Initialize();
            this.PlayerController = this.GetComponent<MB_PlayerController>();
            this.DashCastAt = -1;
            this.DashAvailableAt = 0;
            this.MainSpellCastAt = -1;
            this.MainSpellAvailableAt = 0;
        }

        public override void PostInitialize() {
            this.ObjectsManager.PlayerHUD.SetHealth(this.CharacterStats.CurrentHealth, this.CharacterStats.MaxHealth);

            MB_AttackSpeed as1 = Instantiate(this.m_TEMP_AttackSpeed);
            as1.Level = 3;
            MB_AttackSpeed as2 = Instantiate(this.m_TEMP_AttackSpeed);
            as2.Level = 3;
            MB_AttackPower ap1 = Instantiate(this.m_TEMP_AttackPower);
            ap1.Level = 3;
            MB_AttackPower ap2 = Instantiate(this.m_TEMP_AttackPower);
            ap2.Level = 3;

            this.AddEnhancement(as1);
            this.AddEnhancement(as2);
            this.AddEnhancement(ap1);
            this.AddEnhancement(ap2);
        }

        public void Hide() {
            this.SpritesBox.SetActive(false);
            this.DashCastAt = -1;
            this.DashAvailableAt = 0;
            this.MainSpellCastAt = -1;
            this.MainSpellAvailableAt = 0;
            this.PlayerController.Active = false;
        }

        public void Show() {
            this.SpritesBox.SetActive(true);
            this.PlayerController.Active = true;
            this.InSeconds(1, () => { });
        }

        public void Dash() {
            if (Time.time < this.DashAvailableAt
                || this.ObjectsManager.PauseManager.PauseState != MB_PauseManager.E_PauseState.NotPaused
                || !this.PlayerController.Active) return;

            this.DashCastAt = Time.time;
            this.DashAvailableAt = this.DashCastAt + this.DashCooldown;
            this.PlayerController.Dash();
        }

        public override float TakeDamage(
            bool becomeInvulnerable,
            bool freeze,
            float value,
            bool critical,
            AMB_Character from,
            E_DamageSource damageSource
        ) {
            if (value == 0) {
                this.ObjectsManager.DamageCanvas.Dodge(this);
                return 0;
            } else {
                float damageTaken = base.TakeDamage(becomeInvulnerable, freeze, value, critical, from, damageSource);

                if (damageTaken == 0) return 0;

                // this.ObjectsManager.Stats.AddDamageReceived(
                //     from is AMB_Enemy enemy
                //         ? enemy.Enemy
                //         : null,
                //     damageTaken,
                //     damageSource
                // );

                this.ObjectsManager.PlayerHUD.SetHealth(this.CharacterStats.CurrentHealth, this.CharacterStats.MaxHealth);
                this.ObjectsManager.MainCamera.Shake(0.065f);
                this.ObjectsManager.MainCamera.Damage();
                if (freeze) this.ObjectsManager.PauseManager.QuickPause(0.065f);

                // foreach (AMB_PassiveSkill passiveSkill in this.PassiveSkills) {
                //     passiveSkill.ApplyOnDamageReceived(dealer: from, receiver: this, damageSource: damageSource);
                // }

                return damageTaken;
            }
        }

        #region Enhancements
        public bool CanAddEnhancement(AMB_Enhancement enhancement) {
            if (this.Enhancements.Count < DEFAULT_MAX_ENHANCEMENTS) {
                return true;
            } else {
                AMB_Enhancement existingEnhancement =
                    this.Enhancements.Value.Find(e => e.GetType() == enhancement.GetType() && !e.IsMaxLevel);
                return existingEnhancement != null;
            }
        }

        public MB_Enhancement AddEnhancement(AMB_Enhancement enhancement) {
            if (!this.CanAddEnhancement(enhancement)) {
                Debug.Log($"Cannot add enhancement {enhancement}");
                return null;
            }

            AMB_Enhancement existingEnhancement = this.Enhancements.Value.Find(e => e.GetType() == enhancement.GetType() && !e.IsMaxLevel);
            if (existingEnhancement == null) {
                MB_Enhancement uiEnhancement = this.ObjectsManager.PlayerHUD.AddEnhancement(enhancement);
                if (uiEnhancement != null) {
                    enhancement.transform.SetParent(this.EnhancementsParent);
                    this.Enhancements.Add(enhancement);
                }

                return uiEnhancement;
            } else {
                existingEnhancement.Level += enhancement.Level;
                this.ObjectsManager.PlayerHUD.UpdateEnhancement(existingEnhancement);

                return null;
            }
        }
        #endregion

        #region Spells
        protected bool CanUseMainSpell() => Time.time >= this.MainSpellAvailableAt;

        protected virtual bool UseMainSpell() {
            if (!this.CanUseMainSpell()) return false;

            AMB_Spell spell = this.m_TEMP_MainSpell switch {
                // AMB_DirectionalSpell directionalSpell => this.UseSpell(directionalSpell),
                AMB_RotatingSpell rotatingSpell => this.UseSpell(rotatingSpell),
                AMB_PositionalSpell positionalSpell => this.UseSpell(positionalSpell, this.PlayerController.AimPosition),
                _ => null
            };

            if (spell == null) return false;

            spell.SetLevel(0);

            // this.ObjectsManager.Stats.AddMainSkillUsed(this.MainSkill);

            float cooldown = spell.Cooldown; // / (1 + this.CharacterStats.AdditionalAttackSpeed);
            float cooldownModifier = this.AllEffects.Aggregate(0f, (acc, effect) => acc + effect.GetCooldownModifier(this));
            cooldown /= 1 + cooldownModifier;

            this.MainSpellCastAt = Time.time;
            this.MainSpellAvailableAt = this.MainSpellCastAt + cooldown;
            // this.Until(this.CanUseMainSpell, this.OnMainSpellAvailable);

            return true;
        }

        protected bool CanUseSecondarySpell() => Time.time >= this.SecondarySpellAvailableAt;

        protected virtual bool UseSecondarySpell() {
            if (!this.CanUseSecondarySpell()) return false;

            AMB_Spell spell = this.m_TEMP_SecondarySpell switch {
                // AMB_DirectionalSpell directionalSpell => this.UseSpell(directionalSpell),
                AMB_RotatingSpell rotatingSpell => this.UseSpell(rotatingSpell),
                AMB_PositionalSpell positionalSpell => this.UseSpell(positionalSpell, this.PlayerController.AimPosition),
                _ => null
            };

            if (spell == null) return false;

            spell.SetLevel(0);

            // this.ObjectsManager.Stats.AddSecondarySkillUsed(this.SecondarySkill);

            float cooldown = spell.Cooldown; // / (1 + this.CharacterStats.AdditionalAttackSpeed);

            this.SecondarySpellCastAt = Time.time;
            this.SecondarySpellAvailableAt = this.SecondarySpellCastAt + cooldown;
            // this.Until(this.CanUseSecondarySpell, this.OnSecondarySpellAvailable);

            return true;
        }
        #endregion
    }
}
