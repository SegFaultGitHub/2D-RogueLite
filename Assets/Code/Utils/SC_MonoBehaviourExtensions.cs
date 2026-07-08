using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Utils {
    public static class SC_MonoBehaviourExtensions {
        // Runs the Callback at the end of the current frame, after GUI rendering
        public static Coroutine OnEndOfFrame(this MonoBehaviour self, UnityAction callback) {
            return self.StartCoroutine(EndOfFrameCoroutine(callback));
        }

        public static IEnumerator EndOfFrameCoroutine(UnityAction callback) {
            yield return new WaitForEndOfFrame();
            callback?.Invoke();
        }

        // Runs the Callback after the next Update completes
        public static Coroutine OnUpdate(this MonoBehaviour self, UnityAction callback) {
            return self.InUpdates(1, callback);
        }

        // Runs the Callback after a given number of Updates complete
        public static Coroutine InUpdates(this MonoBehaviour self, int updates, UnityAction callback) {
            return self.StartCoroutine(InUpdatesCoroutine(updates, callback));
        }

        public static IEnumerator InUpdatesCoroutine(int updates, UnityAction callback) {
            for (int i = 0; i < updates; i++) {
                yield return null;
            }

            callback?.Invoke();
        }

        // Runs the Callback after the next FixedUpdate completes
        public static Coroutine OnFixedUpdate(this MonoBehaviour self, UnityAction callback) {
            return self.InFixedUpdates(1, callback);
        }

        // Runs the Callback after a given number of FixedUpdates complete
        public static Coroutine InFixedUpdates(this MonoBehaviour self, int ticks, UnityAction callback) {
            return self.StartCoroutine(InFixedUpdatesCoroutine(ticks, callback));
        }

        public static IEnumerator InFixedUpdatesCoroutine(int ticks, UnityAction callback) {
            for (int i = 0; i < ticks; i++) {
                yield return new WaitForFixedUpdate();
            }

            callback?.Invoke();
        }

        // Runs the Callback after a given number of seconds, after the Update completes
        public static Coroutine InSeconds(this MonoBehaviour self, float seconds, UnityAction callback) {
            return self.StartCoroutine(InSecondsCoroutine(seconds, callback));
        }

        private static IEnumerator InSecondsCoroutine(float seconds, UnityAction callback) {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }

        // Runs the Callback after a given number of real seconds, after the Update completes
        public static Coroutine InRealSeconds(this MonoBehaviour self, float seconds, UnityAction callback) {
            return self.StartCoroutine(InRealSecondsCoroutine(seconds, callback));
        }

        private static IEnumerator InRealSecondsCoroutine(float seconds, UnityAction callback) {
            yield return new WaitForSecondsRealtime(seconds);
            callback?.Invoke();
        }

        public static Coroutine Until(this MonoBehaviour self, Func<bool> callback, UnityAction action) {
            return self.StartCoroutine(UntilCoroutine(callback, action));
        }

        private static IEnumerator UntilCoroutine(Func<bool> callback, UnityAction action) {
            yield return new WaitUntil(() => callback());
            action();
        }

        public static Coroutine While(this MonoBehaviour self, Func<bool> callback, UnityAction action) {
            return self.StartCoroutine(WhileCoroutine(callback, action));
        }

        private static IEnumerator WhileCoroutine(Func<bool> callback, UnityAction action) {
            yield return new WaitWhile(callback);
            action();
        }

        public static bool IsPrefab(this MonoBehaviour self) {
            return self != null && !self.gameObject.scene.IsValid();
        }
    }
}
