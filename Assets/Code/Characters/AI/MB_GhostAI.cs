using System;
using System.Collections;
using Code.Characters.Enemies;
using Code.Utils;
using MyBox;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Characters.AI {
    public class MB_GhostAI : AMB_AI {
        #region Members
        [Foldout("MB_GhostAI", true)]
        [SerializeField] private protected float m_AggressiveRange;

        [SerializeField] private protected C_MovementBehaviour m_IdleMovementBehaviour;
        [SerializeField] private protected C_MovementBehaviour m_AggressiveMovementBehaviour;
        [SerializeField] private protected float m_AttackRange;

        [SerializeField] private protected float m_FocusDuration;
        [SerializeField] private protected float m_RestDurationAfterSpell;

        [SerializeField] private protected GameObject m_Angry;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_Ghost m_Ghost;

        [ReadOnly][SerializeField] private protected bool m_Focusing;
        [ReadOnly][SerializeField] private protected bool m_RestingFromSpell;
        #endregion

        #region Getters / Setters
        private float AggressiveRange { get => this.m_AggressiveRange; }

        private C_MovementBehaviour IdleMovementBehaviour { get => this.m_IdleMovementBehaviour; }
        private C_MovementBehaviour AggressiveMovementBehaviour { get => this.m_AggressiveMovementBehaviour; }
        private float AttackRange { get => this.m_AttackRange; }

        private float FocusDuration { get => this.m_FocusDuration; }
        private float RestDurationAfterSpell { get => this.m_RestDurationAfterSpell; }

        private GameObject Angry { get => this.m_Angry; }

        private MB_Ghost Ghost { get => this.m_Ghost; set => this.m_Ghost = value; }

        private bool Focusing { get => this.m_Focusing; set => this.m_Focusing = value; }
        private bool RestingFromSpell { get => this.m_RestingFromSpell; set => this.m_RestingFromSpell = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.Ghost = this.GetComponent<MB_Ghost>();
            this.Ghost.SetTransparent();
        }
        #endregion

        protected override void UpdateBehaviour() {
            if (this.Behaviour == E_Behaviour.Aggressive) {
                if (!this.RestingFromSpell && !this.Focusing && this.DistanceToPlayer <= this.AttackRange && this.Ghost.CanUseSpell()) {
                    this.Decision.MovementDirection *= 0;
                    this.Focusing = true;
                    this.Ghost.Focus(true);
                    this.InSeconds(
                        this.FocusDuration,
                        () => {
                            this.Ghost.SetVisible();
                            this.Focusing = false;
                            this.RestingFromSpell = true;
                            this.Ghost.Focus(false);
                            this.Ghost.UseSpell();
                            this.InSeconds(
                                this.RestDurationAfterSpell,
                                () => {
                                    this.RestingFromSpell = false;
                                    this.Ghost.SetTransparent();
                                }
                            );
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
            this.OnNewBehaviour(behaviour, E_Behaviour.Aggressive, this.Enemy.PlayHopAnimation);

            this.Behaviour = behaviour;
        }

        protected override Vector2 GetMovementDirection() {
            return this.Behaviour switch {
                E_Behaviour.Idle => this.GetDirectionToPlayer(
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

        private float GetSpeedMultiplier() {
            return this.Ghost.BaseController.Dashing
                ? 1
                : .5f;
        }

        protected override Vector2 GetAimDirection() {
            return this.Focusing || this.RestingFromSpell
                ? this.TrueVectorToPlayer
                : this.Decision.MovementDirection;
        }
    }
}
