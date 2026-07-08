using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MyBox {
    public class ConstantsSelectionAttribute : PropertyAttribute {
        public readonly Type SelectFromType;

        public ConstantsSelectionAttribute(Type type) {
            this.SelectFromType = type;
        }
    }
}

#if UNITY_EDITOR
namespace MyBox.Internal {
    using UnityEditor;
    using EditorTools;

    [CustomPropertyDrawer(typeof(ConstantsSelectionAttribute))]
    public class ConstantsSelectionAttributeDrawer : PropertyDrawer {
        private ConstantsSelectionAttribute _attribute;
        private readonly List<MemberInfo> _constants = new List<MemberInfo>();
        private string[] _names;
        private object[] _values;
        private Type _targetType;
        private int _selectedValueIndex;
        private bool _valueFound;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (this._attribute == null)
                this.Initialize(property);
            if (this._values.IsNullOrEmpty() || this._selectedValueIndex < 0) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            if (!this._valueFound && this._selectedValueIndex == 0) MyGUI.DrawColouredRect(position, MyGUI.Colors.Yellow);

            EditorGUI.BeginChangeCheck();
            this._selectedValueIndex = EditorGUI.Popup(position, label.text, this._selectedValueIndex, this._names);
            if (EditorGUI.EndChangeCheck()) {
                this.fieldInfo.SetValue(property.serializedObject.targetObject, this._values[this._selectedValueIndex]);
                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }

        private object GetValue(SerializedProperty property) {
            return this.fieldInfo.GetValue(property.serializedObject.targetObject);
        }

        private void Initialize(SerializedProperty property) {
            this._attribute = (ConstantsSelectionAttribute)this.attribute;
            this._targetType = this.fieldInfo.FieldType;

            var searchFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            var allPublicStaticFields = this._attribute.SelectFromType.GetFields(searchFlags);
            var allPublicStaticProperties = this._attribute.SelectFromType.GetProperties(searchFlags);

            // IsLiteral determines if its value is written at compile time and not changeable
            // IsInitOnly determines if the field can be set in the body of the constructor
            // for C# a field which is readonly keyword would have both true but a const field would have only IsLiteral equal to true
            foreach (FieldInfo field in allPublicStaticFields) {
                if ((field.IsInitOnly || field.IsLiteral) && field.FieldType == this._targetType)
                    this._constants.Add(field);
            }

            foreach (var prop in allPublicStaticProperties) {
                if (prop.PropertyType == this._targetType)
                    this._constants.Add(prop);
            }


            if (this._constants.IsNullOrEmpty()) return;
            this._names = new string[this._constants.Count];
            this._values = new object[this._constants.Count];
            for (var i = 0; i < this._constants.Count; i++) {
                this._names[i] = this._constants[i].Name;
                this._values[i] = this.GetValue(i);
            }

            var currentValue = this.GetValue(property);
            if (currentValue != null) {
                for (var i = 0; i < this._values.Length; i++) {
                    if (currentValue.Equals(this._values[i])) {
                        this._valueFound = true;
                        this._selectedValueIndex = i;
                    }
                }
            }

            if (!this._valueFound) {
                this._names = this._names.InsertAt(0);
                this._values = this._values.InsertAt(0);
                var actualValue = this.GetValue(property);
                var value = actualValue != null
                    ? actualValue
                    : "NULL";
                this._names[0] = "NOT FOUND: " + value;
                this._values[0] = actualValue;
            }
        }

        private object GetValue(int index) {
            var member = this._constants[index];
            if (member.MemberType == MemberTypes.Field) return ((FieldInfo)member).GetValue(null);
            if (member.MemberType == MemberTypes.Property) return ((PropertyInfo)member).GetValue(null);
            return null;
        }
    }
}
#endif
