#if UNITY_EDITOR
using System;
using UnityEditor;

namespace MyBox.EditorTools {
    [Serializable]
    public class EditorPrefsString : EditorPrefsType {
        public string Value { get => EditorPrefs.GetString(this.Key, this.DefaultValue); set => EditorPrefs.SetString(this.Key, value); }

        public string DefaultValue;

        public static EditorPrefsString WithKey(string key, string defaultValue = "") => new EditorPrefsString(key, defaultValue);

        public EditorPrefsString(string key, string defaultValue = "") {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
    }
}
#endif
