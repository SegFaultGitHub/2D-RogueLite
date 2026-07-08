using System;
using System.Collections.Generic;
using System.Linq;

namespace Code.Utils {
    public static class SC_TopologicalSort {
        public class C_CyclicDependencyException : Exception { }

        public class C_Node<T> {
            public T Object;
            public List<C_Node<T>> Dependencies;

            public bool TemporaryMarked, PermanentlyMarked;

            public C_Node() {
                this.Dependencies = new List<C_Node<T>>();
            }
        }

        public static List<T> Sort<T>(List<C_Node<T>> nodes) {
            List<T> result = new();

            void Visit(C_Node<T> node) {
                if (node.PermanentlyMarked) return;
                if (node.TemporaryMarked) throw new C_CyclicDependencyException();

                node.TemporaryMarked = true;
                foreach (C_Node<T> dependency in node.Dependencies) {
                    Visit(dependency);
                }

                node.TemporaryMarked = false;
                node.PermanentlyMarked = true;
                result.Add(node.Object);
            }

            while (nodes.Any(node => !node.PermanentlyMarked)) {
                Visit(nodes.Find(node => !node.PermanentlyMarked));
            }

            return result;
        }
    }
}
