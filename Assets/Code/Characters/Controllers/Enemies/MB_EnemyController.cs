using Code.Characters.AI;
using MyBox;
using UnityEngine;

namespace Code.Characters.Controllers.Enemies {
    public class MB_EnemyController : AMB_BaseController {
        #region Members
        [Foldout("MB_EnemyController", true)]
        [SerializeField] private protected float m_Lerp;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected AMB_AI m_AI;
        [ReadOnly][SerializeField] private protected float m_LastTurn;
        #endregion

        #region Getters / Setters
        private float Lerp { get => this.m_Lerp; }

        private AMB_AI AI { get => this.m_AI; set => this.m_AI = value; }
        private float LastTurn { get => this.m_LastTurn; set => this.m_LastTurn = value; }
        #endregion

        #region Static / Readonly / Const
        private const float TURN_INTERVAL = .5f;
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.AI = this.GetComponent<AMB_AI>();
            this.LastTurn = float.MinValue;
        }

        protected override void FixedUpdate() {
            if (float.IsNaN(this.MovementDirection.x) || float.IsNaN(this.MovementDirection.y))
                this.MovementDirection = this.AI.Decision.MovementDirection;
            this.MovementDirection = Vector2.Lerp(this.MovementDirection, this.AI.Decision.MovementDirection, this.Lerp);
            this.AimDirection = this.AI.Decision.AimDirection;

            if (Time.time - this.LastTurn > TURN_INTERVAL) {
                switch (this.AimDirection.x) {
                    case > 0.1f:
                        this.Animator.SetInteger(DIRECTION, 0);
                        this.LastTurn = Time.time;
                        break;
                    case < -0.1f:
                        this.Animator.SetInteger(DIRECTION, 1);
                        this.LastTurn = Time.time;
                        break;
                }
            }

            base.FixedUpdate();
        }
        #endregion
    }
}
