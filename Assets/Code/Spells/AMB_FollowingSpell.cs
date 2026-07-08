using MyBox;
using UnityEngine;

namespace Code.Spells {
    public class AMB_FollowingSpell : AMB_MovingSpell {
        #region Members
        [Foldout("AMB_FollowingSpell", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Transform m_Follow;
        [ReadOnly][SerializeField] private protected bool m_StopFollowing;
        #endregion

        #region Getters / Setters
        public Transform Follow { get => this.m_Follow; set => this.m_Follow = value; }
        private bool StopFollowing { get => this.m_StopFollowing; set => this.m_StopFollowing = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected virtual void FixedUpdate() {
            if (this.StopFollowing) return;

            if (this.Follow == null) {
                this.Collide(null, true);
                this.StopFollowing = true;
                this.Destroy();
                return;
            }

            this.transform.position = this.Follow.position;
        }
        #endregion
    }
}
