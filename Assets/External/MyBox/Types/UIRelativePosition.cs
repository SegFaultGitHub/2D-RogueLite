using UnityEngine;

namespace MyBox {
    /// <summary>
    /// Pivot and Anchor of Target makes no difference.
    /// Current Pivot and "Target Anchor" property used for positioning.
    /// </summary>
    [ExecuteAlways]
    public class UIRelativePosition : MonoBehaviour {
        [MustBeAssigned] public RectTransform Target;

        [Separator("Set X/Y, with optional offset")]
        public OptionalFloat SetX = OptionalFloat.WithValue(0);

        public OptionalFloat SetY = OptionalFloat.WithValue(0);

        [Separator("0-1 point on Target rect")]
        public Vector2 TargetAnchor = new Vector2(.5f, .5f);


        private RectTransform _transform;
        private Vector2 _latestSize;
        private Vector3 _latestPosition;
        private bool _firstCall;

        private void Start() {
            this._transform = this.transform as RectTransform;

            if (this._transform == null) Debug.LogError(this.name + " Caused: Transform is not a RectTransform", this);
            if (!this.SetX.IsSet && !this.SetY.IsSet) Debug.LogError(this.name + " Caused: Check SetX and/or SetY for RelativePosition to work", this);
        }

        private void LateUpdate() {
            if (this._transform == null) return;
            if (this.Target == null) return;
            if (!this._firstCall) {
                // Position is zero on PrefabModeEntered?
                // ForceUpdateRectTransforms is not helping, but on second frame it's all ok
                this._firstCall = true;
                return;
            }

            var relativeToSize = this.Target.sizeDelta;
            var relativeToPosition = this.Target.position;
            if (this._latestSize == relativeToSize && this._latestPosition == relativeToPosition) return;
            this._latestSize = relativeToSize;
            this._latestPosition = relativeToPosition;

            var scale = this.Target.lossyScale;
            var pivot = this.Target.pivot;
            var anchorOffsetX = relativeToSize.x * this.TargetAnchor.x;
            var anchorOffsetY = relativeToSize.y * this.TargetAnchor.y;
            var left = relativeToPosition.x - (relativeToSize.x * pivot.x * scale.x);
            var top = relativeToPosition.y + relativeToSize.y - (relativeToSize.y * pivot.y * scale.y);
            var x = left + anchorOffsetX + this.SetX.Value;
            var y = top - anchorOffsetY + this.SetY.Value;

            var localPosition = this._transform.position;
            var finalPosition = new Vector2(
                this.SetX.IsSet
                    ? (int)x
                    : localPosition.x,
                this.SetY.IsSet
                    ? (int)y
                    : localPosition.y
            );
            this._transform.position = finalPosition;
        }

#if UNITY_EDITOR
        private void OnValidate() {
            this.UpdateView();
        }

        [ButtonMethod]
        private void UpdateView() {
            this._latestSize = Vector2.zero;
            this._transform = this.transform as RectTransform;
            if (this._transform == null) return;

            UnityEditor.Undo.RecordObject(this._transform, "UIRelativePosition.UpdateView");
            this.LateUpdate();
        }
#endif
    }
}
