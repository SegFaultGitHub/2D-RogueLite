namespace Code.Spells.Player {
    public class MB_ViolentDash : AMB_FollowingSpell {
        #region Members
        // [Foldout("MB_ViolentDash", true)]
        #endregion

        #region Getters / Setters
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void SetData(Enhancements.MB_ViolentDash.C_EnhancementData violentDashData) {
            this.HitMetadata.Damage = violentDashData.Damage;
            this.HitMetadata.KnockbackForce = violentDashData.KnockbackForce;
        }
    }
}
