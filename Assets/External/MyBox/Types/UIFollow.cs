using UnityEngine;

namespace MyBox {
    /// <summary>
    /// Used on object with RectTransform to follow Transform on the scene
    /// </summary>
    [ExecuteInEditMode]
    public class UIFollow : MonoBehaviour {
        public Transform ToFollow;
        /// <summary>
        /// Follow Offset in Units
        /// </summary>
        public Vector2 Offset;
        /// <summary>
        /// Used Camera (Camera.main by default)
        /// </summary>
        public Camera GameCamera;

#pragma warning disable 0649
        [SerializeField, Tooltip("Hide Canvas when Following Panel is offscreen")]
        private bool _hideOffscreen;
#pragma warning restore 0649
        [SerializeField, ConditionalField("_hideOffscreen")]
        private Canvas _canvas;

        [SerializeField] private bool _editTime = true;


        public bool IsOffscreen => this.OffscreenOffset != Vector2.zero;

        private RectTransform Transform =>
            this._transform
                ? this._transform
                : this._transform = this.transform as RectTransform;
        private RectTransform _transform;

        public Vector2 OffscreenOffset {
            get {
                var rect = this.Transform.rect;

                var halfWidth = rect.width / 2;
                var offX = 0f;
                var anchoredPosition = this.Transform.anchoredPosition;
                var minX = anchoredPosition.x + halfWidth;
                var maxX = anchoredPosition.x - halfWidth - Screen.width;
                if (minX < 0) offX = minX;
                else if (maxX > 0) offX = maxX;

                var halfHeight = rect.height / 2;
                var offY = 0f;
                var minY = anchoredPosition.y + halfHeight;
                var maxY = anchoredPosition.y - halfHeight - Screen.height;
                if (minY < 0) offY = minY;
                else if (maxY > 0) offY = maxY;
                return new Vector2(offX, offY);
            }
        }


        private void LateUpdate() {
            if (!this._editTime && !Application.isPlaying) return;

            if (this.ToFollow == null) return;
            if (this.GameCamera == null) {
                this.GameCamera = Camera.main;
                if (this.GameCamera == null) {
                    WarningsPool.LogWarning(this.name + ".UIFollow Caused: Main Camera not found. Assign Camera manually", this);
                    return;
                }
            }

            this.Transform.anchorMax = Vector2.zero;
            this.Transform.anchorMin = Vector2.zero;

            var followPosition = this.ToFollow.position.Offset(this.Offset);
            Vector3 screenspace = this.GameCamera.WorldToScreenPoint(followPosition);
            this.Transform.anchoredPosition = screenspace;

            this.ToggleCanvasOffscreen();
        }

        private void ToggleCanvasOffscreen() {
            if (!this._hideOffscreen) return;
            this._canvas.enabled = !this.IsOffscreen;
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (this._hideOffscreen && this._canvas == null) {
                this._canvas = this.GetComponentInChildren<Canvas>();
                if (this._canvas == null)
                    this._canvas = this.GetComponentInParent<Canvas>();

                Debug.LogError(this.name + " Caused: UIFollow with HideOffscreen cant found Canvas");
            }
        }
#endif
    }
}
