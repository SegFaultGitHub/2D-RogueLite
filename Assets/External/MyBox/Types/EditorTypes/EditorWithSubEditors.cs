#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace MyBox.EditorTools {
    // Interesting approach for nested editors from Adventure Game Unity tutorial
    // www.unity3d.com/ru/learn/tutorials/projects/adventure-game-tutorial/conditions?playlist=44381
    public abstract class EditorWithSubEditors<TEditor, TTarget> : Editor
        where TEditor : Editor
        where TTarget : Object {
        protected TEditor[] SubEditors;


        protected void CheckAndCreateSubEditors(TTarget[] subEditorTargets) {
            if (this.SubEditors != null && this.SubEditors.Length == subEditorTargets.Length)
                return;

            this.CleanupEditors();

            this.SubEditors = new TEditor[subEditorTargets.Length];

            for (int i = 0; i < this.SubEditors.Length; i++) {
                this.SubEditors[i] = CreateEditor(subEditorTargets[i]) as TEditor;
                this.SubEditorSetup(this.SubEditors[i]);
            }
        }


        protected void CleanupEditors() {
            if (this.SubEditors == null)
                return;

            for (int i = 0; i < this.SubEditors.Length; i++) {
                DestroyImmediate(this.SubEditors[i]);
            }

            this.SubEditors = null;
        }


        protected abstract void SubEditorSetup(TEditor editor);
    }
}
#endif
