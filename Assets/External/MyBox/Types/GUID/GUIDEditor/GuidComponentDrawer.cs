#if UNITY_EDITOR
using UnityEditor;

namespace MyBox.Internal {
    [CustomEditor(typeof(GuidComponent))]
    public class GuidComponentDrawer : Editor {
        private GuidComponent _guid;

        public override void OnInspectorGUI() {
            if (this._guid == null)
                this._guid = (GuidComponent)this.target;

            using (new EditorGUI.DisabledScope(true)) EditorGUILayout.TextField("Guid:", this._guid.GetGuid().ToString());
        }
    }
}
#endif
