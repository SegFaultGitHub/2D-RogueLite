using System;
using System.Collections.Generic;
using Code.Managers;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class MB_DissolveUI : MonoBehaviour {
        #region Members
        [Foldout("MB_DissolveUI", true)]
        [SerializeField] private protected RawImage m_RawImage;
        [SerializeField] private protected Canvas m_Canvas;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private RawImage RawImage { get => this.m_RawImage; }
        private Canvas Canvas { get => this.m_Canvas; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }

        private Tweener Tweener { get; set; }
        private Material DissolveMaterial { get; set; }
        #endregion

        #region Static / Readonly / Const
        private static readonly int DISSOLVE_AMOUNT = Shader.PropertyToID("_DissolveAmount");
        private static readonly int WIDTH = Shader.PropertyToID("_Width");
        private static readonly int HEIGHT = Shader.PropertyToID("_Height");
        private const float SHOW_HIDE_DURATION = 1f;
        #endregion

        #region Unity methods
        private void Awake() {
            this.DissolveMaterial = this.RawImage.material;
        }

        private void Update() {
            Rect rect = this.Canvas.GetComponent<RectTransform>().rect;
            this.DissolveMaterial.SetFloat(WIDTH, rect.width);
            this.DissolveMaterial.SetFloat(HEIGHT, rect.height);
        }
        #endregion

        public void Show(List<Transform> uiComponents, Action action) {
            this.ObjectsManager.ScreenshotManager.ScreenshotUIComponents(uiComponents);

            if (this.Tweener is { active: true }) DOTween.Kill(this.Tweener);
            this.Tweener = DOTween.To( //
                    () => 1f,
                    ratio => this.DissolveMaterial.SetFloat(DISSOLVE_AMOUNT, ratio),
                    0f,
                    SHOW_HIDE_DURATION
                )
                .OnComplete(() => {
                        action();
                        this.DissolveMaterial.SetFloat(DISSOLVE_AMOUNT, 1);
                    }
                );
        }

        public void Hide(ICollection<Transform> uiComponents, Action action) {
            this.ObjectsManager.ScreenshotManager.ScreenshotUIComponents(uiComponents);

            if (this.Tweener is { active: true }) DOTween.Kill(this.Tweener);
            this.Tweener = DOTween.To( //
                    () => 0f,
                    ratio => this.DissolveMaterial.SetFloat(DISSOLVE_AMOUNT, ratio),
                    1f,
                    SHOW_HIDE_DURATION
                )
                .OnComplete(() => {
                        action();
                        this.DissolveMaterial.SetFloat(DISSOLVE_AMOUNT, 1);
                    }
                );
            ;
        }
    }
}
