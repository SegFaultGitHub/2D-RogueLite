using System;
using System.Collections;
using Code.Characters.Enemies;
using Code.Utils;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Characters.AI {
    public class MB_ZombieAI : AMB_AI {
        #region Members
        [Foldout("MB_ZombieAI", true)]
        [SerializeField] private protected float m_AggressiveRange;

        [SerializeField] private protected C_MovementBehaviour m_IdleMovementBehaviour;
        [SerializeField] private protected C_MovementBehaviour m_AggressiveMovementBehaviour;
        [SerializeField] private protected float m_AttackRange;

        [SerializeField] private protected float m_FocusDuration;
        [SerializeField] private protected float m_RestDurationAfterSpell;
        [SerializeField] private RangedFloat m_RestDuration;
        [SerializeField] private RangedFloat m_StrideDuration;

        [SerializeField] private protected GameObject m_Angry;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_Zombie m_Zombie;

        [ReadOnly][SerializeField] private protected bool m_Focusing;
        [ReadOnly][SerializeField] private protected bool m_Resting;
        [ReadOnly][SerializeField] private protected float m_StartStride;
        [ReadOnly][SerializeField] private protected float m_EndStride;
        [ReadOnly][SerializeField] private protected float m_CurrentStrideDuration;
        #endregion

        #region Getters / Setters
        private float AggressiveRange { get => this.m_AggressiveRange; }

        private C_MovementBehaviour IdleMovementBehaviour { get => this.m_IdleMovementBehaviour; }
        private C_MovementBehaviour AggressiveMovementBehaviour { get => this.m_AggressiveMovementBehaviour; }
        private float AttackRange { get => this.m_AttackRange; }

        private float FocusDuration { get => this.m_FocusDuration; }
        private float RestDurationAfterSpell { get => this.m_RestDurationAfterSpell; }
        private RangedFloat RestDuration { get => this.m_RestDuration; }
        private RangedFloat StrideDuration { get => this.m_StrideDuration; }

        private GameObject Angry { get => this.m_Angry; }

        private MB_Zombie Zombie { get => this.m_Zombie; set => this.m_Zombie = value; }

        private bool Focusing { get => this.m_Focusing; set => this.m_Focusing = value; }
        private bool Resting { get => this.m_Resting; set => this.m_Resting = value; }
        private float StartStride { get => this.m_StartStride; set => this.m_StartStride = value; }
        private float EndStride { get => this.m_EndStride; set => this.m_EndStride = value; }
        private float CurrentStrideDuration { get => this.m_CurrentStrideDuration; set => this.m_CurrentStrideDuration = value; }

        private Coroutine MovingCoroutine { get; set; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.Zombie = this.GetComponent<MB_Zombie>();
        }

        protected override void OnEnable() {
            base.OnEnable();
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
                if (!this.Focusing && this.DistanceToPlayer <= this.AttackRange && this.Zombie.CanUseSpell()) {
                    this.Decision.MovementDirection *= 0;
                    this.Focusing = true;
                    this.Zombie.Focus(true);
                    this.InSeconds(
                        this.FocusDuration,
                        () => {
                            this.Zombie.Focus(false);
                            this.Zombie.UseSpell();
                            this.InSeconds(this.RestDurationAfterSpell, () => this.Focusing = false);
                        }
                    );
                }
            } else if (this.DistanceToPlayer <= this.AggressiveRange && Time.time - this.EnabledAt >= AGGRESSIVE_DELAY) {
                this.SetBehaviour(E_Behaviour.Aggressive, false);
            }
        }

        public override void SetBehaviour(E_Behaviour behaviour, bool propagateAggressive) {
            base.SetBehaviour(behaviour, propagateAggressive);

            this.Angry.SetActive(behaviour == E_Behaviour.Aggressive);
            if (behaviour == E_Behaviour.Aggressive) { }

            this.OnNewBehaviour(
                behaviour,
                E_Behaviour.Aggressive,
                () => {
                    this.Enemy.PlayHopAnimation();
                    if (this.MovingCoroutine != null) this.StopCoroutine(this.MovingCoroutine);
                    this.StartCoroutine(this.Stride());
                }
            );
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
                      )
                      * this.GetSpeedMultiplier(),
                E_Behaviour.Aggressive => this.Focusing
                    ? Vector2.zero
                    : this.GetDirectionToPlayer(
                          this.AggressiveMovementBehaviour.PlayerAttraction,
                          this.AggressiveMovementBehaviour.NoiseWeight,
                          this.AggressiveMovementBehaviour.ObstaclesRepulsion
                      )
                      * this.GetSpeedMultiplier(),
                E_Behaviour.Fleeing => throw new ArgumentOutOfRangeException(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override Vector2 GetAimDirection() {
            return this.Behaviour switch {
                E_Behaviour.Idle => this.Decision.MovementDirection,
                E_Behaviour.Aggressive => this.TrueVectorToPlayer,
                E_Behaviour.Fleeing => this.Decision.MovementDirection,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private float GetSpeedMultiplier() {
            // Speed is max when the move period just started. 33% of max speed at the very end
            float strideDurationRemaining = this.EndStride - Time.time;
            return SC_Utils.MapFrom(0, this.CurrentStrideDuration, .33f, 1, strideDurationRemaining);
        }

        private IEnumerator Rest() {
            this.Resting = true;
            yield return new WaitForSeconds(Random.Range(this.RestDuration.Min, this.RestDuration.Max));

            // After resting, always start moving again
            this.MovingCoroutine = this.StartCoroutine(this.Stride());
        }

        private IEnumerator Stride() {
            this.Resting = false;
            this.StartStride = Time.time;
            this.CurrentStrideDuration = Random.Range(this.StrideDuration.Min, this.StrideDuration.Max);
            this.EndStride = this.StartStride + this.CurrentStrideDuration;

            // Stride for the specified amount of time
            yield return new WaitForSeconds(this.CurrentStrideDuration);

            switch (this.Behaviour) {
                case E_Behaviour.Idle:
                    // If idle, rest for a bit
                    if (this.MovingCoroutine != null) this.StopCoroutine(this.MovingCoroutine);
                    this.MovingCoroutine = this.StartCoroutine(this.Rest());
                    break;
                case E_Behaviour.Aggressive:
                    // If focusing player, keep moving
                    if (this.MovingCoroutine != null) this.StopCoroutine(this.MovingCoroutine);
                    this.MovingCoroutine = this.StartCoroutine(this.Stride());
                    break;
                case E_Behaviour.Fleeing:
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
