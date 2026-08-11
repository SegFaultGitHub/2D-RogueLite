using MyBox;
using UnityEngine;

namespace Code.Characters.AI {
    public class MB_DummyAI : AMB_AI {
        #region Members
        // [Foldout("MB_DummyAI", true)]
        #endregion

        #region Getters / Setters
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        protected override void UpdateBehaviour() { }
        protected override Vector2 GetMovementDirection() => Vector2.zero;
        protected override Vector2 GetAimDirection() => Vector2.zero;
    }
}
