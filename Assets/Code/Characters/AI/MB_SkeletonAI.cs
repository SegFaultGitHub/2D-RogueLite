using System;
using System.Collections;
using Code.Characters.Enemies;
using Code.Utils;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Characters.AI {
    public class MB_SkeletonAI : AMB_AI {
        #region Members
        [Foldout("MB_SkeletonAI", true)]
        [SerializeField] private protected float m_AggressiveRange;
        [SerializeField] private protected float m_AttackRange;

        [SerializeField] private protected C_MovementBehaviour m_IdleMovementBehaviour;
        [SerializeField] private protected C_MovementBehaviour m_AggressiveMovementBehaviour;
        [SerializeField] private protected C_MovementBehaviour m_FleeingMovementBehaviour;

        [SerializeField] private protected float m_FocusDuration;
        [SerializeField] private RangedFloat m_RestDuration;
        [SerializeField] private RangedFloat m_StrideDuration;

        [SerializeField] private protected GameObject m_Angry;

        [SerializeField] private protected float m_FleeingSpeedRatio;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_Skeleton m_Skeleton;

        [ReadOnly][SerializeField] private protected bool m_Focusing;
        [ReadOnly][SerializeField] private protected bool m_Resting;
        [ReadOnly][SerializeField] private protected float m_StartStride;
        [ReadOnly][SerializeField] private protected float m_EndStride;
        [ReadOnly][SerializeField] private protected float m_CurrentStrideDuration;
        #endregion

        #region Getters / Setters
        private float AggressiveRange { get => this.m_AggressiveRange; }
        private float AttackRange { get => this.m_AttackRange; }

        private C_MovementBehaviour IdleMovementBehaviour { get => this.m_IdleMovementBehaviour; }
        private C_MovementBehaviour AggressiveMovementBehaviour { get => this.m_AggressiveMovementBehaviour; }
        private C_MovementBehaviour FleeingMovementBehaviour { get => this.m_FleeingMovementBehaviour; }

        private float FocusDuration { get => this.m_FocusDuration; }
        private RangedFloat RestDuration { get => this.m_RestDuration; }
        private RangedFloat StrideDuration { get => this.m_StrideDuration; }

        private GameObject Angry { get => this.m_Angry; }

        private float FleeingSpeedRatio { get => this.m_FleeingSpeedRatio; }

        private MB_Skeleton Skeleton { get => this.m_Skeleton; set => this.m_Skeleton = value; }

        private bool Focusing { get => this.m_Focusing; set => this.m_Focusing = value; }
        private bool Resting { get => this.m_Resting; set => this.m_Resting = value; }
        private float StartStride { get => this.m_StartStride; set => this.m_StartStride = value; }
        private float EndStride { set => this.m_EndStride = value; }
        private float CurrentStrideDuration { get => this.m_CurrentStrideDuration; set => this.m_CurrentStrideDuration = value; }

        private Coroutine MovingCoroutine { get; set; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.Skeleton = this.GetComponent<MB_Skeleton>();
        }

        protected override void OnEnable() {
            base.OnEnable();
            this.Focusing = false;
            if (this.MovingCoroutine != null) this.StopCoroutine(this.MovingCoroutine);
            this.MovingCoroutine = this.StartCoroutine(
                SC_Utils.Rate(.5f)
                    ? this.Rest()
                    : this.Stride()
            );
        }
        #endregion

        protected override void UpdateBehaviour() {
            if (this.Behaviour == E_Behaviour.Aggressive) {
                if (!this.Focusing
                    && this.DistanceToPlayer <= this.AttackRange
                    && this.Skeleton.CanUseSpell()
                    && !this.WallsBlockingSight) {
                    this.Decision.MovementDirection *= 0;
                    this.Focusing = true;
                    this.Skeleton.Focus(true);
                    this.InSeconds(
                        this.FocusDuration,
                        () => {
                            this.Skeleton.Focus(false);
                            this.Focusing = false;
                            this.Skeleton.UseSpell();
                            this.SetBehaviour(E_Behaviour.Fleeing, false);
                            this.Until(this.Skeleton.CanUseSpell, this.StopFleeing);
                        }
                    );
                }

                return;
            }

            if (this.Behaviour != E_Behaviour.Fleeing
                && this.DistanceToPlayer <= this.AggressiveRange
                && Time.time - this.EnabledAt >= AGGRESSIVE_DELAY)
                this.SetBehaviour(E_Behaviour.Aggressive, false);
        }

        private void StopFleeing() {
            this.Angry.SetActive(true);
            this.Behaviour = E_Behaviour.Aggressive;
        }

        public override void SetBehaviour(E_Behaviour behaviour, bool propagateAggressive) {
            base.SetBehaviour(behaviour, propagateAggressive);

            this.Angry.SetActive(behaviour == E_Behaviour.Aggressive);
            this.OnNewBehaviour(behaviour, E_Behaviour.Aggressive, this.Enemy.PlayHopAnimation);
            this.Behaviour = behaviour;
        }

        protected override Vector2 GetMovementDirection() {
            return this.Behaviour switch {
                E_Behaviour.Idle => this.Resting
                    ? Vector2.zero
                    : this.GetDirectionToPlayer(
                        this.IdleMovementBehaviour.PlayerAttraction,
                        this.IdleMovementBehaviour.NoiseWeight,
                        this.IdleMovementBehaviour.ObstaclesRepulsion
                    ),
                E_Behaviour.Aggressive => this.Focusing
                    ? Vector2.zero
                    : this.GetDirectionToPlayer(
                        this.AggressiveMovementBehaviour.PlayerAttraction,
                        this.AggressiveMovementBehaviour.NoiseWeight,
                        this.AggressiveMovementBehaviour.ObstaclesRepulsion
                    ),
                E_Behaviour.Fleeing => this.GetDirectionToPlayer(
                                           this.FleeingMovementBehaviour.PlayerAttraction,
                                           this.FleeingMovementBehaviour.NoiseWeight,
                                           this.FleeingMovementBehaviour.ObstaclesRepulsion
                                       )
                                       * this.FleeingSpeedRatio,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override Vector2 GetAimDirection() {
            return this.Focusing
                ? this.TrueVectorToPlayer
                : this.Decision.MovementDirection;
        }

        private IEnumerator Rest() {
            this.Resting = true;
            yield return new WaitForSeconds(Random.Range(this.RestDuration.Min, this.RestDuration.Max));

            // After resting, always start moving again
            if (this.MovingCoroutine != null) this.StopCoroutine(this.MovingCoroutine);
            this.MovingCoroutine = this.StartCoroutine(this.Stride());
        }

        private IEnumerator Stride() {
            this.Resting = false;
            this.StartStride = Time.time;
            this.CurrentStrideDuration = Random.Range(this.StrideDuration.Min, this.StrideDuration.Max);
            this.EndStride = this.StartStride + this.CurrentStrideDuration;

            // Stride for the specified amount of time
            yield return new WaitForSeconds(this.CurrentStrideDuration);

            if (this.MovingCoroutine != null) this.StopCoroutine(this.MovingCoroutine);
            this.MovingCoroutine = this.Behaviour switch {
                E_Behaviour.Idle => this.StartCoroutine(this.Rest()),
                E_Behaviour.Aggressive => this.StartCoroutine(this.Stride()),
                E_Behaviour.Fleeing => this.StartCoroutine(this.Stride()),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
