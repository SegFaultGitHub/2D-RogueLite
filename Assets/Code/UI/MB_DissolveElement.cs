using System;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class MB_DissolveElement : MonoBehaviour {
        #region Members
        [Foldout("MB_DissolveElement", true)]
        [SerializeField] private protected RawImage m_RawImage;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Canvas m_Canvas;
        #endregion

        #region Getters / Setters
        private RawImage RawImage { get => this.m_RawImage; }

        public Canvas Canvas { get => this.m_Canvas; set => this.m_Canvas = value; }
        #endregion

        #region Static / Readonly / Const
        private static readonly int WIDTH = Shader.PropertyToID("_Width");
        private static readonly int HEIGHT = Shader.PropertyToID("_Height");
        private static readonly int DISSOLVE_AMOUNT = Shader.PropertyToID("_DissolveAmount");
        #endregion

        #region Unity methods
        private void Awake() {
            this.RawImage.material = new Material(this.RawImage.material);
        }

        private void Update() {
            Rect rect = this.Canvas.GetComponent<RectTransform>().rect;
            this.RawImage.material.SetFloat(WIDTH, rect.width);
            this.RawImage.material.SetFloat(HEIGHT, rect.height);
        }
        #endregion

        public void SetTexture(RenderTexture texture) {
            this.RawImage.texture = texture;
        }

        public void Show(float duration, Action action) {
            DOTween.To( //
                    () => 1f,
                    ratio => this.RawImage.material.SetFloat(DISSOLVE_AMOUNT, ratio),
                    0f,
                    duration
                )
                .OnComplete(() => {
                        action();
                        (this.RawImage.texture as RenderTexture)?.Release();
                        Destroy(this.RawImage.texture);
                        Destroy(this.gameObject);
                    }
                );
        }

        public void Hide(float duration, Action action) {
            DOTween.To( //
                    () => 0f,
                    ratio => this.RawImage.material.SetFloat(DISSOLVE_AMOUNT, ratio),
                    1f,
                    duration
                )
                .OnComplete(() => {
                        action();
                        (this.RawImage.texture as RenderTexture)?.Release();
                        Destroy(this.RawImage.texture);
                        Destroy(this.gameObject);
                    }
                );
        }
    }
}
