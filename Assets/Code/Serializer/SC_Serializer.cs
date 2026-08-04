using System;
using System.IO;
using Code.Managers;
using UnityEngine;

namespace Code.Serializer {
    public static class SC_Serializer {
        public static MB_StatsManager.C_Stats ReadGlobalStats() {
            try {
                string path = GetGlobalStatsFilePath();
                StreamReader reader = new(path);
                string json = reader.ReadToEnd();
                reader.Close();
                return JsonUtility.FromJson<MB_StatsManager.C_Stats>(json);
            }
            catch (Exception) {
                Debug.Log("Unable to read global stats");
                return new MB_StatsManager.C_Stats();
            }
        }
        public static void WriteGlobalStats(MB_StatsManager.C_Stats stats) {
            StreamWriter writer = new(GetGlobalStatsFilePath());
            string json = JsonUtility.ToJson(stats, true);
            writer.Write(json);
            writer.Close();
        }

        private static string GetGlobalStatsFilePath() => GetPersistentPath("globalStats.json");

        public static string GetPersistentPath(string path) => Path.Combine(Application.persistentDataPath, path);
    }
}
