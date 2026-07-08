using System;
using System.Collections;
using Code.Characters.Enemies;
using Code.Utils;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Characters.AI {
    public class MB_BatAI : AMB_AI {
        #region Members
        [Foldout("MB_BatAI", true)]
        [SerializeField] private protected float m_AggressiveRange;
        [SerializeField] private protected float m_AttackRange;

        [SerializeField] private protected C_MovementBehaviour m_IdleMovementBehaviour;
        [SerializeField] private protected C_MovementBehaviour m_AggressiveMovementBehaviour;
        [SerializeField] private protected C_MovementBehaviour m_FleeingMovementBehaviour;

        [SerializeField] private protected float m_FocusDuration;
        [SerializeField] private protected RangedFloat m_FleeDuration;

        [SerializeField] private protected GameObject m_Angry;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_Bat m_Bat;

        [ReadOnly][SerializeField] private protected bool m_Focusing;
        #endregion

        #region Getters / Setters
        private float AggressiveRange { get => this.m_AggressiveRange; }
        private float AttackRange { get => this.m_AttackRange; }

        private C_MovementBehaviour IdleMovementBehaviour { get => this.m_IdleMovementBehaviour; }
        private C_MovementBehaviour AggressiveMovementBehaviour { get => this.m_AggressiveMovementBehaviour; }
        private C_MovementBehaviour FleeingMovementBehaviour { get => this.m_FleeingMovementBehaviour; }

        private float FocusDuration { get => this.m_FocusDuration; }
        private RangedFloat FleeDuration { get => this.m_FleeDuration; }

        private GameObject Angry { get => this.m_Angry; }

        private bool Focusing { get => this.m_Focusing; set => this.m_Focusing = value; }

        private MB_Bat Bat { get => this.m_Bat; set => this.m_Bat = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.Bat = this.GetComponent<MB_Bat>();
        }
        #endregion

        protected override void UpdateBehaviour() {
            switch (this.Behaviour) {
                case E_Behaviour.Fleeing:
                    return;
                case E_Behaviour.Aggressive:
                    if (!this.Focusing && this.DistanceToPlayer <= this.AttackRange && this.Bat.CanUseSpell()) {
                        this.Decision.MovementDirection *= 0;
                        this.Focusing = true;
                        this.Bat.Focus(true);
                        this.InSeconds(
                            this.FocusDuration,
                            () => {
                                this.Bat.Focus(false);
                                this.Bat.UseSpell();
                                this.Focusing = false;
                                this.SetBehaviour(E_Behaviour.Fleeing, true);
                            }
                        );
                    }

                    return;
                case E_Behaviour.Idle:
                default:
                    if (this.DistanceToPlayer <= this.AggressiveRange && Time.time - this.EnabledAt >= AGGRESSIVE_DELAY)
                        this.SetBehaviour(E_Behaviour.Aggressive, false);
                    return;
            }
        }

        public override void SetBehaviour(E_Behaviour behaviour, bool propagateAggressive) {
            base.SetBehaviour(behaviour, propagateAggressive);

            this.Angry.SetActive(behaviour == E_Behaviour.Aggressive);

            if (this.Behaviour != E_Behaviour.Fleeing && behaviour == E_Behaviour.Fleeing) {
                this.StartCoroutine(this.Flee());
            }

            this.OnNewBehaviour(behaviour, E_Behaviour.Aggressive, this.Enemy.PlayHopAnimation);

            this.Behaviour = behaviour;
        }

        protected override Vector2 GetMovementDirection() {
            return this.Behaviour switch {
                E_Behaviour.Idle => this.GetDirectionToPlayer(
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
                ),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override Vector2 GetAimDirection() => this.Decision.MovementDirection;

        private IEnumerator Flee() {
            float fleeingDuration = Random.Range(this.FleeDuration.Min, this.FleeDuration.Max);

            yield return new WaitForSeconds(fleeingDuration);

            this.SetBehaviour(E_Behaviour.Aggressive, false);
        }
    }
}
