using UnityEngine;

namespace MyBox {
    /// <summary>
    /// Set size of RectTransform by some other RectTransform
    /// </summary>
    [ExecuteAlways]
    public class UISizeBy : MonoBehaviour {
        [MustBeAssigned] public RectTransform CopySizeFrom;

        [Header("CopyWidth/Height, Set optional offset")]
        public OptionalInt CopyWidth = OptionalInt.WithValue(0);
        public OptionalInt CopyHeight = OptionalInt.WithValue(0);

        [Header("Optional Min/Max Width/Height")]
        public OptionalMinMax MinMaxWidth;
        public OptionalMinMax MinMaxHeight;


        private RectTransform _transform;
        private Vector2 _latestSize;

        private void Start() {
            this._transform = this.transform as RectTransform;

            if (this._transform == null) Debug.LogError(this.name + " Caused: Transform is not a RectTransform", this);
            if (!this.CopyWidth.IsSet && !this.CopyHeight.IsSet)
                Debug.LogError(this.name + " Caused: You must set CopyWidth or CopyHeight for UISizeBy to work", this);
        }

        private void LateUpdate() {
            if (this.CopySizeFrom == null) return;
            if (this._transform == null) return;

            var copyFromSize = this.CopySizeFrom.sizeDelta;
            if (this._latestSize == copyFromSize) return;
            this._latestSize = copyFromSize;

            var toSize = this._transform.sizeDelta;
            var x = this.CopyWidth.IsSet
                ? this._latestSize.x + this.CopyWidth.Value
                : toSize.x;
            var y = this.CopyHeight.IsSet
                ? this._latestSize.y + this.CopyHeight.Value
                : toSize.y;

            x = this.MinMaxWidth.GetFixed(x);
            y = this.MinMaxHeight.GetFixed(y);

            this._transform.sizeDelta = new Vector2(x, y);
        }

#if UNITY_EDITOR
		#if ODIN_INSPECTOR
		[Sirenix.OdinInspector.Button]
		#else
        [ButtonMethod]
		#endif
        private void UpdateView() {
            this._latestSize = Vector2.zero;
            UnityEditor.Undo.RecordObject(this._transform, "UISizeBy.UpdateView");
            this.LateUpdate();
        }
#endif
    }
}
