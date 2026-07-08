using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox {
    [Serializable]
    public class PlayerPrefsVector2 : PlayerPrefsType {
        public Vector2 Value {
            get => new Vector2(PlayerPrefs.GetFloat(this.Key + "x", this.DefaultValue.x), PlayerPrefs.GetFloat(this.Key + "y", this.DefaultValue.y));
            set {
                PlayerPrefs.SetFloat(this.Key + "x", value.x);
                PlayerPrefs.SetFloat(this.Key + "y", value.y);
            }
        }

        public Vector2 DefaultValue;

        public static PlayerPrefsVector2 WithKey(string key, Vector2 defaultValue = new Vector2()) =>
            new PlayerPrefsVector2(key, defaultValue);

        public PlayerPrefsVector2(string key, Vector2 defaultValue = new Vector2()) {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
    }
}
