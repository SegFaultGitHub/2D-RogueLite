using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.UI {
    public class MB_Cursor : MonoBehaviour {
        #region Members
        [Foldout("MB_Cursor", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Canvas m_Canvas;
        #endregion

        #region Getters / Setters
        private Canvas Canvas { get => this.m_Canvas; set => this.m_Canvas = value; }

        private PlayerInputs PlayerInputs { get; set; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        private void Awake() {
            Cursor.visible = false;
            this.Canvas = this.GetComponentInParent<Canvas>();
        }

        private void Update() {
            Vector3 mousePosition = this.PlayerInputs.Actions.AimMouse.ReadValue<Vector2>();
            this.transform.localPosition = this.Canvas.ScreenToCanvasPosition(mousePosition);
        }

        protected void OnEnable() {
            this.PlayerInputs = new PlayerInputs();
            this.PlayerInputs.Actions.Enable();
        }

        protected void OnDisable() {
            this.PlayerInputs.Actions.Disable();
        }
        #endregion
    }
}
