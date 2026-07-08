using UnityEngine;

namespace MyBox {
    public class StateOnAwake : MonoBehaviour {
        public bool Active;
        public GameObject TargetObject;
        public Renderer TargetComponent;

        private void Awake() {
            if (this.TargetObject != null)
                this.TargetObject.SetActive(this.Active);
            if (this.TargetComponent != null) {
                //TODO: enabled field through reflection..? Renderer is not a behaviour -_-
                this.TargetComponent.enabled = this.Active;
            }
        }
    }
}
