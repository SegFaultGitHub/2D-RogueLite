#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace MyBox.EditorTools {
    [Serializable]
    public class EditorPrefsVector3 : EditorPrefsType {
        public Vector3 Value {
            get => new Vector3(
                EditorPrefs.GetFloat(this.Key + "x", this.DefaultValue.x),
                EditorPrefs.GetFloat(this.Key + "y", this.DefaultValue.y),
                EditorPrefs.GetFloat(this.Key + "z", this.DefaultValue.z)
            );
            set {
                EditorPrefs.SetFloat(this.Key + "x", value.x);
                EditorPrefs.SetFloat(this.Key + "y", value.y);
                EditorPrefs.SetFloat(this.Key + "z", value.z);
            }
        }

        public Vector3 DefaultValue;

        public static EditorPrefsVector3 WithKey(string key, Vector3 defaultValue = new Vector3()) =>
            new EditorPrefsVector3(key, defaultValue);

        public EditorPrefsVector3(string key, Vector3 defaultValue = new Vector3()) {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
    }
}
#endif
