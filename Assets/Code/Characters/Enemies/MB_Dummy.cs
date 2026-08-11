using Code.Utils;

namespace Code.Characters.Enemies {
    public class MB_Dummy : AMB_Enemy {
        #region Members
        // [Foldout("MB_Dummy", true)]
        #endregion

        #region Getters / Setters
        public override E_Enemy Enemy { get => E_Enemy.Dummy; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();

            this.FullHeal();
        }
        #endregion

        private void FullHeal() {
            this.InSeconds(5, () => {
                    this.Heal(this, this.CharacterStats.MissingHealth);
                    this.FullHeal();
                }
            );
        }
    }
}
