#if UNITY_EDITOR
namespace MyBox.EditorTools {
    using System;
    using UnityEditor;

    [Serializable]
    public class EditorPrefsInt : EditorPrefsType {
        public int Value { get => (int)EditorPrefs.GetFloat(this.Key, this.DefaultValue); set => EditorPrefs.SetFloat(this.Key, value); }

        public int DefaultValue;

        public static EditorPrefsInt WithKey(string key, int defaultValue = 0) => new EditorPrefsInt(key, defaultValue);

        public EditorPrefsInt(string key, int defaultValue = 0) {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
    }
}
#endif
