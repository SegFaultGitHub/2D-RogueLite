#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace MyBox.Internal {
    public class AssetsPresetPreprocessBase : ScriptableObject {
        public static AssetsPresetPreprocessBase Instance { get; set; }

        public ConditionalPreset[] Presets;

        public string[] ExcludeProperties = { "SpriteBorder", "Pivot", "Alignment" };

        private void Awake() => Instance = this;
    }

    [Serializable]
    public class ConditionalPreset {
        public string PathContains;
        public string TypeOf;
        public string Prefix;
        public string Postfix;

        public Preset Preset;

        public string[] PropertiesToApply;

        public bool Sample(string path) {
            var pathSet = !string.IsNullOrEmpty(this.PathContains);
            var typeSet = !string.IsNullOrEmpty(this.TypeOf);
            var prefixSet = !string.IsNullOrEmpty(this.Prefix);
            var postfixSet = !string.IsNullOrEmpty(this.Postfix);

            if (pathSet && !path.Contains(this.PathContains)) return false;

            var extension = Path.GetExtension(path);
            var filename = Path.GetFileNameWithoutExtension(path);
            if (extension == null || filename == null) return false;

            if (typeSet && !extension.Contains(this.TypeOf)) return false;

            if (prefixSet && !filename.StartsWith(this.Prefix)) return false;
            if (postfixSet && !filename.EndsWith(this.Postfix)) return false;

            return true;
        }
    }
}
#endif
