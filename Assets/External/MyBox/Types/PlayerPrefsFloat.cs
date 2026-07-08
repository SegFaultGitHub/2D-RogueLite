using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox {
    [Serializable]
    public class PlayerPrefsFloat : PlayerPrefsType {
        public float Value { get => PlayerPrefs.GetFloat(this.Key, this.DefaultValue); set => PlayerPrefs.SetFloat(this.Key, value); }
        public float DefaultValue;


        public static PlayerPrefsFloat WithKey(string key, float defaultValue = 0) => new PlayerPrefsFloat(key, defaultValue);

        public PlayerPrefsFloat(string key, float defaultValue = 0) {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
    }
}
