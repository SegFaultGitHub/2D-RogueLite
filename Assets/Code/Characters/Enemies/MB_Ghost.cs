using Code.Spells;
using Code.Spells.Enemies;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Characters.Enemies {
    public class MB_Ghost : AMB_Enemy {
        #region Members
        [Foldout("MB_Ghost", true)]
        [SerializeField] private protected MB_GhostMelee m_GhostMelee;

        [SerializeField] private SpriteRenderer m_Sprite;
        [SerializeField] private SpriteRenderer m_SilhouetteSprite;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected float m_SpellCastAt;
        [ReadOnly][SerializeField] private protected float m_SpellAvailableAt;

        [ReadOnly][SerializeField] private protected bool m_InitialKnockbackable;
        #endregion

        #region Getters / Setters
        private MB_GhostMelee GhostMelee { get => this.m_GhostMelee; }

        private SpriteRenderer Sprite { get => this.m_Sprite; }
        private SpriteRenderer SilhouetteSprite { get => this.m_SilhouetteSprite; }

        private float SpellCastAt { get => this.m_SpellCastAt; set => this.m_SpellCastAt = value; }
        private float SpellAvailableAt { get => this.m_SpellAvailableAt; set => this.m_SpellAvailableAt = value; }

        private bool InitialKnockbackable { get => this.m_InitialKnockbackable; set => this.m_InitialKnockbackable = value; }

        public override E_Enemy Enemy { get => E_Enemy.Ghost; }
        private Sequence TransparentTween { get; set; }
        #endregion

        #region Static / Readonly / Const
        private const float TRANSPARENT_DURATION = 0.5f;
        private const float TRANSPARENT_ALPHA = 0.25f;
        private const float DASH_DURATION = .35f;
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();

            this.InitialKnockbackable = this.Knockbackable;
        }
        #endregion

        public bool CanUseSpell() => this.SpellAvailableAt <= Time.time;

        public void Focus(bool focusing) {
            // ReSharper disable once SimplifyConditionalTernaryExpression
            this.Knockbackable = focusing
                ? false
                : this.InitialKnockbackable;
            this.PlayFocusingAnimation(focusing);
        }

        public void UseSpell() {
            if (!this.CanUseSpell()) return;

            AMB_Spell spell = this.UseSpell(this.GhostMelee, this.transform);

            if (spell == null) return;

            this.BaseController.Dash(DASH_DURATION, disableCharacterColliders: true, disableSpellColliders: false);
            this.InSeconds(DASH_DURATION, () => spell.Collide(null, true));
            float cooldown = spell.Cooldown;

            this.SpellCastAt = Time.time;
            this.SpellAvailableAt = this.SpellCastAt + cooldown;
        }

        public void SetTransparent() {
            if (this.TransparentTween != null) DOTween.Kill(this.TransparentTween);

            this.TransparentTween = DOTween.Sequence();
            this.TransparentTween.Join(this.Sprite.DOColor(new Color(1f, 1f, 1f, TRANSPARENT_ALPHA), TRANSPARENT_DURATION));
            this.TransparentTween.Join(this.SilhouetteSprite.DOColor(new Color(1f, 1f, 1f, TRANSPARENT_ALPHA), TRANSPARENT_DURATION));

            this.BaseController.DisableCharacterCollisions();
            this.BaseController.DisableSpellCollisions();
        }

        public void SetVisible() {
            if (this.TransparentTween != null) DOTween.Kill(this.TransparentTween);

            this.TransparentTween = DOTween.Sequence();
            this.TransparentTween.Join(this.Sprite.DOColor(new Color(1f, 1f, 1f, 1f), TRANSPARENT_DURATION));
            this.TransparentTween.Join(this.SilhouetteSprite.DOColor(new Color(1f, 1f, 1f, 1f), TRANSPARENT_DURATION));

            this.BaseController.EnableCharacterCollisions();
            this.BaseController.EnableSpellCollisions();
        }

        protected override void PlayHurtSoundEffect() => this.ObjectsManager.AudioManager.PlayGhostHurt();
    }
}
