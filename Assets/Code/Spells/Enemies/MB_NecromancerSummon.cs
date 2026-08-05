namespace Code.Spells.Enemies {
    public class MB_NecromancerSummon : AMB_Summon {
        #region Members
        // [Foldout("MB_NecromancerSummon", true)]
        #endregion

        #region Getters / Setters
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Start() {
            base.Start();
            this.ObjectsManager.AudioManager.PlayNecromancerSummon();
        }
        #endregion
    }
}
