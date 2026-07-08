#if UNITY_EDITOR
using System;
using UnityEngine;

namespace MyBox.EditorTools {
    public class ConditionallyEnabledGUIBlock : IDisposable {
        private readonly bool _originalState;

        public ConditionallyEnabledGUIBlock(bool condition) {
            this._originalState = GUI.enabled;
            GUI.enabled = condition;
        }

        public void Dispose() {
            GUI.enabled = this._originalState;
        }
    }
}
#endif
