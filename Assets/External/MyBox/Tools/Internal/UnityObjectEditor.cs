#if UNITY_EDITOR && !MYBOX_DISABLE_INSPECTOR_OVERRIDE && !ODIN_INSPECTOR
namespace MyBox.Internal {
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Object), true), CanEditMultipleObjects]
    public class UnityObjectEditor : Editor {
        private FoldoutAttributeHandler _foldout;
        private ButtonMethodHandler _buttonMethod;

        private void OnEnable() {
            if (this.target == null) return;

            this._foldout = new FoldoutAttributeHandler(this.target, this.serializedObject);
            this._buttonMethod = new ButtonMethodHandler(this.target);
        }

        private void OnDisable() {
            this._foldout?.OnDisable();
        }

        public override void OnInspectorGUI() {
            this._buttonMethod?.OnBeforeInspectorGUI();

            if (this._foldout != null) {
                this._foldout.Update();
                if (!this._foldout.OverrideInspector) base.OnInspectorGUI();
                else
                    this._foldout.OnInspectorGUI();
            }

            this._buttonMethod?.OnAfterInspectorGUI();
        }
    }
}
#endif
