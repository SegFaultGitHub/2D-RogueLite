using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace MyBox {
    [PublicAPI]
    public static class MyDebug {
        #region Log Array
        private static StringBuilder _stringBuilder;

        private static void PrepareStringBuilder() {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            else _stringBuilder.Clear();
        }

        public static void LogArray<T>(T[] toLog) {
            PrepareStringBuilder();

            _stringBuilder.Append("Log Array: ").Append(typeof(T).Name).Append(" (").Append(toLog.Length).Append(")\n");
            for (int i = 0; i < toLog.Length; i++) {
                _stringBuilder.Append("\n\t").Append(i.ToString().Colored(Colors.brown)).Append(": ").Append(toLog[i]);
            }

            Debug.Log(_stringBuilder.ToString());
        }

        public static void LogArray<T>(IList<T> toLog) {
            PrepareStringBuilder();

            int count = toLog.Count;
            _stringBuilder.Append("Log Array: ").Append(typeof(T).Name).Append(" (").Append(count).Append(")\n");

            for (int i = 0; i < count; i++) {
                _stringBuilder.Append("\n\t" + i.ToString().Colored(Colors.brown) + ": " + toLog[i]);
            }

            Debug.Log(_stringBuilder.ToString());
        }
        #endregion

        public static void LogColor(Color color) {
            Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">████████████</color> = " + color);
        }

        #region Debug Bounds
        /// <summary>
        /// Draw bounds of Mesh
        /// </summary>
        public static void DrawDebugBounds(MeshFilter mesh, Color color) {
#if UNITY_EDITOR
            if (mesh == null) return;
            MeshRenderer renderer = mesh.GetComponent<MeshRenderer>();
            DrawDebugBounds(renderer, color);
#endif
        }

        /// <summary>
        /// Draw bounds of MeshRenderer
        /// </summary>
        public static void DrawDebugBounds(MeshRenderer renderer, Color color) {
#if UNITY_EDITOR
            Bounds bounds = renderer.bounds;
            DrawDebugBounds(bounds, color);
#endif
        }

        /// <summary>
        /// Draw bounds of Bounds
        /// </summary>
        public static void DrawDebugBounds(Bounds bounds, Color color) {
#if UNITY_EDITOR
            Vector3 v3Center = bounds.center;
            Vector3 v3Extents = bounds.extents;

            Vector3 v3FrontTopLeft = new Vector3(
                v3Center.x - v3Extents.x,
                v3Center.y + v3Extents.y,
                v3Center.z - v3Extents.z
            ); // Front top left corner
            Vector3 v3FrontTopRight = new Vector3(
                v3Center.x + v3Extents.x,
                v3Center.y + v3Extents.y,
                v3Center.z - v3Extents.z
            ); // Front top right corner
            Vector3 v3FrontBottomLeft = new Vector3(
                v3Center.x - v3Extents.x,
                v3Center.y - v3Extents.y,
                v3Center.z - v3Extents.z
            ); // Front bottom left corner
            Vector3 v3FrontBottomRight = new Vector3(
                v3Center.x + v3Extents.x,
                v3Center.y - v3Extents.y,
                v3Center.z - v3Extents.z
            ); // Front bottom right corner
            Vector3 v3BackTopLeft = new Vector3(
                v3Center.x - v3Extents.x,
                v3Center.y + v3Extents.y,
                v3Center.z + v3Extents.z
            ); // Back top left corner
            Vector3 v3BackTopRight = new Vector3(
                v3Center.x + v3Extents.x,
                v3Center.y + v3Extents.y,
                v3Center.z + v3Extents.z
            ); // Back top right corner
            Vector3 v3BackBottomLeft = new Vector3(
                v3Center.x - v3Extents.x,
                v3Center.y - v3Extents.y,
                v3Center.z + v3Extents.z
            ); // Back bottom left corner
            Vector3 v3BackBottomRight = new Vector3(
                v3Center.x + v3Extents.x,
                v3Center.y - v3Extents.y,
                v3Center.z + v3Extents.z
            ); // Back bottom right corner

            Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, color);
            Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, color);
            Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, color);
            Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, color);

            Debug.DrawLine(v3BackTopLeft, v3BackTopRight, color);
            Debug.DrawLine(v3BackTopRight, v3BackBottomRight, color);
            Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, color);
            Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, color);

            Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, color);
            Debug.DrawLine(v3FrontTopRight, v3BackTopRight, color);
            Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, color);
            Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, color);
#endif
        }
        #endregion


        public static void DrawString(string text, Vector3 worldPos, Color? colour = null) {
#if UNITY_EDITOR
            SceneView view = UnityEditor.SceneView.currentDrawingSceneView;
            if (view == null) return;

            Color defaultColor = GUI.color;

            UnityEditor.Handles.BeginGUI();
            if (colour.HasValue) GUI.color = colour.Value;
            Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);

            UnityEditor.Handles.EndGUI();

            GUI.color = defaultColor;
#endif
        }


        /// <summary>
        /// Draw directional arrow
        /// </summary>
        public static void DrawArrowRay(Vector3 position, Vector3 direction, float headLength = 0.25f, float headAngle = 20.0f) {
#if UNITY_EDITOR
            Vector3 rightVector = new Vector3(0, 0, 1);
            Quaternion directionRotation = Quaternion.LookRotation(direction);

            Debug.DrawRay(position, direction);
            Vector3 right = directionRotation * Quaternion.Euler(0, 180 + headAngle, 0) * rightVector;
            Vector3 left = directionRotation * Quaternion.Euler(0, 180 - headAngle, 0) * rightVector;
            Debug.DrawRay(position + direction, right * headLength);
            Debug.DrawRay(position + direction, left * headLength);
#endif
        }


        /// <summary>
        /// Draw XYZ dimensional RGB cross
        /// </summary>
        public static void DrawDimensionalCross(Vector3 position, float size, float duration = 0) {
#if UNITY_EDITOR
            float halfSize = size / 2;
            Debug.DrawLine(position.OffsetY(-halfSize), position.OffsetY(halfSize), Color.green, duration);
            Debug.DrawLine(position.OffsetX(-halfSize), position.OffsetX(halfSize), Color.red, duration);
            Debug.DrawLine(position.OffsetZ(-halfSize), position.OffsetZ(halfSize), Color.blue, duration);
#endif
        }

#if UNITY_AI_ENABLED
        /// <summary>
        /// Draw segments of a path with Debug Lines
        /// </summary>
        public static void VisualizeNavMeshPath(UnityEngine.AI.NavMeshPath path) {
            Vector3[] corners = path.corners;
            for (int i = 1; i < corners.Length; i++) {
                Vector3 cornerA = corners[i - 1];
                Vector3 cornerB = corners[i];
                Debug.DrawLine(cornerA, cornerB);
            }
        }
#endif
    }
}
