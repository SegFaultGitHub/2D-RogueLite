using System;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.UI {
    public class MB_CircleRenderer : MonoBehaviour {
        #region Members
        [Foldout("MB_CircleRenderer", true)]
        [SerializeField] private protected Transform m_Circle;
        [SerializeField] private protected LineRenderer m_LineRenderer;
        #endregion

        #region Getters / Setters
        private Transform Circle { get => this.m_Circle; }
        private LineRenderer LineRenderer { get => this.m_LineRenderer; set => this.m_LineRenderer = value; }
        #endregion

        #region Static / Readonly / Const
        private const int STEPS = 36;
        #endregion

        #region Unity methods
        #endregion

        public void Run(float size, float duration) {
            this.SetPermimeterSize(size);
            DOTween.To( //
                    () => 0f,
                    this.SetCircleSize,
                    size * 2f,
                    duration
                )
                .SetEase(Ease.Linear);
            DOTween.To( //
                    () => 1f,
                    this.SetPermiterWidth,
                    0f,
                    duration
                )
                .SetEase(Ease.Linear);

            this.InSeconds(
                duration,
                () => {
                    DOTween.To( //
                            () => size * 2f,
                            this.SetCircleSize,
                            0f,
                            .5f
                        )
                        .SetEase(Ease.OutQuad);
                }
            );
        }

        private void SetPermimeterSize(float radius) {
            if (radius == 0) {
                this.LineRenderer.positionCount = 0;
                return;
            }

            Vector2 center = this.transform.position;
            this.LineRenderer.positionCount = STEPS + 1;

            for (int i = 0; i <= STEPS; i++) {
                float angle = (float)Math.PI * 2 / STEPS * i;
                this.LineRenderer.SetPosition(i, new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius) + center);
            }
        }

        private void SetPermiterWidth(float width) {
            this.LineRenderer.widthMultiplier = width;
        }

        private void SetCircleSize(float radius) {
            this.Circle.localScale = new Vector3(radius, radius, radius);
        }
    }
}
