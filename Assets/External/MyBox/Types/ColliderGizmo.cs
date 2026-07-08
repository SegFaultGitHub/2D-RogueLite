using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.AI;
#endif

namespace MyBox {
    public class ColliderGizmo : MonoBehaviour {
#if UNITY_EDITOR
        public Presets Preset;

        public Color CustomWireColor;
        public Color CustomFillColor;
        public Color CustomCenterColor;

        public float Alpha = 1.0f;
        public Color WireColor = new Color(.6f, .6f, 1f, .5f);
        public Color FillColor = new Color(.6f, .7f, 1f, .1f);
        public Color CenterColor = new Color(.6f, .7f, 1f, .7f);

        public bool DrawFill = true;
        public bool DrawWire = true;
        public bool DrawCenter;

        /// <summary>
        /// The radius of the center marker on your collider(s)
        /// </summary>
        public float CenterMarkerRadius = 1.0f;

        public bool IncludeChildColliders;

#if UNITY_AI_ENABLED
        private NavMeshObstacle _navMeshObstacle;
#endif

#if UNITY_PHYSICS2D_ENABLED
        private List<EdgeCollider2D> _edgeColliders2D;
        private List<BoxCollider2D> _boxColliders2D;
        private List<CapsuleCollider2D> _capsuleColliders2D;
        private List<CircleCollider2D> _circleColliders2D;
#endif

#if UNITY_PHYSICS_ENABLED
        private List<BoxCollider> _boxColliders;
        private List<SphereCollider> _sphereColliders;
        private List<MeshCollider> _meshColliders;
#endif

        private readonly HashSet<Transform> _withColliders = new HashSet<Transform>();

        private Color _wireGizmoColor;
        private Color _fillGizmoColor;
        private Color _centerGizmoColor;

        private bool _initialized;


        private void OnDrawGizmos() {
            if (!this.enabled) return;
            if (!this._initialized)
                this.Refresh();

            this.DrawColliders();
        }

        #region Refresh
        public void Refresh() {
            this._initialized = true;

            this._wireGizmoColor = new Color(this.WireColor.r, this.WireColor.g, this.WireColor.b, this.WireColor.a * this.Alpha);
            this._fillGizmoColor = new Color(this.FillColor.r, this.FillColor.g, this.FillColor.b, this.FillColor.a * this.Alpha);
            this._centerGizmoColor = new Color(this.CenterColor.r, this.CenterColor.g, this.CenterColor.b, this.CenterColor.a * this.Alpha);

            this._withColliders.Clear();

#if UNITY_AI_ENABLED
            this._navMeshObstacle = this.gameObject.GetComponent<NavMeshObstacle>();
#endif

#if UNITY_PHYSICS2D_ENABLED
            this._edgeColliders2D?.Clear();
            this._boxColliders2D?.Clear();
            this._capsuleColliders2D?.Clear();
            this._circleColliders2D?.Clear();

            Collider2D[] colliders2d = this.IncludeChildColliders
                ? this.gameObject.GetComponentsInChildren<Collider2D>()
                : this.gameObject.GetComponents<Collider2D>();

            for (var i = 0; i < colliders2d.Length; i++) {
                var c = colliders2d[i];

                var box2d = c as BoxCollider2D;
                if (box2d != null) {
                    this._boxColliders2D ??= new List<BoxCollider2D>();
                    this._boxColliders2D.Add(box2d);
                    this._withColliders.Add(box2d.transform);
                    continue;
                }

                var edge = c as EdgeCollider2D;
                if (edge != null) {
                    this._edgeColliders2D ??= new List<EdgeCollider2D>();
                    this._edgeColliders2D.Add(edge);
                    this._withColliders.Add(edge.transform);
                    continue;
                }

                var capsule = c as CapsuleCollider2D;
                if (capsule != null) {
                    this._capsuleColliders2D ??= new List<CapsuleCollider2D>();
                    this._capsuleColliders2D.Add(capsule);
                    this._withColliders.Add(capsule.transform);
                    continue;
                }

                var circle2d = c as CircleCollider2D;
                if (circle2d != null) {
                    this._circleColliders2D ??= new List<CircleCollider2D>();
                    this._circleColliders2D.Add(circle2d);
                    this._withColliders.Add(circle2d.transform);
                }
            }
#endif

#if UNITY_PHYSICS_ENABLED
            this._boxColliders?.Clear();
            this._sphereColliders?.Clear();
            this._meshColliders?.Clear();

            Collider[] colliders = this.IncludeChildColliders
                ? this.gameObject.GetComponentsInChildren<Collider>()
                : this.gameObject.GetComponents<Collider>();

            for (var i = 0; i < colliders.Length; i++) {
                var c = colliders[i];

                var box = c as BoxCollider;
                if (box != null) {
                    this._boxColliders ??= new List<BoxCollider>();
                    this._boxColliders.Add(box);
                    this._withColliders.Add(box.transform);
                    continue;
                }

                var sphere = c as SphereCollider;
                if (sphere != null) {
                    this._sphereColliders ??= new List<SphereCollider>();
                    this._sphereColliders.Add(sphere);
                    this._withColliders.Add(sphere.transform);
                }

                var mesh = c as MeshCollider;
                if (mesh != null) {
                    this._meshColliders ??= new List<MeshCollider>();
                    this._meshColliders.Add(mesh);
                    this._withColliders.Add(mesh.transform);
                }
            }
#endif
        }
        #endregion


