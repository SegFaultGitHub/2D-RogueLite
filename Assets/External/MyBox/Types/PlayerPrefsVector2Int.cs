using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox {
    [Serializable]
    public class PlayerPrefsVector2Int : PlayerPrefsType {
        public Vector2Int Value {
            get => new Vector2Int(PlayerPrefs.GetInt(this.Key + "x", this.DefaultValue.x), PlayerPrefs.GetInt(this.Key + "y", this.DefaultValue.y));
            set {
                PlayerPrefs.SetInt(this.Key + "x", value.x);
                PlayerPrefs.SetInt(this.Key + "y", value.y);
            }
        }

        public Vector2Int DefaultValue;

        public static PlayerPrefsVector2Int WithKey(string key, Vector2Int defaultValue = new Vector2Int()) =>
            new PlayerPrefsVector2Int(key, defaultValue);

        public PlayerPrefsVector2Int(string key, Vector2Int defaultValue = new Vector2Int()) {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
    }
}
