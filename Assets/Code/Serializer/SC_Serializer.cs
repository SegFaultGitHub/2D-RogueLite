using System.IO;
using UnityEngine;

namespace Code.Serializer {
    public static class SC_Serializer {
        public static string GetPersistentPath(string path) => Path.Combine(Application.persistentDataPath, path);
    }
}