        #region Drawers
#if UNITY_PHYSICS2D_ENABLED

        private void DrawEdgeCollider2D(EdgeCollider2D coll) {
            var target = coll.transform;
            var lossyScale = target.lossyScale;
            var position = target.position;

            Gizmos.color = this.WireColor;
            Vector3 previous = Vector2.zero;
            bool first = true;
            for (int i = 0; i < coll.points.Length; i++) {
                var collPoint = coll.points[i];
                Vector3 pos = new Vector3(collPoint.x * lossyScale.x, collPoint.y * lossyScale.y, 0);
                Vector3 rotated = target.rotation * pos;

                if (first) first = false;
                else {
                    Gizmos.color = this._wireGizmoColor;
                    Gizmos.DrawLine(position + previous, position + rotated);
                }

                previous = rotated;

                this.DrawColliderGizmo(target.position + rotated, .05f);
            }
        }

        private void DrawBoxCollider2D(BoxCollider2D coll) {
            var target = coll.transform;
            Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
            this.DrawColliderGizmo(coll.offset, coll.size);
            Gizmos.matrix = Matrix4x4.identity;
        }

        private void DrawCapsuleCollider2D(CapsuleCollider2D coll) {
            var target = coll.transform;
            Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
            this.DrawColliderGizmo(coll.offset, coll.size);
            Gizmos.matrix = Matrix4x4.identity;
        }

        private void DrawCircleCollider2D(CircleCollider2D coll) {
            var target = coll.transform;
            var offset = coll.offset;
            var scale = target.lossyScale;
            this.DrawColliderGizmo(target.position + new Vector3(offset.x, offset.y, 0.0f), coll.radius * Mathf.Max(scale.x, scale.y));
        }

#endif

#if UNITY_PHYSICS_ENABLED

        private void DrawBoxCollider(BoxCollider coll) {
            var target = coll.transform;
            Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
            this.DrawColliderGizmo(coll.center, coll.size);
            Gizmos.matrix = Matrix4x4.identity;
        }

