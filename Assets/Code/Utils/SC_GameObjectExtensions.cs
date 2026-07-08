using UnityEngine;

namespace Code.Utils {
    public static class SC_GameObjectExtensions {
        /// <summary>
        /// Returns the full hierarchy name of the game object.
        /// </summary>
        /// <param name="go">The game object.</param>
        public static string GetFullName(this GameObject go) {
            string name = go.name;
            while (go.transform.parent != null) {
                go = go.transform.parent.gameObject;
                name = go.name + "/" + name;
            }

            return name;
        }
    }
}
