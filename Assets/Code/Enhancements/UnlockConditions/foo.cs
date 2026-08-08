using System;
using System.Text;
using UnityEngine;
namespace Code.Enhancements.UnlockConditions {
    public class SerializeReferenceLinkedListExample : MonoBehaviour
    {
        // This example shows a linked list structure with a single int per Node.
        // This would be much more efficiently represented using a List<int>, without any SerializeReference needed.
        // But it demonstrates an approach that can be extended for trees and other more advanced graphs

        [Serializable]
        public class Node
        {
            // This field must use serialize reference so that serialization can store
            // a reference to another Node object, or null.  By-value
            // can never properly represent this sort of self-referencing structure.
            [SerializeReference]
            public Node m_Next = null;

            public int m_Data = 1;
        }

        [SerializeReference]
        public Node m_Front = null;

        // Points to the last node in the list.  This is an
        // example of a having more than one field pointing to a single Node
        // object, which cannot be done with "by-value" serialization
        [SerializeReference]
        public Node m_End = null;

        SerializeReferenceLinkedListExample()
        {
            this.AddEntry(1);
            this.AddEntry(3);
            this.AddEntry(9);
            this.AddEntry(81);
            this.PrintList();
        }

        private void AddEntry(int data)
        {
            if (this.m_Front == null)
            {
                this.m_Front = new Node() {m_Data = data};
                this.m_End = this.m_Front;
            }
            else
            {
                this.m_End.m_Next = new Node() {m_Data = data};
                this.m_End = this.m_End.m_Next;
            }
        }

        private void PrintList()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Link list contents: ");
            Node position = this.m_Front;
            while (position != null)
            {
                sb.Append("  Node data " + position.m_Data).AppendLine();
                position = position.m_Next;
            }
            Debug.Log(sb.ToString());
        }
    }
}
