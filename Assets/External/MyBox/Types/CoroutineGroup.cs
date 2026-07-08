using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBox {
    public class CoroutineGroup {
        public int ActiveCoroutinesAmount => this._activeCoroutines.Count;
        public bool AnyProcessing => this._activeCoroutines.Count > 0;

        public CoroutineGroup(MonoBehaviour owner) {
            this._owner = owner;
        }

        public Coroutine StartCoroutine(IEnumerator coroutine) {
            return this._owner.StartCoroutine(this.DoStart(coroutine));
        }

        public void StopAll() {
            for (var i = 0; i < this._activeCoroutines.Count; i++)
                this._owner.StopCoroutine(this._activeCoroutines[i]);
        }

        private readonly MonoBehaviour _owner;
        private readonly List<Coroutine> _activeCoroutines = new List<Coroutine>();

        private IEnumerator DoStart(IEnumerator coroutine) {
            var started = this._owner.StartCoroutine(coroutine);

            this._activeCoroutines.Add(started);
            yield return started;
            this._activeCoroutines.Remove(started);
        }
    }
}