        private void DrawSphereCollider(SphereCollider coll) {
            var target = coll.transform;
            var scale = target.lossyScale;
            var center = coll.center;
            var max = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z)); // to not use Mathf.Max version with params[]
            this.DrawColliderGizmo(target.position + new Vector3(center.x, center.y, 0.0f), coll.radius * max);
        }

        private void DrawMeshCollider(MeshCollider coll) {
            var target = coll.transform;

            if (this.DrawWire) {
                Gizmos.color = this._wireGizmoColor;
                Gizmos.DrawWireMesh(coll.sharedMesh, target.position, target.rotation, target.localScale * 1.01f);
            }

            if (this.DrawFill) {
                Gizmos.color = this._fillGizmoColor;
                Gizmos.DrawMesh(coll.sharedMesh, target.position, target.rotation, target.localScale * 1.01f);
            }
        }

#endif

#if UNITY_AI_ENABLED

        private void DrawNavMeshObstacle(NavMeshObstacle obstacle) {
            var target = obstacle.transform;

            if (obstacle.shape == NavMeshObstacleShape.Box) {
                Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
                this.DrawColliderGizmo(obstacle.center, obstacle.size);
                Gizmos.matrix = Matrix4x4.identity;
            } else {
                var scale = target.lossyScale;
                var center = obstacle.center;
                var max = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z)); // to not use Mathf.Max version with params[]
                this.DrawColliderGizmo(target.position + new Vector3(center.x, center.y, 0.0f), obstacle.radius * max);
            }
        }

