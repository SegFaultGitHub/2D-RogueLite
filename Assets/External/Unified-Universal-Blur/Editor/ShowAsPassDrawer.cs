using System;
using System.Collections.Generic;
using System.Reflection;
using Unified.UniversalBlur.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unified.UniversalBlur.Editor
{
    [CustomPropertyDrawer(typeof(ShowAsPass))]
    public class ShowAsPassDrawer : PropertyDrawer
    {
        private Type _targetType;
        private FieldInfo _targetField;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            ShowAsPass passAttribute = (ShowAsPass)this.attribute;

            Object target = property.serializedObject.targetObject;
            string targetMaterialField = passAttribute.TargetMaterialField;

            this._targetType ??= target.GetType();
            this._targetField ??= this._targetType.GetField(targetMaterialField, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (this._targetField != null)
            {
                object fieldValue = this._targetField.GetValue(target);
            
                Material material = fieldValue as Material;

                if (material != null)
                {
                    List<GUIContent> selectablePasses = this.GetPassIndexStringEntries(material);
                    int choiceIndex = EditorGUI.Popup(position, label, property.intValue, selectablePasses.ToArray());
                    
                    property.intValue = choiceIndex;
                }
                else
                {
                    EditorGUI.HelpBox(position, $"Incorrect target field or Material not set.", MessageType.Error);
                }
            }
            else
            {
                EditorGUI.HelpBox(position, $"Field {targetMaterialField} not found on {this._targetType.Name}.", MessageType.Error);
            }
            
            EditorGUI.EndProperty();
        }
        
        private List<GUIContent> GetPassIndexStringEntries(Material material)
        {
            List<GUIContent> passIndexEntries = new List<GUIContent>();
            for (int i = 0; i < material.passCount; ++i)
            {
                string entry = $"{material.GetPassName(i)} ({i})";
                passIndexEntries.Add(new GUIContent(entry));
            }

            return passIndexEntries;
        }
    }
}
