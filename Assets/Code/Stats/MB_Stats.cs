using System.Collections.Generic;
using Code.Characters.Enemies;
using UnityEngine;

namespace Code.Stats {
    public class MB_Stats : MonoBehaviour {
        public class SC_Stats {
            public Dictionary<E_Enemy, int> EnemiesKilled;

            public string Serialize() {
                return default;
            }
        }


        public void Serialize() { }
    }
}