#endif


        private void DrawColliders() {
            if (this.DrawCenter) {
                Gizmos.color = this._centerGizmoColor;
                foreach (var withCollider in this._withColliders) {
                    Gizmos.DrawSphere(withCollider.position, this.CenterMarkerRadius);
                }
            }

            if (!this.DrawWire && !this.DrawFill) return;

#if UNITY_AI_ENABLED
            if (this._navMeshObstacle != null)
                this.DrawNavMeshObstacle(this._navMeshObstacle);
#endif

#if UNITY_PHYSICS2D_ENABLED
            if (this._edgeColliders2D != null) {
                foreach (var edge in this._edgeColliders2D) {
                    if (edge == null) continue;
                    this.DrawEdgeCollider2D(edge);
                }
            }

            if (this._boxColliders2D != null) {
                foreach (var box in this._boxColliders2D) {
                    if (box == null) continue;
                    this.DrawBoxCollider2D(box);
                }
            }

            if (this._capsuleColliders2D != null) {
                foreach (var capsule in this._capsuleColliders2D) {
                    if (capsule == null) continue;
                    this.DrawCapsuleCollider2D(capsule);
                }
            }

            if (this._circleColliders2D != null) {
                foreach (var circle in this._circleColliders2D) {
                    if (circle == null) continue;
                    this.DrawCircleCollider2D(circle);
                }
            }
#endif

#if UNITY_PHYSICS_ENABLED
            if (this._boxColliders != null) {
                foreach (var box in this._boxColliders) {
                    if (box == null) continue;
                    this.DrawBoxCollider(box);
                }
            }

            if (this._sphereColliders != null) {
                foreach (var sphere in this._sphereColliders) {
                    if (sphere == null) continue;
                    this.DrawSphereCollider(sphere);
                }
            }

            if (this._meshColliders != null) {
                foreach (var mesh in this._meshColliders) {
                    if (mesh == null) continue;
                    this.DrawMeshCollider(mesh);
                }
            }
#endif
        }


        private void DrawColliderGizmo(Vector3 position, Vector3 size) {
            if (this.DrawWire) {
                Gizmos.color = this._wireGizmoColor;
                Gizmos.DrawWireCube(position, size);
            }

            if (this.DrawFill) {
                Gizmos.color = this._fillGizmoColor;
                Gizmos.DrawCube(position, size);
            }
        }

        private void DrawColliderGizmo(Vector3 position, float radius) {
            if (this.DrawWire) {
                Gizmos.color = this._wireGizmoColor;
                Gizmos.DrawWireSphere(position, radius);
            }

            if (this.DrawFill) {
                Gizmos.color = this._fillGizmoColor;
                Gizmos.DrawSphere(position, radius);
            }
        }
        #endregion


        #region Change Preset
        public enum Presets {
            Custom,
            Red,
            Blue,
            Green,
            Purple,
            Yellow,
            Aqua,
            White,
            Lilac,
            DirtySand
        }

        public void ChangePreset(Presets preset) {
            this.Preset = preset;

            switch (this.Preset) {
                case Presets.Red:
                    this.WireColor = new Color32(143, 0, 21, 202);
                    this.FillColor = new Color32(218, 0, 0, 37);
                    this.CenterColor = new Color32(135, 36, 36, 172);
                    break;

                case Presets.Blue:
                    this.WireColor = new Color32(0, 116, 214, 202);
                    this.FillColor = new Color32(0, 110, 218, 37);
                    this.CenterColor = new Color32(57, 160, 221, 172);
                    break;

                case Presets.Green:
                    this.WireColor = new Color32(153, 255, 187, 128);
                    this.FillColor = new Color32(153, 255, 187, 62);
                    this.CenterColor = new Color32(153, 255, 187, 172);
                    break;

                case Presets.Purple:
                    this.WireColor = new Color32(138, 138, 234, 128);
                    this.FillColor = new Color32(173, 178, 255, 26);
                    this.CenterColor = new Color32(153, 178, 255, 172);
                    break;

                case Presets.Yellow:
                    this.WireColor = new Color32(255, 231, 35, 128);
                    this.FillColor = new Color32(255, 252, 153, 100);
                    this.CenterColor = new Color32(255, 242, 84, 172);
                    break;

                case Presets.DirtySand:
                    this.WireColor = new Color32(255, 170, 0, 60);
                    this.FillColor = new Color32(180, 160, 80, 175);
                    this.CenterColor = new Color32(255, 242, 84, 172);
                    break;

                case Presets.Aqua:
                    this.WireColor = new Color32(255, 255, 255, 120);
                    this.FillColor = new Color32(0, 230, 255, 140);
                    this.CenterColor = new Color32(255, 255, 255, 120);
                    break;

                case Presets.White:
                    this.WireColor = new Color32(255, 255, 255, 130);
                    this.FillColor = new Color32(255, 255, 255, 130);
                    this.CenterColor = new Color32(255, 255, 255, 130);
                    break;

                case Presets.Lilac:
                    this.WireColor = new Color32(255, 255, 255, 255);
                    this.FillColor = new Color32(160, 190, 255, 140);
                    this.CenterColor = new Color32(255, 255, 255, 130);
                    break;


                case Presets.Custom:
                    this.WireColor = this.CustomWireColor;
                    this.FillColor = this.CustomFillColor;
                    this.CenterColor = this.CustomCenterColor;
                    break;
            }

            this.Refresh();
        }
        #endregion

#endif
    }
}


#if UNITY_EDITOR

namespace MyBox.Internal {
    [CustomEditor(typeof(ColliderGizmo)), CanEditMultipleObjects]
    public class ColliderGizmoEditor : Editor {
        private SerializedProperty _enabledProperty;
        private SerializedProperty _alphaProperty;
        private SerializedProperty _drawWireProperty;
        private SerializedProperty _wireColorProperty;
        private SerializedProperty _drawFillProperty;
        private SerializedProperty _fillColorProperty;
        private SerializedProperty _drawCenterProperty;
        private SerializedProperty _centerColorProperty;
        private SerializedProperty _centerRadiusProperty;

        private SerializedProperty _includeChilds;

        private ColliderGizmo _target;

        private int _collidersCount;

