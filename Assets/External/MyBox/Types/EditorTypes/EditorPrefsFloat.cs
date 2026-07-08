#if UNITY_EDITOR
using System;
using UnityEditor;

namespace MyBox.EditorTools {
    [Serializable]
    public class EditorPrefsFloat : EditorPrefsType {
        public float Value { get => EditorPrefs.GetFloat(this.Key, this.DefaultValue); set => EditorPrefs.SetFloat(this.Key, value); }

        public float DefaultValue;

        public static EditorPrefsFloat WithKey(string key, float defaultValue = 0) => new EditorPrefsFloat(key, defaultValue);

        public EditorPrefsFloat(string key, float defaultValue = 0) {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
    }
}
#endif
