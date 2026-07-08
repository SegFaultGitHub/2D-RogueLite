using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox {
    [Serializable]
    public class PlayerPrefsString : PlayerPrefsType {
        public string Value { get => PlayerPrefs.GetString(this.Key, this.DefaultString); set => PlayerPrefs.SetString(this.Key, value); }

        public string DefaultString;

        public static PlayerPrefsString WithKey(string key, string defaultString = "") => new PlayerPrefsString(key, defaultString);

        public PlayerPrefsString(string key, string defaultString = "") {
            this.Key = key;
            this.DefaultString = defaultString;
        }
    }
}
