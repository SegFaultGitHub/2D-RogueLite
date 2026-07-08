using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Characters.Effects {
    public class MB_ShortSighted : AMB_Effect {
        #region Members
        [Foldout("MB_ShortSighted", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected int m_SightRatio;
        #endregion

        #region Getters / Setters
        private int SightRatio { get => this.m_SightRatio; set => this.m_SightRatio = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void SetSightRatio(int sightRatio) => this.SightRatio = sightRatio;

        public override void OnApply(AMB_Character character) {
            base.OnApply(character);
            this.ObjectsManager.MainCamera.SetPixelSize(8 * this.SightRatio, 1f, Ease.OutQuad);
        }

        public override void OnRemove(AMB_Character character) {
            base.OnRemove(character);
            this.ObjectsManager.MainCamera.SetPixelSize(8, .5f, Ease.InQuad);
        }
    }
}
