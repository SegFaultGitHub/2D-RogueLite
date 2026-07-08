using System;
using UnityEngine;

namespace MyBox.Internal {
    [Serializable]
    public abstract class PlayerPrefsType {
        public string Key { get; protected set; }

        public bool IsSet => PlayerPrefs.HasKey(this.Key);

        public void Delete() => PlayerPrefs.DeleteKey(this.Key);
    }
}
