using UnityEngine;

namespace Code.Utils {
    /// <summary>
    /// Small helper class to convert viewport, screen or world positions to canvas space.
    /// Only works with screen space canvases.
    /// </summary>
    /// <example>
    /// <code>
    /// objectOnCanvasRectTransform.anchoredPosition = specificCanvas.WorldToCanvasPoint(worldspaceTransform.position);
    /// </code>
    /// </example>
    public static class SC_CanvasPositioningExtensions {
        public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null) {
            if (camera == null) {
                camera = Camera.main;
            }

            Vector3 viewportPosition = camera!.WorldToViewportPoint(worldPosition);
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition) {
            Vector3 viewportPosition = new Vector3(screenPosition.x / Screen.width, screenPosition.y / Screen.height, 0);
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        private static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition) {
            Vector3 centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 scale = canvasRect.sizeDelta;
            return Vector3.Scale(centerBasedViewPortPosition, scale);
        }
    }
}
