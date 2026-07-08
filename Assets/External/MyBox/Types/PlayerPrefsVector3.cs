using System;
using MyBox.Internal;
using UnityEngine;

namespace MyBox {
    [Serializable]
    public class PlayerPrefsVector3 : PlayerPrefsType {
        public Vector3 Value {
            get => new Vector3(
                PlayerPrefs.GetFloat(this.Key + "x", this.DefaultValue.x),
                PlayerPrefs.GetFloat(this.Key + "y", this.DefaultValue.y),
                PlayerPrefs.GetFloat(this.Key + "z", this.DefaultValue.z)
            );
            set {
                PlayerPrefs.SetFloat(this.Key + "x", value.x);
                PlayerPrefs.SetFloat(this.Key + "y", value.y);
                PlayerPrefs.SetFloat(this.Key + "z", value.z);
            }
        }
        public Vector3 DefaultValue;

        public static PlayerPrefsVector3 WithKey(string key, Vector3 defaultValue = new Vector3()) =>
            new PlayerPrefsVector3(key, defaultValue);

        public PlayerPrefsVector3(string key, Vector3 defaultValue = new Vector3()) {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
    }
}
