using MyBox;
using UnityEngine;

namespace Code.Characters.Enemies {
    public class MB_SmallSlime : AMB_Slime {
        #region Members
        // [Foldout("MB_SmallSlime", true)]
        #endregion

        #region Getters / Setters
        public override E_Enemy Enemy { get => E_Enemy.SmallSlime; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.SetInvulnerable();
        }
        #endregion
    }
}
