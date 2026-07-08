using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox {
    [Serializable]
    public class PlayerPrefsInt : PlayerPrefsType {
        public int Value { get => PlayerPrefs.GetInt(this.Key, this.DefaultValue); set => PlayerPrefs.SetInt(this.Key, value); }

        public int DefaultValue;

        public static PlayerPrefsInt WithKey(string key, int defaultValue = 0) => new PlayerPrefsInt(key, defaultValue);

        public PlayerPrefsInt(string key, int defaultValue = 0) {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
    }
}
