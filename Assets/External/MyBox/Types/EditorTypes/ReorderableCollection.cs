#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MyBox.EditorTools {
    public class ReorderableCollection {
        public bool IsExpanded { get => this._property.isExpanded; set => this._property.isExpanded = value; }

        public float Height =>
            this._property.isExpanded
                ? this._list.GetHeight()
                : EditorGUIUtility.singleLineHeight + 12;

        public SerializedProperty Property => this._property;
        public ReorderableList ReorderableList => this._list;

        public void Draw() {
            if (this._property.isExpanded) {
                var headerRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true));
                headerRect.height = 20;
                this._list.DoLayoutList();
                this.DrawHeader(headerRect);
            } else
                this.DrawHeader(GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true)));
        }

        public void Draw(Rect rect) {
            if (this._property.isExpanded)
                this._list.DoList(rect);
            this.DrawHeader(rect);
        }



        public Action<SerializedProperty, Rect, int> CustomDrawer;

        /// <summary>
        /// Return Height, Receive element index.
        /// Use EditorApplication.delayCall to perform custom logic
        /// </summary>
        public Func<int, int> CustomDrawerHeight;

        /// <summary>
        /// Return false to perform default logic, Receive element index.
        /// Use EditorApplication.delayCall to perform custom logic.
        /// </summary>
        public Func<int, bool> CustomAdd;

        /// <summary>
        /// Return false to perform default logic, Receive element index.
        /// Use EditorApplication.delayCall to perform custom logic.
        /// </summary>
        public Func<int, bool> CustomRemove;

        public Func<int, string> CustomElementName;


        private ReorderableList _list;
        private SerializedProperty _property;
        private readonly string _customHeader;

        public ReorderableCollection(
            SerializedProperty property,
            bool withAddButton = true,
            bool withRemoveButton = true,
            string customHeader = null
        ) {
            this._property = property;
            this._property.isExpanded = true;
            this._customHeader = customHeader;

            this.CreateList(property, withAddButton, withRemoveButton, customHeader == null);
        }

        ~ReorderableCollection() {
            this._property = null;
            this._list = null;
        }

        private void DrawHeader(Rect rect) {
            var headerRect = new Rect(rect);
            headerRect.height = EditorGUIUtility.singleLineHeight;

            ReorderableList.defaultBehaviours.DrawHeaderBackground(headerRect);

            headerRect.width -= 50;
            headerRect.x += 6;
            headerRect.y += 2;
            this._property.isExpanded = EditorGUI.ToggleLeft(
                headerRect,
                (this._customHeader != null
                    ? this._customHeader
                    : this._property.displayName)
                + "["
                + this._property.arraySize
                + "]",
                this._property.isExpanded,
                EditorStyles.boldLabel
            );
        }

        private void CreateList(SerializedProperty property, bool withAddButton, bool withRemoveButton, bool displayHeader) {
            this._list = new ReorderableList(property.serializedObject, property, true, displayHeader, withAddButton, withRemoveButton);
            this._list.onChangedCallback += list => this.Apply();
            this._list.onAddCallback += this.AddElement;
            this._list.onRemoveCallback += this.RemoveElement;
            this._list.onCanRemoveCallback += (list) => this._list.count > 0;
            this._list.drawElementCallback += this.DrawElement;
            this._list.elementHeightCallback += this.GetElementHeight;
        }

        private void AddElement(ReorderableList list) {
            if (this.CustomAdd == null || !this.CustomAdd(this._property.arraySize))
                ReorderableList.defaultBehaviours.DoAddButton(list);
        }

        private void RemoveElement(ReorderableList list) {
            if (this.CustomRemove == null || !this.CustomRemove(list.index))
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }

        private void DrawElement(Rect rect, int index, bool active, bool focused) {
            EditorGUI.BeginChangeCheck();

            var property = this._property.GetArrayElementAtIndex(index);
            rect.height = this.GetElementHeight(index);
            rect.y += 1;

            if (this.CustomDrawer != null)
                this.CustomDrawer(property, rect, index);
            else {
                var element = this._property.GetArrayElementAtIndex(index);

                if (element.propertyType == SerializedPropertyType.Generic) {
                    rect.x += 12;
                    rect.width -= 14;
                    var genericsLabel = rect;
                    genericsLabel.height = EditorGUIUtility.singleLineHeight;

                    string displayName = this.CustomElementName != null
                        ? this.CustomElementName.Invoke(index)
                        : element.displayName;
                    EditorGUI.LabelField(genericsLabel, displayName);
                }

                EditorGUI.PropertyField(rect, property, GUIContent.none, true);
            }

            this._list.elementHeight = rect.height + 4.0f;
            if (EditorGUI.EndChangeCheck())
                this.Apply();
        }

        private float GetElementHeight(int index) {
            if (this.CustomDrawerHeight != null) return this.CustomDrawerHeight(index);

            var element = this._property.GetArrayElementAtIndex(index);
            var height = EditorGUI.GetPropertyHeight(element, GUIContent.none, true);
            return Mathf.Max(EditorGUIUtility.singleLineHeight, height + 4.0f);
        }

        private void Apply() {
            this._property.serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
