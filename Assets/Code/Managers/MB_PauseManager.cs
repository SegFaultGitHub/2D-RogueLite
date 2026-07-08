using System;
using System.Collections;
using Code.UI;
using Code.Utils;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Code.Managers {
    public class MB_PauseManager : MonoBehaviour {
        public enum E_PauseState {
            NotPaused,
            Paused
        }

        #region Members
        [Foldout("MB_PauseManager", true)]
        [SerializeField] private protected MB_Cursor m_Cursor;
        [SerializeField] private protected RawImage m_PausedImage;
        [SerializeField] private protected GameObject m_PausedOverlay;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        [ReadOnly][SerializeField] private protected E_PauseState m_PauseState;
        [ReadOnly][SerializeField] private protected Texture2D m_PausedFrame;
        #endregion

        #region Getters / Setters
        private MB_Cursor Cursor { get => this.m_Cursor; }
        private RawImage PausedImage { get => this.m_PausedImage; }
        public GameObject PausedOverlay { get => this.m_PausedOverlay; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        public E_PauseState PauseState { get => this.m_PauseState; private set => this.m_PauseState = value; }
        private Texture2D PausedFrame { get => this.m_PausedFrame; set => this.m_PausedFrame = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void Initialize() {
            Time.timeScale = 1;
            this.PauseState = E_PauseState.NotPaused;
        }

        public void PostInitialize() { }

        public void Pause(E_PauseState pauseState) {
            if (this.PauseState is not E_PauseState.NotPaused) return;

            switch (pauseState) {
                case E_PauseState.Paused:
                    this.Cursor.gameObject.SetActive(false);
                    this.OnEndOfFrame(() => {
                            this.PausedFrame = this.ObjectsManager.ScreenshotManager.Screenshot();
                            this.PausedFrame = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);
                            this.Cursor.gameObject.SetActive(true);
                            this.ToggleComponents(pausedOverlay: true);
                        }
                    );
                    break;
                case E_PauseState.NotPaused:
                default: throw new ArgumentOutOfRangeException(nameof(pauseState), pauseState, null);
            }

            this.PauseState = pauseState;
            this.ObjectsManager.AudioManager.SetBackgroundMusicVolume(.5f);
            this.ObjectsManager.AudioManager.SetSoundEffectsVolume(.5f);
            // this.ObjectsManager.BlurCanvas.gameObject.SetActive(true);
            Time.timeScale = 0;
        }

        public void Unpause() {
            if (this.PauseState is E_PauseState.NotPaused) return;

            this.ToggleComponents(pausedOverlay: false);

            this.PauseState = E_PauseState.NotPaused;
            this.ObjectsManager.AudioManager.SetBackgroundMusicVolume(1);
            this.ObjectsManager.AudioManager.SetSoundEffectsVolume(1);
            // this.ObjectsManager.BlurCanvas.gameObject.SetActive(false);
            Time.timeScale = 1;
        }

        public void QuickPause(float duration) {
            IEnumerator _Coroutine() {
                switch (this.PauseState) {
                    case E_PauseState.NotPaused:
                        Time.timeScale = 0;
                        break;
                    case E_PauseState.Paused:
                    default:
                        yield break;
                }

                yield return new WaitForSecondsRealtime(duration);

                switch (this.PauseState) {
                    case E_PauseState.NotPaused:
                        Time.timeScale = 1;
                        break;
                    case E_PauseState.Paused:
                    default:
                        yield break;
                }
            }

            this.StartCoroutine(_Coroutine());
        }

        private void ToggleComponents(bool pausedOverlay) {
            if (this.PausedOverlay.activeInHierarchy != pausedOverlay) {
                this.PausedImage.texture = this.PausedFrame;
                this.PausedOverlay.SetActive(pausedOverlay);
            }
        }

        private void TogglePause() {
            switch (this.PauseState) {
                case E_PauseState.NotPaused:
                    this.Pause(E_PauseState.Paused);
                    break;
                case E_PauseState.Paused:
                    this.Unpause();
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        #region Input
        private PlayerInputs PlayerInputs { get; set; }

        protected void OnEnable() {
            this.PlayerInputs = new PlayerInputs();
            this.PlayerInputs.Actions.Enable();

            this.PlayerInputs.Actions.Pause.performed += this.TogglePauseInput;
        }

        protected void OnDisable() {
            this.PlayerInputs.Actions.Pause.performed -= this.TogglePauseInput;

            this.PlayerInputs.Actions.Disable();
        }

        private void TogglePauseInput(InputAction.CallbackContext _) {
            this.TogglePause();
        }
        #endregion
    }
}
