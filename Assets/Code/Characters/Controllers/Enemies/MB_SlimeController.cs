using System;
using Code.Characters.Enemies;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Characters.Controllers.Enemies {
    public class MB_SlimeController : MB_EnemyController {
        #region Members
        [Foldout("MB_SlimeController", true)]
        [SerializeField] private protected Transform m_Sprites;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected AMB_Slime m_Slime;

        [ReadOnly][SerializeField] private protected bool m_Jumping;
        [ReadOnly][SerializeField] private protected bool m_InTheAir;
        #endregion

        #region Getters / Setters
        private Transform Sprites { get => this.m_Sprites; }

        private AMB_Slime Slime { get => this.m_Slime; set => this.m_Slime = value; }

        public bool Jumping { get => this.m_Jumping; private set => this.m_Jumping = value; }
        public bool InTheAir { get => this.m_InTheAir; private set => this.m_InTheAir = value; }
        #endregion

        #region Static / Readonly / Const
        private static readonly int JUMP = Animator.StringToHash("Jump");
        private static readonly int LAND = Animator.StringToHash("Land");
        private static readonly int IDLE = Animator.StringToHash("Idle");
        private static readonly int STRETCH = Animator.StringToHash("Stretch");
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.Slime = this.GetComponent<AMB_Slime>();
        }
        #endregion

        public void StartJump(Vector2 jumpDirection) {
            if (this.Jumping) return;

            float jumpDistance = jumpDirection.magnitude;
            float jumpHeight = jumpDistance / 2;
            float jumpDuration = GetJumpDuration(jumpHeight) * 1.75f;
            this.MovementSpeed = jumpDistance / jumpDuration;

            this.Jumping = true;
            this.Animator.SetTrigger(JUMP);
            this.UntilAnimation(STRETCH, () => this.Jump(jumpHeight));
        }

        private void Jump(float jumpHeight) {
            foreach (Collider2D spellCollider in this.SpellColliders)
                spellCollider.enabled = false;
            foreach (Collider2D environmentCollider in this.EnvironmentColliders)
                environmentCollider.excludeLayers = this.CharactersLayers;

            this.ObjectsManager.AudioManager.PlaySlimeJump();
            this.InTheAir = true;

            DOTween.Sequence()
                .Append(
                    DOTween.To( //
                            () => 0,
                            height => this.Sprites.localPosition = new Vector3(
                                this.Sprites.localPosition.x,
                                height,
                                this.Sprites.localPosition.z
                            ),
                            jumpHeight,
                            GetJumpDuration(jumpHeight)
                        )
                        .SetEase(Ease.OutQuad)
                )
                .Append(
                    DOTween.To( //
                            () => jumpHeight,
                            height => this.Sprites.localPosition = new Vector3(
                                this.Sprites.localPosition.x,
                                height,
                                this.Sprites.localPosition.z
                            ),
                            0,
                            GetJumpDuration(jumpHeight) * .75f
                        )
                        .SetEase(Ease.InQuad)
                )
                .OnComplete(this.Land);
        }

        private void Land() {
            this.ObjectsManager.AudioManager.PlaySlimeLand();
            this.InTheAir = false;

            this.Slime.UseSpell();
            this.Animator.SetTrigger(LAND);
            this.UntilAnimation(IDLE, () => this.Jumping = false);
            foreach (Collider2D spellCollider in this.SpellColliders)
                spellCollider.enabled = true;
            foreach (Collider2D environmentCollider in this.EnvironmentColliders)
                environmentCollider.excludeLayers = 0;
        }

        private static float GetJumpDuration(float jumpHeight) => Mathf.Min(Mathf.Sqrt(jumpHeight / 15), .4f);

        private void UntilAnimation(int shortNameHash, Action action) {
            this.Until(
                () => {
                    AnimatorStateInfo animatorStateInfo = this.Animator.GetCurrentAnimatorStateInfo(0);
                    return animatorStateInfo.shortNameHash == shortNameHash;
                },
                action.Invoke
            );
        }
    }
}
