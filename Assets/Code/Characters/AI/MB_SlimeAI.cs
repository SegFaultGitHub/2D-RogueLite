using System.Collections;
using Code.Characters.Enemies;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Characters.AI {
    public class MB_SlimeAI : AMB_AI {
        #region Members
        [Foldout("MB_SlimeAI", true)]
        [SerializeField] private protected float m_AggressiveRange;
        [SerializeField] private protected RangedFloat m_InitialRestDuration;
        [SerializeField] private protected RangedFloat m_DurationBetweenJumps;

        [SerializeField] private protected GameObject m_Angry;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected AMB_Slime m_Slime;

        [ReadOnly][SerializeField] private protected Vector2 m_MovementDirection;
        #endregion

        #region Getters / Setters
        private float AggressiveRange { get => this.m_AggressiveRange; }
        private RangedFloat InitialRestDuration { get => this.m_InitialRestDuration; }
        private RangedFloat DurationBetweenJumps { get => this.m_DurationBetweenJumps; }

        private GameObject Angry { get => this.m_Angry; }

        private AMB_Slime Slime { get => this.m_Slime; set => this.m_Slime = value; }

        private Vector2 MovementDirection { get => this.m_MovementDirection; set => this.m_MovementDirection = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.Slime = this.GetComponent<AMB_Slime>();
        }

        protected override void OnEnable() {
            base.OnEnable();

            this.InSeconds(
                Random.Range(this.InitialRestDuration.Min, this.InitialRestDuration.Max),
                () => {
                    this.Slime.Jump();
                    this.JumpLoop();
                }
            );
        }
        #endregion

        protected override void UpdateBehaviour() {
            if (this.DistanceToPlayer <= this.AggressiveRange && Time.time - this.EnabledAt >= AGGRESSIVE_DELAY) {
                this.SetBehaviour(E_Behaviour.Aggressive, false);
            }
        }

        public override void SetBehaviour(E_Behaviour behaviour, bool propagateAggressive) {
            base.SetBehaviour(behaviour, propagateAggressive);

            this.Angry.SetActive(behaviour == E_Behaviour.Aggressive);
            this.Behaviour = behaviour;
        }

        protected override Vector2 GetMovementDirection() {
            return this.Slime.SlimeController.InTheAir
                ? this.MovementDirection
                : Vector2.zero;
        }

        protected override Vector2 GetAimDirection() => Vector2.zero;

        public Vector2 GetJumpDirection() {
            if (this.Behaviour == E_Behaviour.Aggressive) {
                Vector2 direction = !this.WallsBlockingSight && !this.HolesBlockingSight
                    ? this.TrueVectorToPlayer
                    : this.VectorToPlayer;
                this.MovementDirection = direction.normalized;
                float distance = Mathf.Min(Random.Range(1f, 2f), direction.magnitude);
                direction = this.MovementDirection * distance;
                return direction;
            } else {
                Vector2 direction = this.GetDirectionToPlayer(0, 1, 3);
                this.MovementDirection = direction.normalized;
                direction = this.MovementDirection * Random.Range(1f, 2f);
                return direction;
            }
        }

        private void JumpLoop() {
            this.InSeconds(
                Random.Range(this.DurationBetweenJumps.Min, this.DurationBetweenJumps.Max),
                () => {
                    this.Slime.Jump();
                    this.Until(() => !this.Slime.SlimeController.Jumping, this.JumpLoop);
                }
            );
        }
    }
}
