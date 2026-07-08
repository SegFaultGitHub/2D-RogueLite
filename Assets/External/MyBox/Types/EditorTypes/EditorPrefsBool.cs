#if UNITY_EDITOR
using System;
using UnityEditor;

namespace MyBox.EditorTools {
    [Serializable]
    public class EditorPrefsBool : EditorPrefsType {
        public bool Value { get => EditorPrefs.GetBool(this.Key, this.DefaultValue); set => EditorPrefs.SetBool(this.Key, value); }

        public bool DefaultValue;

        public static EditorPrefsBool WithKey(string key, bool defaultValue = false) => new EditorPrefsBool(key, defaultValue);

        public void Toggle() => this.Value = !this.Value;

        public EditorPrefsBool(string key, bool defaultValue = false) {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
    }
}
#endif
