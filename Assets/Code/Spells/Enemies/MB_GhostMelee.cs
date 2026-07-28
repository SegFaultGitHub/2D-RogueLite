namespace Code.Spells.Enemies {
    public class MB_GhostMelee : AMB_FollowingSpell {
        #region Members
        // [Foldout("MB_GhostMelee", true)]
        #endregion

        #region Getters / Setters
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Start() {
            base.Start();
            this.ObjectsManager.AudioManager.PlayGhostAttack();
        }
        #endregion
    }
}
