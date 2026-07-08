using Code.Characters.AI;
using Code.Characters.Players;
using Code.Spells;
using Code.Spells.Enemies;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Characters.Enemies {
    public class MB_Ghost : AMB_Enemy {
        #region Members
        [Foldout("MB_Ghost", true)]
        [SerializeField] private SpriteRenderer m_Sprite;
        [SerializeField] private SpriteRenderer m_SilhouetteSprite;
        
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected float m_SpellCastAt;
        [ReadOnly][SerializeField] private protected float m_SpellAvailableAt;
        #endregion

        #region Getters / Setters
        private SpriteRenderer Sprite { get => this.m_Sprite; }
        private SpriteRenderer SilhouetteSprite { get => this.m_SilhouetteSprite; }
        
        private float SpellCastAt { get => this.m_SpellCastAt; set => this.m_SpellCastAt = value; }
        private float SpellAvailableAt { get => this.m_SpellAvailableAt; set => this.m_SpellAvailableAt = value; }

        public override E_Enemy Enemy { get => E_Enemy.Ghost; }
        private Sequence TransparentTween { get; set; }
        #endregion

        #region Static / Readonly / Const
        private const float TRANSPARENT_DURATION = 0.5f;
        private const float TRANSPARENT_ALPHA = 0.25f;
        #endregion

        #region Unity methods
        #endregion

        public bool CanUseSpell() => this.SpellAvailableAt <= Time.time;

        public void SetTransparent() {
            if (this.TransparentTween != null) DOTween.Kill(this.TransparentTween);
            
            this.TransparentTween = DOTween.Sequence();
            this.TransparentTween.Join(this.Sprite.DOColor(
                new Color(1f, 1f, 1f, TRANSPARENT_ALPHA),
                TRANSPARENT_DURATION
            ));
            this.TransparentTween.Join(this.SilhouetteSprite.DOColor(
                new Color(1f, 1f, 1f, TRANSPARENT_ALPHA),
                TRANSPARENT_DURATION
            ));

            this.BaseController.DisableCollisions();
        }

        public void SetVisible() {
            if (this.TransparentTween != null) DOTween.Kill(this.TransparentTween);
            
            this.TransparentTween = DOTween.Sequence();
            this.TransparentTween.Join(this.Sprite.DOColor(
                new Color(1f, 1f, 1f, 1f),
                TRANSPARENT_DURATION
            ));
            this.TransparentTween.Join(this.SilhouetteSprite.DOColor(
                new Color(1f, 1f, 1f, 1f),
                TRANSPARENT_DURATION
            ));
            
            this.BaseController.EnableCollisions();
        }
    }
}
