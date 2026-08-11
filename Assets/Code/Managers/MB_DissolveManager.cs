using System;
using System.Collections.Generic;
using Code.UI;
using MyBox;
using UnityEngine;

namespace Code.Managers {
    public class MB_DissolveManager : MonoBehaviour {
        #region Members
        [Foldout("MB_DissolveManager", true)]
        [SerializeField] private protected MB_DissolveElement m_DissolveElementPrefab;
        [SerializeField] private protected Canvas m_Canvas;
        [SerializeField] private protected float m_DissolveDuration;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private MB_DissolveElement DissolveElement { get => this.m_DissolveElementPrefab; }
        private Canvas Canvas { get => this.m_Canvas; }
        private float DissolveDuration { get => this.m_DissolveDuration; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void Show(Transform uiComponent, Action action) =>
            this.Show(new List<Transform> { uiComponent }, action);

        public void Show(List<Transform> uiComponents, Action action) {
            RenderTexture screenshot = this.ObjectsManager.ScreenshotManager.ScreenshotUIComponents(uiComponents);
            MB_DissolveElement dissolveElement = Instantiate(this.DissolveElement, this.Canvas.transform);
            dissolveElement.Canvas = this.Canvas;
            dissolveElement.SetTexture(screenshot);
            dissolveElement.Show(this.DissolveDuration, action);
        }

        public void Hide(Transform uiComponent, Action action) =>
            this.Hide(new List<Transform> { uiComponent }, action);

        public void Hide(List<Transform> uiComponents, Action action) {
            RenderTexture screenshot = this.ObjectsManager.ScreenshotManager.ScreenshotUIComponents(uiComponents);
            MB_DissolveElement dissolveElement = Instantiate(this.DissolveElement, this.Canvas.transform);
            dissolveElement.Canvas = this.Canvas;
            dissolveElement.SetTexture(screenshot);
            dissolveElement.Hide(this.DissolveDuration, action);
        }
    }
}
