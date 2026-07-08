using Code.Characters;
using Code.Characters.Effects;
using Code.Managers;
using Code.UI;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Spells.Enemies {
    public class MB_Meteor : AMB_PositionalSpell {
        #region Members
        [Foldout("MB_Meteor", true)]
        [Separator("Spell data")]
        [SerializeField] private protected float m_Size;
        [SerializeField] private protected float m_Delay;
        [SerializeField] private protected float m_DropTime;
        [SerializeField] private protected float m_AngleSpread;

        [Separator("Burning")]
        [SerializeField] private protected MB_Burning m_BurningPrefab;
        [SerializeField] private protected float m_BurningDamagePerTick;
        [SerializeField] private protected int m_BurningTickCount;

        [Separator]
        [SerializeField] private protected Transform m_Sprites;
        [SerializeField] private protected Transform m_Shadow;
        [SerializeField] private protected MB_CircleRenderer m_CircleRenderer;
        [SerializeField] private protected CircleCollider2D m_InnerCircleCollider;
        [SerializeField] private protected CircleCollider2D m_OuterCircleCollider;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Animator m_Animator;
        #endregion

        #region Getters / Setters
        private float Size { get => this.m_Size; }
        private float Delay { get => this.m_Delay; }
        private float DropTime { get => this.m_DropTime; set => this.m_DropTime = value; }
        private float AngleSpread { get => this.m_AngleSpread; }

        private MB_Burning BurningPrefab { get => this.m_BurningPrefab; }
        private float BurningDamagePerTick { get => this.m_BurningDamagePerTick; }
        private int BurningTickCount { get => this.m_BurningTickCount; }

        private Transform Sprites { get => this.m_Sprites; }
        private Transform Shadow { get => this.m_Shadow; }
        private MB_CircleRenderer CircleRenderer { get => this.m_CircleRenderer; }
        private CircleCollider2D InnerCircleCollider { get => this.m_InnerCircleCollider; }
        private CircleCollider2D OuterCircleCollider { get => this.m_OuterCircleCollider; }

        private Animator Animator { get => this.m_Animator; set => this.m_Animator = value; }
        #endregion

        #region Static / Readonly / Const
        private const float METEOR_HEIGHT = 20;
        private static readonly int DESTROY = Animator.StringToHash("Destroy");
        private const float SHOCKWAVE_DURATION = .25f;
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();

            this.Animator = this.GetComponent<Animator>();
        }

        protected override void Start() {
            base.Start();

            this.CharacterCollider.enabled = false;
            Vector2 offsetGoingDown = SC_Utils.Rotate(new Vector2(0, METEOR_HEIGHT), Random.Range(-this.AngleSpread, this.AngleSpread));
            this.Sprites.right = -offsetGoingDown;
            this.Sprites.position = this.To + offsetGoingDown;
            this.Shadow.localScale *= 0;
            // Wait for next update because initialization is done after instantiating
            this.InUpdates(
                1,
                () => {
                    // Run feedback circle
                    this.CircleRenderer.Run(this.Size, this.Delay);

                    // Drop meteor and increase shadow size before circle is fully grown
                    this.InSeconds(
                        this.Delay - this.DropTime,
                        () => {
                            this.Shadow.DOScale(1, this.DropTime).SetEase(Ease.Linear);
                            this.Sprites.DOMove(this.To, this.DropTime).SetEase(Ease.Linear);
                        }
                    );

                    // Wait delay and:
                    // - (1) perform shockwave
                    // - (2) increase colliders size
                    // - (3) trigger explosion animation, disable shadow
                    this.InSeconds(
                        this.Delay,
                        () => {
                            // (1) perform shockwave
                            this.ObjectsManager.ShockWavesManager.PerformShockWave(
                                worldPosition: this.To,
                                duration: SHOCKWAVE_DURATION,
                                strength: 5f,
                                maxSize: this.Size
                            );

                            // (2) increase colliders size
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

                            // (3) trigger explosion animation, disable shadow
                            this.Shadow.gameObject.SetActive(false);
                            this.Sprites.right = Vector2.right;
                            this.Sprites.localScale = SC_Utils.Rate(.5f)
                                ? new Vector2(-1, 1)
                                : new Vector2(1, 1);
                            this.Animator.SetTrigger(DESTROY);
                        }
                    );

                    // Wait for shockwave duration and disable colliders
                    this.InSeconds(this.Delay + SHOCKWAVE_DURATION * .8f, () => this.InnerCircleCollider.enabled = false);
                    this.InSeconds(this.Delay + SHOCKWAVE_DURATION * .8f, () => this.OuterCircleCollider.enabled = false);
                }
            );
        }
        #endregion

        public override void TriggerEnter(AMB_Character character) {
            E_SpellCollisionFlag flag = base.Collide(character);
            if (flag is not E_SpellCollisionFlag.Character and not E_SpellCollisionFlag.CharacterInvulnerable) return;

            MB_Burning burning = Instantiate(this.BurningPrefab);
            burning.SetDamage(this.BurningDamagePerTick, this.BurningTickCount);
            character.AddEffect(burning, this.Character);
            character.Knockback(character.transform.position.ToVector2() - this.To, this.HitMetadata.KnockbackForce);
        }

        public override void TriggerStay(AMB_Character character) { }

        public override void TriggerExit(AMB_Character character) { }
    }
}
