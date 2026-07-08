using MyBox.Internal;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using MyBox.EditorTools;
#endif

#pragma warning disable 0414
namespace MyBox {
    public class MaxValueAttribute : AttributeBase {
        private readonly float _x;
        private readonly float _y;
        private readonly float _z;

        private readonly bool _vectorValuesSet;

        public MaxValueAttribute(float value) {
            this._x = value;
        }

        public MaxValueAttribute(float x, float y, float z) {
            this._x = x;
            this._y = y;
            this._z = z;
            this._vectorValuesSet = true;
        }

#if UNITY_EDITOR
        private string _warning;

        public override void ValidateProperty(SerializedProperty property) {
            if (!property.IsNumerical()) {
                if (this._warning == null)
                    this._warning = property.name + " caused: [MaxValueAttribute] used with non-numeric property";
                return;
            }

            bool valueHandled = this.HandleValues(property);
            if (valueHandled) property.serializedObject.ApplyModifiedProperties();
        }

        public override bool OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (this._warning == null) return false;
            EditorGUI.HelpBox(position, this._warning, MessageType.Warning);
            return true;
        }

        public override float? OverrideHeight() {
            if (this._warning == null) return null;
            return EditorGUIUtility.singleLineHeight;
        }


        #region Handle Value
        /// <returns>true if fixed</returns>
        private bool HandleValues(SerializedProperty property) {
            switch (property.propertyType) {
                case SerializedPropertyType.Float:
                case SerializedPropertyType.Integer:
                    return this.HandleNumerics(property);

                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector4:
                    return this.HandleVectors(property);

                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                    return this.HandleIntVectors(property);
            }

            return false;
        }


        private bool HandleNumerics(SerializedProperty property) {
            var maxValue = this._x;

            if (property.propertyType == SerializedPropertyType.Integer && property.intValue > maxValue) {
                property.intValue = (int)maxValue;
                return true;
            }

            if (property.propertyType == SerializedPropertyType.Float && property.floatValue > maxValue) {
                property.floatValue = maxValue;
                return true;
            }

            return false;
        }


        private bool HandleVectors(SerializedProperty property) {
            var x = this._x;
            var y = this._vectorValuesSet
                ? this._y
                : x;
            var z = this._vectorValuesSet
                ? this._z
                : x;

            Vector4 vector = Vector4.zero;
            switch (property.propertyType) {
                case SerializedPropertyType.Vector2:
                    vector = property.vector2Value;
                    break;
                case SerializedPropertyType.Vector3:
                    vector = property.vector3Value;
                    break;
                case SerializedPropertyType.Vector4:
                    vector = property.vector4Value;
                    break;
            }

            bool handled = false;
            if (vector[0] > x) {
                vector[0] = x;
                handled = true;
            }

            if (vector[1] > y) {
                vector[1] = y;
                handled = true;
            }

            if (vector[2] > z) {
                vector[2] = z;
                handled = true;
            }

            switch (property.propertyType) {
                case SerializedPropertyType.Vector2:
                    property.vector2Value = vector;
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = vector;
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = vector;
                    break;
            }

            return handled;
        }


        private bool HandleIntVectors(SerializedProperty property) {
            var x = (int)this._x;
            var y = this._vectorValuesSet
                ? (int)this._y
                : x;
            var z = this._vectorValuesSet
                ? (int)this._z
                : x;

            if (property.propertyType == SerializedPropertyType.Vector2Int) {
                var vector = property.vector2IntValue;
                if (vector.x > x || vector.y > y) {
                    property.vector2IntValue = new Vector2Int(
                        vector.x > x
                            ? x
                            : vector.x,
                        vector.y > y
                            ? y
                            : vector.y
                    );
                    return true;
                }

                return false;
            }

            if (property.propertyType == SerializedPropertyType.Vector3Int) {
                var vector = property.vector3IntValue;
                if (vector.x > x || vector.y > y || vector.z > z) {
                    property.vector3IntValue = new Vector3Int(
                        vector.x > x
                            ? x
                            : vector.x,
                        vector.y > y
                            ? y
                            : vector.y,
                        vector.z > z
                            ? z
                            : vector.z
                    );
                    return true;
                }

                return false;
            }

            return false;
        }
        #endregion

#endif
    }
}
