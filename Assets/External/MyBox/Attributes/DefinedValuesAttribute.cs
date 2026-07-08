using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

//TODO: Support for method returning (Str, Obj)[] collection for custom display values
//TODO: Test the assignment of the custom data classes (serialized structs with specific values?)
//TODO: Use the Methods returning enumerable collections
//TODO: Test the methods returning non-serializable objects
//TODO: What if the Value collection is changed? Add warning?
//TODO: Utilize WarningsPool to notify about any issues (or display warning instead of the field?)
//TODO: Refactoring

namespace MyBox {
    /// <summary>
    /// Create Popup with predefined values for string, int or float property
    /// </summary>
    public class DefinedValuesAttribute : PropertyAttribute {
        public readonly object[] ValuesArray;
        public readonly string[] LabelsArray;
        public readonly string UseMethod;

        public DefinedValuesAttribute(params object[] definedValues) {
            this.ValuesArray = definedValues;
        }

        public DefinedValuesAttribute(bool withLabels, params object[] definedValues) {
            var actualLength = definedValues.Length / 2;
            this.ValuesArray = new object[actualLength];
            this.LabelsArray = new string[actualLength];
            int actualIndex = 0;
            for (var i = 0; i < definedValues.Length; i++) {
                this.ValuesArray[actualIndex] = definedValues[i];
                this.LabelsArray[actualIndex] = definedValues[++i].ToString();
                actualIndex++;
            }
        }

        public DefinedValuesAttribute(string method) {
            this.UseMethod = method;
        }
    }
}

#if UNITY_EDITOR
namespace MyBox.Internal {
    using UnityEditor;
    using EditorTools;

    [CustomPropertyDrawer(typeof(DefinedValuesAttribute))]
    public class DefinedValuesAttributeDrawer : PropertyDrawer {
        private object[] _objects;
        private string[] _labels;
        private Type _valueType;
        private bool _initialized;

        private void Initialize(SerializedProperty targetProperty, DefinedValuesAttribute defaultValuesAttribute) {
            if (this._initialized) return;
            this._initialized = true;

            var targetObject = targetProperty.serializedObject.targetObject;

            var values = defaultValuesAttribute.ValuesArray;
            var labels = defaultValuesAttribute.LabelsArray;
            var methodName = defaultValuesAttribute.UseMethod;

            if (methodName.NotNullOrEmpty()) {
                var valuesFromMethod = GetValuesFromMethod();
                if (valuesFromMethod.NotNullOrEmpty()) values = valuesFromMethod;
                else {
                    WarningsPool.LogWarning(
                        "DefinedValuesAttribute caused: Method " + methodName + " not found or returned null",
                        targetObject
                    );
                    return;
                }
            }

            var firstValue = values.FirstOrDefault(v => v != null);
            if (firstValue == null) return;

            this._objects = values;
            this._valueType = firstValue.GetType();

            if (labels != null && labels.Length == values.Length)
                this._labels = labels;
            else
                this._labels = values.Select(v => v?.ToString() ?? "NULL").ToArray();


            object[] GetValuesFromMethod() {
                var methodOwner = targetProperty.GetParent();
                if (methodOwner == null) methodOwner = targetObject;
                var type = methodOwner.GetType();
                var bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
                var methodsOnType = type.GetMethods(bindings);
                var method = methodsOnType.SingleOrDefault(m => m.Name == methodName);
                if (method == null) return null;

                try {
                    var result = method.Invoke(methodOwner, null);
                    return result as object[];
                }
                catch {
                    return null;
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            this.Initialize(property, (DefinedValuesAttribute)this.attribute);

            if (this._labels.IsNullOrEmpty() || this._valueType != this.fieldInfo.FieldType) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            bool isBool = this._valueType == typeof(bool);
            bool isString = this._valueType == typeof(string);
            bool isInt = this._valueType == typeof(int);
            bool isFloat = this._valueType == typeof(float);

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(position, label, property);
            var newIndex = EditorGUI.Popup(position, label.text, GetSelectedIndex(), this._labels);
            EditorGUI.EndProperty();
            if (EditorGUI.EndChangeCheck()) ApplyNewValue(newIndex);


            int GetSelectedIndex() {
                object value = null;
                for (var i = 0; i < this._objects.Length; i++) {
                    if (isBool && property.boolValue == Convert.ToBoolean(this._objects[i])) return i;
                    if (isString && property.stringValue == Convert.ToString(this._objects[i])) return i;
                    if (isInt && property.intValue == Convert.ToInt32(this._objects[i])) return i;
                    if (isFloat && Mathf.Approximately(property.floatValue, Convert.ToSingle(this._objects[i]))) return i;

                    if (value == null) value = property.GetValue();
                    if (value == this._objects[i]) return i;
                }

                return 0;
            }

            void ApplyNewValue(int newValueIndex) {
                var newValue = this._objects[newValueIndex];
                if (isBool) property.boolValue = Convert.ToBoolean(newValue);
                else if (isString) property.stringValue = Convert.ToString(newValue);
                else if (isInt) property.intValue = Convert.ToInt32(newValue);
                else if (isFloat) property.floatValue = Convert.ToSingle(newValue);
                else {
                    property.SetValue(newValue);
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }

                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif
