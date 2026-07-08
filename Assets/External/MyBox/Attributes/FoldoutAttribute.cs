// ---------------------------------------------------------------------------- 
// Author: Dimitry, PixeyeHQ
// Project : UNITY FOLDOUT
// https://github.com/PixeyeHQ/InspectorFoldoutGroup
// Contacts : Pix - ask@pixeye.games
// Website : http://www.pixeye.games
// ----------------------------------------------------------------------------

using UnityEngine;

namespace MyBox {
    public class FoldoutAttribute : PropertyAttribute {
        public readonly string Name;
        public readonly bool FoldEverything;

        /// <summary>Adds the property to the specified foldout group.</summary>
        /// <param name="name">Name of the foldout group.</param>
        /// <param name="foldEverything">Toggle to put all properties to the specified group</param>
        public FoldoutAttribute(string name, bool foldEverything = false) {
            this.FoldEverything = foldEverything;
            this.Name = name;
        }
    }
}

#if UNITY_EDITOR
namespace MyBox.Internal {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;

    public class FoldoutAttributeHandler {
        private readonly Dictionary<string, CacheFoldProp> _cacheFoldouts = new Dictionary<string, CacheFoldProp>();
        private readonly List<SerializedProperty> _props = new List<SerializedProperty>();
        private bool _initialized;

        private readonly UnityEngine.Object _target;
        private readonly SerializedObject _serializedObject;

        public bool OverrideInspector => this._props.Count > 0;

        public FoldoutAttributeHandler(UnityEngine.Object target, SerializedObject serializedObject) {
            this._target = target;
            this._serializedObject = serializedObject;
        }

        public void OnDisable() {
            if (this._target == null) return;

            foreach (var c in this._cacheFoldouts) {
                EditorPrefs.SetBool(string.Format($"{c.Value.Attribute.Name}{c.Value.Properties[0].name}{this._target.name}"), c.Value.Expanded);
                c.Value.Dispose();
            }
        }

        public void Update() {
            this._serializedObject.Update();
            this.Setup();
        }

        public void OnInspectorGUI() {
            this.Header();
            this.Body();

            this._serializedObject.ApplyModifiedProperties();
        }

        private void Header() {
            using (new EditorGUI.DisabledScope("m_Script" == this._props[0].propertyPath)) {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(this._props[0], true);
                EditorGUILayout.Space();
            }
        }

        private void Body() {
            foreach (var pair in this._cacheFoldouts) {
                EditorGUILayout.BeginVertical(StyleFramework.Box);
                this.Foldout(pair.Value);
                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel = 0;
            }

            EditorGUILayout.Space();

            for (var i = 1; i < this._props.Count; i++) {
                EditorGUILayout.PropertyField(this._props[i], true);
            }

            EditorGUILayout.Space();
        }

