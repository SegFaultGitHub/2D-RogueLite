using UnityEngine;

namespace Code.Utils {
    public class MB_DisableForAndroid : MonoBehaviour {
        #region Members
        // [Foldout("MB_DisableForAndroid", true)]
        #endregion

        #region Getters / Setters
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        private void Start() {
            #if !UNITY_ANDROID
            Destroy(this.gameObject);
            #endif
        }
        #endregion
    }
}
