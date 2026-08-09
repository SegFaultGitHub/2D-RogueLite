using System.Collections.Generic;
using System.Linq;
using Code.Characters.Controllers.Players;
using Code.Characters.Enemies;
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

        [ReadOnly][SerializeField] private protected int m_MaxEnhancements;
        [ReadOnly][SerializeField] private protected CollectionWrapperList<AMB_Enhancement> m_Enhancements;

        [Separator("TEMP")]
        [SerializeField] private protected AMB_Spell m_TEMP_MainSpell;
        [SerializeField] private protected AMB_Spell m_TEMP_SecondarySpell;

        [SerializeField] private protected AMB_Enhancement[] m_TEMP_Enhancements;
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

        private int MaxEnhancements { get => this.m_MaxEnhancements; set => this.m_MaxEnhancements = value; }
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
        private const float DASH_DURATION = .15f;
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
            this.MaxEnhancements = DEFAULT_MAX_ENHANCEMENTS;
        }

        public override void PostInitialize() {
            this.ObjectsManager.PlayerHUD.SetHealth(this.CharacterStats.CurrentHealth, this.CharacterStats.MaxHealth);

            this.InUpdates(
                1,
                () => {
                    foreach (AMB_Enhancement enhancement in this.m_TEMP_Enhancements) {
                        AMB_Enhancement e = Instantiate(enhancement);
                        e.Level = e.MaxLevel - 1;
                        this.AddEnhancement(e);

                        // AMB_Enhancement ne = Instantiate(enhancement);
                        // ne.Level = ne.MaxLevel;
                        //
                        // if (this.CanAddEnhancement(ne)) {
                        //     AMB_Enhancement ee = this.GetUpgradableEnhancement(ne);
                        //     MB_EnhancementChoice ec = Instantiate(this.m_TEMP_EnhancementChoicePrefab, this.m_TEMP_HUDCanvas);
                        //     ec.SetEnhancement(ne, ee);
                        // } else {
                        //     Destroy(ne.gameObject);
                        // }
                    }
                }
            );
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
            this.PlayerController.Dash(DASH_DURATION, disableCharacterColliders: true, disableSpellColliders: true);
            this.ObjectsManager.StatsManager.AddDash();
        }

        protected override void Kill(AMB_Character character) {
            base.Kill(character);
            if (character is AMB_Enemy enemy) this.ObjectsManager.StatsManager.AddKilled(enemy);
        }

        protected override bool Die(AMB_Character killedBy) {
            bool alreadyDead = base.Die(killedBy);

            if (!alreadyDead && killedBy is AMB_Enemy enemy) {
                this.ObjectsManager.StatsManager.AddKilledBy(enemy);
            }

            return alreadyDead;
        }

        public override int TakeDamage(
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
                int damageTaken = base.TakeDamage(becomeInvulnerable, freeze, value, critical, from, damageSource);

                if (damageTaken == 0) return 0;

                this.ObjectsManager.StatsManager.AddDamageReceived(from as AMB_Enemy, damageTaken, damageSource);

                this.ObjectsManager.PlayerHUD.SetHealth(this.CharacterStats.CurrentHealth, this.CharacterStats.MaxHealth);
                this.ObjectsManager.MainCamera.Shake(0.065f);
                this.ObjectsManager.MainCamera.Damage();
                if (freeze) this.ObjectsManager.PauseManager.QuickPause(0.195f);

                return damageTaken;
            }
        }

        protected override void DealtDamage(int damageDealt, AMB_Character character, E_DamageSource source) {
            base.DealtDamage(damageDealt, character, source);

            if (character is AMB_Enemy enemy) {
                this.ObjectsManager.StatsManager.AddDamageDealt(enemy, damageDealt, source);
            }
        }

        public override float Heal(AMB_Character from, float value) {
            if (value == 0) {
                return 0;
            } else {
                float healReceived = base.Heal(from, value);

                if (healReceived == 0) return 0;

                this.ObjectsManager.PlayerHUD.SetHealth(this.CharacterStats.CurrentHealth, this.CharacterStats.MaxHealth);
                this.ObjectsManager.MainCamera.Heal();

                return healReceived;
            }
        }

        #region Enhancements
        public void AddEnhancementSlot() {
            this.MaxEnhancements++;
            this.ObjectsManager.PlayerHUD.AddEnhancementSlot();
        }

        public bool CanAddEnhancement(AMB_Enhancement enhancement) {
            if (this.Enhancements.Count < this.MaxEnhancements) {
                return true;
            } else {
                AMB_Enhancement existingEnhancement =
                    this.Enhancements.Value.Find(e => e.Enhancement == enhancement.Enhancement && !e.IsMaxLevel);
                return existingEnhancement != null;
            }
        }

        public AMB_Enhancement GetUpgradableEnhancement(AMB_Enhancement enhancement) {
            return this.Enhancements.Value.Find(e => e.Enhancement == enhancement.Enhancement && !e.IsMaxLevel);
        }

        public void AddEnhancement(AMB_Enhancement enhancement) {
            if (!this.CanAddEnhancement(enhancement)) {
                Debug.Log($"Cannot add enhancement {enhancement}");
                return;
            }

            int _GetEnhancementCount() => this.Enhancements.Value.Count(e => e.Enhancement == enhancement.Enhancement);

            AMB_Enhancement existingEnhancement = this.GetUpgradableEnhancement(enhancement);
            if (existingEnhancement == null) {
                MB_Enhancement uiEnhancement = this.ObjectsManager.PlayerHUD.AddEnhancement(enhancement);
                if (uiEnhancement != null) {
                    enhancement.transform.SetParent(this.EnhancementsParent);
                    this.Enhancements.Add(enhancement);
                }

                enhancement.OnNew(this);
                this.ObjectsManager.StatsManager.AddEnhancementTaken( //
                    enhancement,
                    enhancement.EffectiveLevel,
                    _GetEnhancementCount(),
                    enhancement.IsMaxLevel
                );
            } else {
                int previousLevel = existingEnhancement.EffectiveLevel;
                existingEnhancement.Level += enhancement.Level;
                Destroy(enhancement.gameObject);
                this.ObjectsManager.PlayerHUD.UpdateEnhancement(existingEnhancement);

                existingEnhancement.OnUpgrade(this, previousLevel);
                this.ObjectsManager.StatsManager.AddEnhancementTaken(
                    existingEnhancement,
                    existingEnhancement.EffectiveLevel,
                    _GetEnhancementCount(),
                    existingEnhancement.IsMaxLevel
                );
            }
        }
        #endregion

        protected override void PlayHurtFromDamageOverTimeSoundEffect() =>
            this.ObjectsManager.AudioManager.PlayPlayerHurtFromDamageOverTime();

        protected override void PlayDashSoundEffect() => this.ObjectsManager.AudioManager.PlayPlayerDash();

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
