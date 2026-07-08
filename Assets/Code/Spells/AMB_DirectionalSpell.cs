using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Spells {
    public abstract class AMB_DirectionalSpell : AMB_MovingSpell {
        #region Members
        [Foldout("AMB_DirectionalSpell", true)]
        [SerializeField] private protected float m_Duration;
        [SerializeField] private protected float m_Speed;
        #endregion

        #region Getters / Setters
        private float Duration { get => this.m_Duration; }
        private float Speed { get => this.m_Speed; }
        #endregion

        #region Unity methods
        protected override void Start() {
            base.Start();

            this.InSeconds(this.Duration, () => this.Collide(null, true));
        }

        protected virtual void FixedUpdate() {
            if (!this.Rigidbody) return;
            this.Rigidbody.linearVelocity = this.Direction * this.Speed;
        }
        #endregion

        public void SetDirection(Vector2 direction) {
            this.Direction = direction;
            this.Sprites.right = this.Direction;
        }
    }
}
