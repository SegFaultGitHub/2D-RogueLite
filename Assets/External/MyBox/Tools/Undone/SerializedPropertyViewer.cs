#if UNITY_EDITOR
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace MyBox.Internal {
    //TODO: Make it prettier and add to Docs
    public class SerializedPropertyViewer : EditorWindow {
        private const string MenuItem = "Show Serialized Properties";

        [MenuItem("CONTEXT/Component/" + MenuItem)]
        static void ContextMenuItem(MenuCommand command) {
            OpenPropertyViewer(command.context);
        }

        [MenuItem("Assets/" + MenuItem)]
        private static void AssetsMenuItem() {
            OpenPropertyViewer(Selection.activeObject);
        }

        [MenuItem("Assets/" + MenuItem, true)]
        private static bool AssetsMenuItemValidator() {
            return Selection.activeObject != null;
        }

        private static void OpenPropertyViewer(Object target) {
            var propertyViewer = (SerializedPropertyViewer)GetWindow(typeof(SerializedPropertyViewer));
            propertyViewer.titleContent = new GUIContent("Property Explorer");

            propertyViewer.Initialize(target);
            propertyViewer.Show();
        }


        private struct PropertyData {
            public readonly int Depth;
            public readonly string Info;
            public readonly int ObjectId;

            public PropertyData(int depth, string info, int objectId) {
                if (depth < 0) depth = 0;
                this.Depth = depth;
                this.Info = info;
                this.ObjectId = objectId;
            }
        }


        private Object _target;
        private SerializedObject _targetSO;
        private readonly List<PropertyData> _propertiesData = new List<PropertyData>();


        private bool _debugMode;
        private string _searchString = string.Empty;
        private string _highlightedSearchString = string.Empty;
        private Vector2 _scrollPos;


        private static GUIStyle _richTextStyle;

        private void Initialize(Object target) {
            this._target = target;
            this._targetSO = new SerializedObject(this._target);

            if (_richTextStyle == null) {
                _richTextStyle = new GUIStyle(EditorStyles.label);
                _richTextStyle.richText = true;
            }

            this.CollectProperties();
        }


        private void OnGUI() {
            if (this._target == null)
                this.Close();
            if (this._targetSO == null || this._propertiesData.Count == 0 || _richTextStyle == null)
                this.Initialize(this._target);

            var debug = EditorGUILayout.Toggle("Debug Mode", this._debugMode);
            if (debug != this._debugMode) {
                this._debugMode = debug;
                this.CollectProperties();
            }

            string searchString = EditorGUILayout.TextField("Search:", this._searchString);
            if (searchString != this._searchString) {
                this._searchString = searchString;
                this._highlightedSearchString = "<color=olive><b>" + this._searchString + "</b></color>";
                this.CollectProperties();
            }


            this._scrollPos = EditorGUILayout.BeginScrollView(this._scrollPos);
            foreach (PropertyData property in this._propertiesData) {
                EditorGUI.indentLevel = property.Depth;
                if (property.ObjectId > 0) {
                    GUILayout.BeginHorizontal();
                }

                EditorGUILayout.SelectableLabel(property.Info, _richTextStyle, GUILayout.Height(20));
                if (property.ObjectId > 0) {
                    if (GUILayout.Button(">Ping>", GUILayout.Width(50))) {
                        Selection.activeInstanceID = property.ObjectId;
                    }

                    GUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();


            if (GUILayout.Button("Copy To Clipboard")) {
                StringBuilder sb = new StringBuilder();
                Dictionary<int, string> paddingHash = new Dictionary<int, string>();
                string padding = "";
                for (int i = 0; i < 40; i++) {
                    paddingHash[i] = padding;
                    padding += " ";
                }

                foreach (PropertyData line in this._propertiesData) {
                    sb.Append(paddingHash[line.Depth]);
                    sb.Append(line.Info);
                    sb.Append("\n");
                }

                EditorGUIUtility.systemCopyBuffer = sb.ToString();
            }
        }

        private void CollectProperties() {
            this._propertiesData.Clear();
            if (this._targetSO == null) return;

            var inspectorMode = this._debugMode
                ? InspectorMode.Debug
                : InspectorMode.Normal;
            PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty(
                "inspectorMode",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            if (inspectorModeInfo != null) inspectorModeInfo.SetValue(this._targetSO, inspectorMode, null);


            var iterator = this._targetSO.GetIterator();

            this.LogPropertyData(iterator);
            while (iterator.Next(true)) {
                this.LogPropertyData(iterator);
            }
        }

        private readonly StringBuilder _descriptionBuilder = new StringBuilder();

        private void LogPropertyData(SerializedProperty property) {
            var value = this.AsStringValue(property);

            // This ugly construction is to prevent massive allocations and process selection on all text except <color> tags
            this._descriptionBuilder.Length = 0;
            this._descriptionBuilder.Append(property.propertyPath).Append(this.ProcessSelection(" — Type: "));
            if (property.isArray)
                this._descriptionBuilder.Append("<color=maroon>").Append(this.ProcessSelection("[Array]")).Append("</color> ");
            this._descriptionBuilder.Append("<color=blue>").Append(this.ProcessSelection(property.type)).Append("</color>");

            this._descriptionBuilder.Append(this.ProcessSelection(", Name:"))
                .Append("<color=green>")
                .Append(this.ProcessSelection(property.name))
                .Append("</color>");
            if (!string.IsNullOrEmpty(value)) {
                this._descriptionBuilder.Append(this.ProcessSelection(", Value:"))
                    .Append("<color=navy>")
                    .Append(this.ProcessSelection(value))
                    .Append("</color>");
            }


            string description = this._descriptionBuilder.ToString();


            bool isObject = property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null;
            int propertyId = isObject
                ? property.objectReferenceValue.GetInstanceID()
                : 0;


            this._propertiesData.Add(new PropertyData(property.depth, description, propertyId));
        }

        private string ProcessSelection(string str) {
            if (this._searchString.Length <= 0) return str;
            return str.Replace(this._searchString, this._highlightedSearchString);
        }


        private string AsStringValue(SerializedProperty property) {
            switch (property.propertyType) {
                case SerializedPropertyType.String:
                    return property.stringValue;

                case SerializedPropertyType.Character:
                case SerializedPropertyType.Integer:
                    if (property.type == "char") {
                        if (property.intValue < char.MinValue || property.intValue > char.MaxValue)
                            return "Char with invalid value of " + property.intValue;
                        return System.Convert.ToChar(property.intValue).ToString();
                    }

                    return property.intValue.ToString();

                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue != null
                        ? property.objectReferenceValue.ToString()
                        : "null";

                case SerializedPropertyType.Float:
                    return property.floatValue.ToString(CultureInfo.InvariantCulture);

                case SerializedPropertyType.Boolean:
                    return property.boolValue.ToString();

                case SerializedPropertyType.Enum:
                    return property.enumNames[property.enumValueIndex];

                default:
                    return string.Empty;
            }
        }
    }
}
#endif
