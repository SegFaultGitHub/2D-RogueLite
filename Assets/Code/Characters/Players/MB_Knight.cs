using MyBox;
using UnityEngine;

namespace Code.Characters.Players {
    public class MB_Knight : AMB_Player {
        #region Members
        [Foldout("MB_Knight", true)]
        [SerializeField] private protected Transform m_Sword;
        [SerializeField] private protected Transform m_SwordBox;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected bool m_SwordDirection;
        #endregion

        #region Getters / Setters
        private Transform Sword { get => this.m_Sword; }
        private Transform SwordBox { get => this.m_SwordBox; }

        public bool SwordDirection { get => this.m_SwordDirection; set => this.m_SwordDirection = value; }
        #endregion

        #region Static / Readonly / Const
        private const float SWORD_ANGLE = 135;
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.ShowSword();
        }
        #endregion

        public void HideSword() {
            this.Sword.gameObject.SetActive(false);
        }

        public void ShowSword() {
            this.SwordBox.localEulerAngles = new Vector3(0, 0, this.SwordDirection ? SWORD_ANGLE : -SWORD_ANGLE);
            this.Sword.gameObject.SetActive(true);
        }

        public override void Aim(Vector2 direction) {
            this.Sword.up = direction;
        }

        protected override void Kill(AMB_Character character) {
            // throw new System.NotImplementedException();
        }

        protected override void Die(AMB_Character killedBy) {
            // throw new System.NotImplementedException();
        }

        protected override void PlayHurtSoundEffect() => this.ObjectsManager.AudioManager.PlayPlayerHurt();
    }
}