        private void OnEnable() {
            this._target = this.target as ColliderGizmo;

            this._enabledProperty = this.serializedObject.FindProperty("m_Enabled");
            this._alphaProperty = this.serializedObject.FindProperty("Alpha");

            this._drawWireProperty = this.serializedObject.FindProperty("DrawWire");
            this._wireColorProperty = this.serializedObject.FindProperty("WireColor");

            this._drawFillProperty = this.serializedObject.FindProperty("DrawFill");
            this._fillColorProperty = this.serializedObject.FindProperty("FillColor");

            this._drawCenterProperty = this.serializedObject.FindProperty("DrawCenter");
            this._centerColorProperty = this.serializedObject.FindProperty("CenterColor");
            this._centerRadiusProperty = this.serializedObject.FindProperty("CenterMarkerRadius");

            this._includeChilds = this.serializedObject.FindProperty("IncludeChildColliders");

            this._collidersCount = this.CollidersCount();
        }


        public override void OnInspectorGUI() {
            Undo.RecordObject(this._target, "CG_State");

            EditorGUILayout.PropertyField(this._enabledProperty);

            EditorGUI.BeginChangeCheck();
            this._target.Preset = (ColliderGizmo.Presets)EditorGUILayout.EnumPopup("Color Preset", this._target.Preset);
            if (EditorGUI.EndChangeCheck()) {
                foreach (var singleTarget in this.targets) {
                    var gizmo = (ColliderGizmo)singleTarget;
                    gizmo.ChangePreset(this._target.Preset);
                    EditorUtility.SetDirty(gizmo);
                }
            }

            this._alphaProperty.floatValue = EditorGUILayout.Slider("Overall Transparency", this._alphaProperty.floatValue, 0, 1);


            EditorGUI.BeginChangeCheck();
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.PropertyField(this._drawWireProperty);
                if (this._drawWireProperty.boolValue) EditorGUILayout.PropertyField(this._wireColorProperty, new GUIContent(""));
            }

            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.PropertyField(this._drawFillProperty);
                if (this._drawFillProperty.boolValue) EditorGUILayout.PropertyField(this._fillColorProperty, new GUIContent(""));
            }

            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.PropertyField(this._drawCenterProperty);
                if (this._drawCenterProperty.boolValue) {
                    EditorGUILayout.PropertyField(this._centerColorProperty, GUIContent.none);
                    EditorGUILayout.PropertyField(this._centerRadiusProperty);
                }
            }


            if (EditorGUI.EndChangeCheck()) {
                var presetProp = this.serializedObject.FindProperty("Preset");
                var customWireColor = this.serializedObject.FindProperty("CustomWireColor");
                var customFillColor = this.serializedObject.FindProperty("CustomFillColor");
                var customCenterColor = this.serializedObject.FindProperty("CustomCenterColor");

                presetProp.enumValueIndex = (int)ColliderGizmo.Presets.Custom;
                customWireColor.colorValue = this._wireColorProperty.colorValue;
                customFillColor.colorValue = this._fillColorProperty.colorValue;
                customCenterColor.colorValue = this._centerColorProperty.colorValue;
            }

            EditorGUILayout.PropertyField(this._includeChilds);

            int collidersCountCheck = this.CollidersCount();
            bool collidersCountChanged = collidersCountCheck != this._collidersCount;
            this._collidersCount = collidersCountCheck;

            if (GUI.changed || collidersCountChanged) {
                this.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(this._target);

                this._target.Refresh();
            }
        }

        private int CollidersCount() {
            int result = 0;

            if (this._includeChilds.boolValue) {
#if UNITY_PHYSICS_ENABLED
                result += this._target.gameObject.GetComponentsInChildren<Collider>().Length;
#endif
#if UNITY_PHYSICS2D_ENABLED
                result += this._target.gameObject.GetComponentsInChildren<Collider2D>().Length;
#endif
                return result;
            }

#if UNITY_PHYSICS_ENABLED
            result += this._target.gameObject.GetComponents<Collider>().Length;
#endif
#if UNITY_PHYSICS2D_ENABLED
            result += this._target.gameObject.GetComponents<Collider2D>().Length;
#endif
            return result;
        }
    }
}

#endif
