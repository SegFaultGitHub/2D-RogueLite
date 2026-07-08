using MyBox;
using UnityEngine;

namespace Code.UI.HUD {
    public class MB_EnemyIndicator : MonoBehaviour {
        #region Members
        [Foldout("MB_EnemyIndicator", true)]
        [SerializeField] private protected Transform m_Image;
        #endregion

        #region Getters / Setters
        private Transform Image { get => this.m_Image; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void SetAngle(float angle) => this.Image.localEulerAngles = new Vector3(0, 0, angle);
    }
}
