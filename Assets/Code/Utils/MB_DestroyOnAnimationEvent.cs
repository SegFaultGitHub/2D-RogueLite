using System.Collections;
using System.Linq;
using UnityEngine;

namespace Code.Utils {
    public class MB_DestroyOnAnimationEvent : MonoBehaviour {
        public void Destroy() {
            this.StartCoroutine(this.DestroyWhenFinished());
        }

        private IEnumerator DestroyWhenFinished() {
            TrailRenderer[] trails = this.GetComponentsInChildren<TrailRenderer>();
            ParticleSystem[] particles = this.GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem particle in particles) particle.Stop();

            foreach (TrailRenderer trail in trails) trail.emitting = false;
            float trailDuration = trails.Select(trail => trail.time).Prepend(0).Max();

            yield return new WaitForSeconds(trailDuration);
            yield return new WaitUntil(() => particles.All(particle => particle.particleCount == 0));

            Destroy(this.gameObject);
        }
    }
}
