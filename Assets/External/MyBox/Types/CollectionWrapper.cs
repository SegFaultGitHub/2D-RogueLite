using System;
using System.Collections.Generic;
using MyBox.Internal;
using UnityEditor;
using UnityEngine;

namespace MyBox {
    /// <summary>
    /// CollectionWrapper used to apply custom drawers to Array fields
    /// </summary>
    [Serializable]
    public class CollectionWrapper<T> : CollectionWrapperBase {
        public T[] Value;

        public int Length => Value.Length;
        public T this[int index] {
            get => Value[index];
            set => Value[index] = value;
        }
    }
    /// <summary>
    /// CollectionWrapper used to apply custom drawers to List fields
    /// </summary>
    [Serializable]
    public class CollectionWrapperList<T> : CollectionWrapperBase {
        public List<T> Value = new List<T>();

        public int Count => Value.Count;
        public bool Contains(T value) => Value.Contains(value);
        public T this[int index] {
            get => Value[index];
            set => Value[index] = value;
        }

        public void Add(T value) => Value.Add(value);
        public void Remove(T value) => Value.Remove(value);
    }
}

namespace MyBox.Internal {
    [Serializable]
    public class CollectionWrapperBase { }
}

#if UNITY_EDITOR
namespace MyBox.Internal {
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(CollectionWrapperBase), true)]
    public class CollectionWrapperDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var collection = property.FindPropertyRelative("Value");
            return EditorGUI.GetPropertyHeight(collection, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var collection = property.FindPropertyRelative("Value");
            EditorGUI.PropertyField(position, collection, label, true);
        }
    }
}
#endif
