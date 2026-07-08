using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Characters.Enemies {
    public class MB_BigSlime : AMB_Slime {
        #region Members
        [Foldout("MB_BigSlime", true)]
        [SerializeField] private protected MB_SmallSlime m_SmallSlimePrefab;
        #endregion

        #region Getters / Setters
        private MB_SmallSlime SmallSlimePrefab { get => this.m_SmallSlimePrefab; }

        public override E_Enemy Enemy { get => E_Enemy.BigSlime; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        protected override void Die(AMB_Character killedBy) {
            for (int i = 0; i < 2; i++) {
                MB_SmallSlime slime = Instantiate(this.SmallSlimePrefab, this.transform.parent);
                slime.transform.position = this.Center.position;
                slime.Wave = this.Wave;
            }

            base.Die(killedBy);
        }
    }
}
