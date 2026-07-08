using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox {
    [Serializable]
    public class PlayerPrefsVector3Int : PlayerPrefsType {
        public Vector3Int Value {
            get => new Vector3Int(
                PlayerPrefs.GetInt(this.Key + "x", this.DefaultValue.x),
                PlayerPrefs.GetInt(this.Key + "y", this.DefaultValue.y),
                PlayerPrefs.GetInt(this.Key + "z", this.DefaultValue.z)
            );
            set {
                PlayerPrefs.SetInt(this.Key + "x", value.x);
                PlayerPrefs.SetInt(this.Key + "y", value.y);
                PlayerPrefs.SetInt(this.Key + "z", value.z);
            }
        }

        public Vector3Int DefaultValue;

        public static PlayerPrefsVector3Int WithKey(string key, Vector3Int defaultValue = new Vector3Int()) =>
            new PlayerPrefsVector3Int(key, defaultValue);

        public PlayerPrefsVector3Int(string key, Vector3Int defaultValue = new Vector3Int()) {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
    }
}
