#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MyBox.Internal {
    [CustomPropertyDrawer(typeof(AttributeBase), true)]
    public class AttributeBaseDrawer : PropertyDrawer {
        private AttributeBase[] _cachedAttributes;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            this.CacheAttributes();

            for (var i = this._cachedAttributes.Length - 1; i >= 0; i--) {
                var overriden = this._cachedAttributes[i].OverrideHeight();
                if (overriden != null) return overriden.Value;
            }

            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            this.CacheAttributes();

            bool drawn = false;
            for (var i = this._cachedAttributes.Length - 1; i >= 0; i--) {
                var ab = this._cachedAttributes[i];
                ab.ValidateProperty(property);

                ab.OnBeforeGUI(position, property, label);

                // Draw the things with higher priority first. If drawn once - skip drawing
                if (!drawn) {
                    if (ab.OnGUI(position, property, label)) drawn = true;
                }

                ab.OnAfterGUI(position, property, label);
            }

            if (!drawn) EditorGUI.PropertyField(position, property, label);
        }

        private void CacheAttributes() {
            if (this._cachedAttributes.IsNullOrEmpty()) {
                this._cachedAttributes = this.fieldInfo.GetCustomAttributes(typeof(AttributeBase), false)
                    .OrderBy(s => ((PropertyAttribute)s).order)
                    .Select(a => a as AttributeBase)
                    .ToArray();
            }
        }
    }
}
#endif
