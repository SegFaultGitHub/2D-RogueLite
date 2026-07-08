#if UNITY_EDITOR
using System;
using UnityEngine;

namespace MyBox.EditorTools {
    public class ConditionallyColoredGUIBlock : IDisposable {
        private readonly Color _originalColor;

        public ConditionallyColoredGUIBlock(bool condition, Color color) {
            this._originalColor = GUI.color;

            if (condition) GUI.color = color;
        }

        public void Dispose() {
            GUI.color = this._originalColor;
        }
    }
}
#endif
