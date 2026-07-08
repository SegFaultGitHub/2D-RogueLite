using UnityEngine;

namespace MyBox {
    [ExecuteAlways]
    public class Billboard : MonoBehaviour {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize() {
            _camera = null;
        }

        public Transform FacedObject;

        private Transform ActiveFacedObject {
            get {
                if (this.FacedObject != null) return this.FacedObject;
                if (_camera != null) return _camera.transform;
                _camera = Camera.main;

                return _camera == null
                    ? null
                    : _camera.transform;
            }
        }
        private static Camera _camera;

        private void Update() {
            if (this.ActiveFacedObject == null) return;
            this.transform.LookAt(this.ActiveFacedObject);
        }
    }
}
