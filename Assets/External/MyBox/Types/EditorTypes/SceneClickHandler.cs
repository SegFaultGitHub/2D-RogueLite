#if UNITY_EDITOR
using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;

namespace MyBox.EditorTools {
    /// <summary>
    /// Handles mouse click on scene objects in edit mode.
    /// Set up event, Single Click, Optionally - Cancellable and LayerMask
    /// Enable handler when needed (or call ToggleState)
    /// </summary>
    [PublicAPI]
    public class SceneClickHandler {
        /// <param name="onClick">Event called onClick on the scene</param>
        /// <param name="plane">Plane to read the input</param>
        /// <param name="singleClick">Single click handler will deactivate itself on click</param>
        /// <param name="cancellable">Cancellable handler will listen for Escape key to set Enabled to false</param>
        public SceneClickHandler(Action<Vector3> onClick, Plane plane, bool singleClick = false, bool cancellable = true) : this(
            onClick,
            singleClick,
            cancellable
        ) {
            this._usePlane = true;
            this._plane = plane;
        }

        /// <param name="onClick">Event called onClick on the scene</param>
        /// <param name="singleClick">Single click handler will deactivate itself on click</param>
        /// <param name="cancellable">Cancellable handler will listen for Escape key to set Enabled to false</param>
        /// <param name="physics2d">Use Physics2d raycast instead of Physics</param>
        public SceneClickHandler(Action<Vector3> onClick, bool singleClick = false, bool cancellable = true, bool physics2d = false) {
            this._onClick = onClick;

            this.SingleClickHandler = singleClick;
            this.UsePhysics2D = physics2d;
            this.Cancellable = cancellable;

#pragma warning disable 618
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
#pragma warning restore 618
        }

        ~SceneClickHandler() {
#pragma warning disable 618
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
#pragma warning restore 618
        }


        public bool Enabled {
            private get => this._enabled;
            set {
                if (value)
                    this.FocusSceneView();
                this._enabled = value;
            }
        }

        // We need to focus on SceneView to handle cancellation
        // (OnSceneGUI not handling keyboard events otherwise)
        private void FocusSceneView() {
            if (SceneView.sceneViews.Count == 0) return;
            ((SceneView)SceneView.sceneViews[0]).Focus();
        }

        /// <summary>
        /// Cancellable handler will listen for Escape key to set Enabled to false
        /// </summary>
        public bool Cancellable { get; set; }

        /// <summary>
        /// Single click handler will deactivate itself on click
        /// </summary>
        public bool SingleClickHandler { get; set; }

        public bool UsePhysics2D { get; set; }


        public void SetLayerMask(LayerMask mask) {
            this._useMask = true;
            this._mask = mask;
        }

        public void ToggleState() => this.Enabled = !this.Enabled;

        public Color HandleColor { get; set; } = Color.white;

        public float HandleRadius { get; set; } = .3f;


        private readonly Action<Vector3> _onClick;

        private bool _enabled;
        private bool _useMask;
        private LayerMask _mask;
        private bool _usePlane;
        private Plane _plane;


        private void OnSceneGUI(SceneView sceneview) {
            if (!this.Enabled) return;

            var point = this._usePlane
                ? GetPlanePoint()
                : this.UsePhysics2D
                    ? GetRaycast2dPoint()
                    : GetRaycastPoint();

            if (point.Hit != null) {
                var color = Handles.color;
                Handles.color = this.HandleColor;
                Handles.DrawWireDisc(point.Hit.Value, point.Normal, this.HandleRadius);
                Handles.color = color;

                if (Handles.Button(Vector3.zero, SceneView.currentDrawingSceneView.rotation, 30, 5000, Handles.RectangleHandleCap))
                    this.HandleClick(point.Hit.Value);
            }


            if (EscapeInput())
                this.HandleEscape();
            return;


            (Vector3? Hit, Vector3 Normal) GetRaycastPoint() {
#if UNITY_PHYSICS_ENABLED
                var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                if (this._useMask
                        ? Physics.Raycast(ray, out var hit, this._mask.value)
                        : Physics.Raycast(ray, out hit))
                    return (hit.point, hit.normal);
#else
				WarningsPool.LogWarning("SceneClickHandler caused: PHYSICS is not enabled. Use Physics2d or Plane mode instead");
#endif
                return (null, Vector3.zero);
            }

            (Vector3? Hit, Vector3 Normal) GetRaycast2dPoint() {
#if UNITY_PHYSICS2D_ENABLED
                var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                var hit = this._useMask
                    ? Physics2D.Raycast(ray.origin, ray.direction, this._mask.value)
                    : Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null) return (hit.point, hit.normal);
#else
				WarningsPool.LogWarning("SceneClickHandler caused: PHYSICS2D is not enabled. Use Physics or Plane mode instead");
#endif
                return (null, Vector3.zero);
            }

            (Vector3? Hit, Vector3 Normal) GetPlanePoint() {
                var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                if (this._plane.Raycast(ray, out var enter)) return (ray.GetPoint(enter), this._plane.normal);

                return (null, Vector3.zero);
            }

            bool EscapeInput() => Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
        }

        private void HandleClick(Vector3 point) {
            if (this.SingleClickHandler)
                this.Enabled = false;

            this._onClick(point);

            Event.current.Use();
            HandleUtility.Repaint();
        }

        private void HandleEscape() {
            if (!this.Cancellable) return;

            Debug.LogWarning("Cancelled");
            this.Enabled = false;
        }
    }
}
#endif
