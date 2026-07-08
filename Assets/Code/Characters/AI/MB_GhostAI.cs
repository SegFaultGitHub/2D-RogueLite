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

        [SerializeField] private protected GameObject m_Angry;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_Ghost m_Ghost;
        #endregion

        #region Getters / Setters
        private float AggressiveRange { get => this.m_AggressiveRange; }

        private C_MovementBehaviour IdleMovementBehaviour { get => this.m_IdleMovementBehaviour; }
        private C_MovementBehaviour AggressiveMovementBehaviour { get => this.m_AggressiveMovementBehaviour; }

        private GameObject Angry { get => this.m_Angry; }

        private MB_Ghost Ghost { get => this.m_Ghost; set => this.m_Ghost = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.Ghost = this.GetComponent<MB_Ghost>();
            this.SetVisible();
        }
        #endregion

        protected override void UpdateBehaviour() {
            switch (this.Behaviour) {
                case E_Behaviour.Fleeing:
                    return;
                case E_Behaviour.Aggressive:
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
                E_Behaviour.Aggressive => this.GetDirectionToPlayer(
                    this.AggressiveMovementBehaviour.PlayerAttraction,
                    this.AggressiveMovementBehaviour.NoiseWeight,
                    this.AggressiveMovementBehaviour.ObstaclesRepulsion
                ),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override Vector2 GetAimDirection() => this.Decision.MovementDirection;

        private void SetTransparent() {
            this.Ghost.SetTransparent();
            this.InSeconds(3, this.SetVisible);
        }

        private void SetVisible() {
            this.Ghost.SetVisible();
            this.InSeconds(3, this.SetTransparent);
        }
    }
}
