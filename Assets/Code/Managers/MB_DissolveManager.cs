using System;
using System.Collections.Generic;
using Code.UI;
using MyBox;
using UnityEngine;

namespace Code.Managers {
    public class MB_DissolveManager : MonoBehaviour {
        public enum E_Position {
            BeforeBlur, AfterBlur
        }

        #region Members
        [Foldout("MB_DissolveManager", true)]
        [SerializeField] private protected MB_DissolveElement m_DissolveElementPrefab;
        [SerializeField] private protected Canvas m_CanvasBeforeBlur;
        [SerializeField] private protected Canvas m_CanvasAfterBlur;
        [SerializeField] private protected float m_DissolveDuration;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private MB_DissolveElement DissolveElement { get => this.m_DissolveElementPrefab; }
        private Canvas CanvasBeforeBlur { get => this.m_CanvasBeforeBlur; }
        private Canvas CanvasAfterBlur { get => this.m_CanvasAfterBlur; }
        private float DissolveDuration { get => this.m_DissolveDuration; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void Show(Transform uiComponent, bool realTime, E_Position position, Action action) =>
            this.Show(new List<Transform> { uiComponent }, realTime, position, action);

        public void Show(List<Transform> uiComponents, bool realTime, E_Position position, Action action) {
            RenderTexture screenshot = this.ObjectsManager.ScreenshotManager.ScreenshotUIComponents(uiComponents);
            MB_DissolveElement dissolveElement;
            switch (position) {
                case E_Position.BeforeBlur:
                    dissolveElement = Instantiate(this.DissolveElement, this.CanvasBeforeBlur.transform);
                    dissolveElement.Canvas = this.CanvasBeforeBlur;
                    break;
                case E_Position.AfterBlur:
                    dissolveElement = Instantiate(this.DissolveElement, this.CanvasAfterBlur.transform);
                    dissolveElement.Canvas = this.CanvasAfterBlur;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }

            dissolveElement.SetTexture(screenshot);
            dissolveElement.Show(this.DissolveDuration, realTime, action);
        }

        public void Hide(Transform uiComponent, bool realTime, E_Position position, Action action) =>
            this.Hide(new List<Transform> { uiComponent }, realTime, position, action);

        public void Hide(List<Transform> uiComponents, bool realTime, E_Position position, Action action) {
            RenderTexture screenshot = this.ObjectsManager.ScreenshotManager.ScreenshotUIComponents(uiComponents);
            MB_DissolveElement dissolveElement;
            switch (position) {
                case E_Position.BeforeBlur:
                    dissolveElement = Instantiate(this.DissolveElement, this.CanvasBeforeBlur.transform);
                    dissolveElement.Canvas = this.CanvasBeforeBlur;
                    break;
                case E_Position.AfterBlur:
                    dissolveElement = Instantiate(this.DissolveElement, this.CanvasAfterBlur.transform);
                    dissolveElement.Canvas = this.CanvasAfterBlur;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
            dissolveElement.SetTexture(screenshot);
            dissolveElement.Hide(this.DissolveDuration, realTime, action);
        }
    }
}
