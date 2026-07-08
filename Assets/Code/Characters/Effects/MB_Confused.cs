using MyBox;
using UnityEngine;

namespace Code.Characters.Effects {
    public class MB_Confused : AMB_Effect {
        #region Members
        // [Foldout("MB_Confused", true)]
        #endregion

        #region Getters / Setters
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override float ApplyToMovementSpeed(AMB_Character character, float speed) {
            return -Mathf.Abs(speed);
        }
    }
}
