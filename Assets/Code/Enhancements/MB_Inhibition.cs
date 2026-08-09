using Code.UI.HUD;

namespace Code.Enhancements {
    public class MB_Inhibition : AMB_Enhancement {
        #region Members
        // [Foldout("MB_Inhibition", true)]
        #endregion

        #region Getters / Setters
        public override E_Enhancement Enhancement { get => E_Enhancement.Inhibition; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override string GetFullDescription() {
            string nothingString = "nothing".NegativeEffect();
            return this.Description.Replace("<nothing>", nothingString);
        }

        public override string GetDescriptionWithUpgrade(int currentLevel, int newLevel) {
            string nothingString = "nothing".NegativeEffect();
            return this.Description.Replace("<nothing>", nothingString);
        }

        public override string GetDescription() {
            string nothingString = "nothing".NegativeEffect();
            return this.Description.Replace("<nothing>", nothingString);
        }
    }
}
