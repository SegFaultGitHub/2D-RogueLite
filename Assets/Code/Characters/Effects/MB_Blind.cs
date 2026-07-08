using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Characters.Effects {
    public class MB_Blind : AMB_Effect {
        #region Members
        [Foldout("MB_Blind", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected float m_FOVSize;
        #endregion

        #region Getters / Setters
        private float FOVSize { get => this.m_FOVSize; set => this.m_FOVSize = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void SetFOVSize(float fovSize) => this.FOVSize = fovSize;

        public override void OnApply(AMB_Character character) {
            base.OnApply(character);
            this.ObjectsManager.FOVManager.SetPlayerFOVSize(this.FOVSize, .5f, Ease.InOutQuint);
        }

        public override void OnRemove(AMB_Character character) {
            base.OnRemove(character);
            this.ObjectsManager.FOVManager.SetDefaultPlayerFOVSize(.5f, Ease.InSine);
        }
    }
}