        private void Foldout(CacheFoldProp cache) {
            cache.Expanded = EditorGUILayout.Foldout(cache.Expanded, cache.Attribute.Name, true, StyleFramework.FoldoutHeader);
            var rect = GUILayoutUtility.GetLastRect();
            rect.x -= 18;
            rect.y -= 4;
            rect.height += 8;
            rect.width += 22;
            EditorGUI.LabelField(rect, GUIContent.none, EditorStyles.helpBox);

            if (cache.Expanded) {
                EditorGUILayout.Space(2);

                foreach (var property in cache.Properties) {
                    EditorGUILayout.BeginVertical(StyleFramework.BoxChild);
                    EditorGUILayout.PropertyField(property, new GUIContent(ObjectNames.NicifyVariableName(property.name)), true);
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void Setup() {
            if (this._initialized) return;

            FoldoutAttribute prevFold = default;

            var length = EditorTypes.Get(this._target, out var objectFields);

            for (var i = 0; i < length; i++) {
                #region FOLDERS
                var fold = Attribute.GetCustomAttribute(objectFields[i], typeof(FoldoutAttribute)) as FoldoutAttribute;
                CacheFoldProp c;
                if (fold == null) {
                    if (prevFold != null && prevFold.FoldEverything) {
                        if (!this._cacheFoldouts.TryGetValue(prevFold.Name, out c)) {
                            this._cacheFoldouts.Add(
                                prevFold.Name,
                                new CacheFoldProp { Attribute = prevFold, Types = new HashSet<string> { objectFields[i].Name } }
                            );
                        } else {
                            c.Types.Add(objectFields[i].Name);
                        }
                    }

                    continue;
                }

                prevFold = fold;

                if (!this._cacheFoldouts.TryGetValue(fold.Name, out c)) {
                    var expanded = EditorPrefs.GetBool(string.Format($"{fold.Name}{objectFields[i].Name}{this._target.name}"), false);
                    this._cacheFoldouts.Add(
                        fold.Name,
                        new CacheFoldProp { Attribute = fold, Types = new HashSet<string> { objectFields[i].Name }, Expanded = expanded }
                    );
                } else c.Types.Add(objectFields[i].Name);
                #endregion
            }

            var property = this._serializedObject.GetIterator();
            var next = property.NextVisible(true);
            if (next) {
                do {
                    this.HandleFoldProp(property);
                } while (property.NextVisible(false));
            }

            this._initialized = true;
        }

        private void HandleFoldProp(SerializedProperty prop) {
            bool shouldBeFolded = false;

            foreach (var pair in this._cacheFoldouts) {
                if (pair.Value.Types.Contains(prop.name)) {
                    var pr = prop.Copy();
                    shouldBeFolded = true;
                    pair.Value.Properties.Add(pr);

                    break;
                }
            }

            if (shouldBeFolded == false) {
                var pr = prop.Copy();
                this._props.Add(pr);
            }
        }

        private class CacheFoldProp {
            public HashSet<string> Types = new HashSet<string>();
            public readonly List<SerializedProperty> Properties = new List<SerializedProperty>();
            public FoldoutAttribute Attribute;
            public bool Expanded;

            public void Dispose() {
                this.Properties.Clear();
                this.Types.Clear();
                this.Attribute = null;
            }
        }
    }


    static class StyleFramework {
        public static readonly GUIStyle Box;
        public static readonly GUIStyle BoxChild;
        public static readonly GUIStyle FoldoutHeader;

        static StyleFramework() {
            FoldoutHeader = new GUIStyle(EditorStyles.foldout);
            FoldoutHeader.overflow = new RectOffset(-10, 0, 3, 0);
            FoldoutHeader.padding = new RectOffset(20, 0, 0, 0);
            FoldoutHeader.border = new RectOffset(2, 2, 2, 2);

            Box = new GUIStyle(GUI.skin.box);
            Box.padding = new RectOffset(18, 0, 4, 4);

            BoxChild = new GUIStyle(GUI.skin.box);
        }
    }

    static class EditorTypes {
        private static readonly Dictionary<int, List<FieldInfo>> Fields = new Dictionary<int, List<FieldInfo>>(FastComparable.Default);

        public static int Get(Object target, out List<FieldInfo> objectFields) {
            var t = target.GetType();
            var hash = t.GetHashCode();

            if (!Fields.TryGetValue(hash, out objectFields)) {
                var typeTree = GetTypeTree(t);
                objectFields = target.GetType()
                    .GetFields(
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.NonPublic
                    )
                    .OrderByDescending(x => typeTree.IndexOf(x.DeclaringType))
                    .ToList();
                Fields.Add(hash, objectFields);
            }

            return objectFields.Count;
        }

        static IList<Type> GetTypeTree(Type t) {
            var types = new List<Type>();
            while (t.BaseType != null) {
                types.Add(t);
                t = t.BaseType;
            }

            return types;
        }
    }


    internal class FastComparable : IEqualityComparer<int> {
        public static readonly FastComparable Default = new FastComparable();

        public bool Equals(int x, int y) {
            return x == y;
        }

        public int GetHashCode(int obj) {
            return obj.GetHashCode();
        }
    }
}
#endif
