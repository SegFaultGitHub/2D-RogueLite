using System;
using System.Collections.Generic;
using MyBox.Internal;
using UnityEngine;

namespace MyBox
{
	/// <summary>
	/// CollectionWrapper used to apply custom drawers to Array fields
	/// </summary>
	[Serializable]
	public class CollectionWrapper<T> : CollectionWrapperBase
	{
		public T[] Value;
        public int Length => this.Value.Length;
        public T this[int index] { get => this.Value[index]; set => this.Value[index] = value; }
    }
 	/// <summary>
	/// CollectionWrapper used to apply custom drawers to List fields
	/// </summary>
 	[Serializable]
	public class CollectionWrapperList<T> : CollectionWrapperBase
	{
		public List<T> Value = new List<T>();
        public int Count => this.Value.Count;

        public void Add(T t) => this.Value.Add(t);

        public void Remove(T t) => this.Value.Remove(t);
        public T this[int i] { get => this.Value[i]; set => this.Value[i] = value; }

        public bool Contains(T t) => this.Value.Contains(t);
    }
}

namespace MyBox.Internal
{
	[Serializable]
	public class CollectionWrapperBase {}
}

#if UNITY_EDITOR
namespace MyBox.Internal
{
	using UnityEditor;
	using UnityEngine;
	
	[CustomPropertyDrawer(typeof(CollectionWrapperBase), true)]
	public class CollectionWrapperDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var collection = property.FindPropertyRelative("Value");
			return EditorGUI.GetPropertyHeight(collection, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var collection = property.FindPropertyRelative("Value");
			EditorGUI.PropertyField(position, collection, label, true);
		}
	}
}
#endif
