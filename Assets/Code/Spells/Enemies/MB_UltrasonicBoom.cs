using Code.Characters;
using Code.Characters.Effects;
using Code.Managers;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Spells.Enemies {
    public class MB_UltrasonicBoom : AMB_PositionalSpell {
        #region Members
        [Foldout("MB_UltrasonicBoom", true)]
        [SerializeField] private protected float m_Duration;
        [SerializeField] private protected float m_Size;
        [SerializeField] private protected CircleCollider2D m_InnerCircleCollider;
        [SerializeField] private protected CircleCollider2D m_OuterCircleCollider;
        [SerializeField] private protected MB_Confused m_Confused;
        #endregion

        #region Getters / Setters
        private float Duration { get => this.m_Duration; }
        private float Size { get => this.m_Size; }
        private CircleCollider2D InnerCircleCollider { get => this.m_InnerCircleCollider; }
        private CircleCollider2D OuterCircleCollider { get => this.m_OuterCircleCollider; }
        private MB_Confused Confused { get => this.m_Confused; }
        #endregion

        #region Static / Readonly / Const
        private const float SHOCKWAVE_DURATION = 2f;
        #endregion

        #region Unity methods
        protected override void Start() {
            base.Start();

            // Wait for next update because initialization is done after instantiating
            this.InUpdates(
                1,
                () => {
                    this.ObjectsManager.ShockWavesManager.PerformShockWave(
                        worldPosition: this.To,
                        duration: SHOCKWAVE_DURATION,
                        strength: 6f,
                        maxSize: this.Size
                    );
                    DOTween.To( //
                            () => 0,
                            size => this.OuterCircleCollider.radius = size,
                            this.Size,
                            SHOCKWAVE_DURATION
                        )
                        .OnComplete(() => this.OuterCircleCollider.radius = 0)
                        .SetEase(MB_ShockWavesManager.EASE);
                    DOTween.To( //
                            () => 0f,
                            ratio => this.InnerCircleCollider.radius = ratio * this.OuterCircleCollider.radius,
                            1f,
                            SHOCKWAVE_DURATION
                        )
                        .OnComplete(() => this.InnerCircleCollider.radius = 0)
                        .SetEase(MB_ShockWavesManager.EASE);
                    this.InSeconds(SHOCKWAVE_DURATION * .8f, () => this.InnerCircleCollider.enabled = false);
                    this.InSeconds(SHOCKWAVE_DURATION * .8f, () => this.OuterCircleCollider.enabled = false);
                    this.InSeconds(SHOCKWAVE_DURATION, () => this.Destroyer.Destroy());
                }
            );
        }
        #endregion

        public override void TriggerEnter(AMB_Character character) {
            E_SpellCollisionFlag flag = base.Collide(character);
            if (flag is not E_SpellCollisionFlag.Character and not E_SpellCollisionFlag.CharacterInvulnerable) return;

            character.Knockback(character.transform.position.ToVector2() - this.To, this.HitMetadata.KnockbackForce);
            character.AddEffect(this.Confused, this.Character, this.Duration);
        }

        public override void TriggerStay(AMB_Character character) { }

        public override void TriggerExit(AMB_Character character) { }
    }
}
