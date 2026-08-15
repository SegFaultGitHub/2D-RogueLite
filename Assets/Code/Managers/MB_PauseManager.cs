using System;
using System.Collections;
using Code.UI;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Code.Managers {
    public class MB_PauseManager : MonoBehaviour {
        public enum E_PauseState {
            NotPaused,
            Paused,
            EnhancementChoices
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
        private GameObject PausedOverlay { get => this.m_PausedOverlay; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        public E_PauseState PauseState { get => this.m_PauseState; private set => this.m_PauseState = value; }
        private Texture2D PausedFrame { get => this.m_PausedFrame; set => this.m_PausedFrame = value; }

        private Tweener QuickPauseTweener { get; set; }
        private Coroutine QuickPauseCoroutine { get; set; }
        private string QuickPauseGuid { get; set; }
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

            if (this.QuickPauseCoroutine != null) this.StopCoroutine(this.QuickPauseCoroutine);
            if (this.QuickPauseTweener is { active: true }) DOTween.Kill(this.QuickPauseTweener);

            switch (pauseState) {
                case E_PauseState.Paused:
                    this.Cursor.gameObject.SetActive(false);
                    this.OnEndOfFrame(() => {
                            this.ObjectsManager.ScreenshotManager.Screenshot();
                            this.ObjectsManager.ScreenshotManager.ScreenshotUIComponents();
                            //this.PausedFrame = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);
                            this.Cursor.gameObject.SetActive(true);
                            this.ToggleComponents(pausedOverlay: true);
                        }
                    );
                    break;
                case E_PauseState.EnhancementChoices:
                    this.QuickPauseTweener = DOTween.To( //
                            () => Time.timeScale,
                            timeScale => Time.timeScale = timeScale,
                            0,
                            this.ObjectsManager.DissolveManager.DissolveDuration
                        )
                        .SetEase(Ease.OutExpo)
                        .SetUpdate(true)
                        .OnComplete(() => this.QuickPauseTweener = null);
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
            E_PauseState previousPauseState = this.PauseState;
            if (this.PauseState is E_PauseState.NotPaused) return;

            this.ToggleComponents(pausedOverlay: false);

            this.PauseState = E_PauseState.NotPaused;
            this.ObjectsManager.AudioManager.SetBackgroundMusicVolume(1);
            this.ObjectsManager.AudioManager.SetSoundEffectsVolume(1);
            // this.ObjectsManager.BlurCanvas.gameObject.SetActive(false);

            switch (previousPauseState) {
                case E_PauseState.Paused:
                    Time.timeScale = 1; break;
                case E_PauseState.EnhancementChoices:
                    this.QuickPauseTweener = DOTween.To( //
                            () => Time.timeScale,
                            timeScale => Time.timeScale = timeScale,
                            1,
                            this.ObjectsManager.DissolveManager.DissolveDuration
                        )
                        .SetEase(Ease.OutExpo)
                        .SetUpdate(true)
                        .OnComplete(() => this.QuickPauseTweener = null);
                    break;
                case E_PauseState.NotPaused:
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void QuickPause(float duration) {
            IEnumerator _Coroutine() {
                switch (this.PauseState) {
                    case E_PauseState.NotPaused:
                        this.QuickPauseTweener = DOTween.To( //
                                () => Time.timeScale,
                                timeScale => Time.timeScale = timeScale,
                                .25f,
                                duration
                            )
                            .SetEase(Ease.OutExpo)
                            .SetUpdate(true)
                            .OnComplete(() => this.QuickPauseTweener = null);
                        break;
                    case E_PauseState.Paused:
                    case E_PauseState.EnhancementChoices:
                    default:
                        yield break;
                }

                yield return new WaitForSecondsRealtime(duration);

                switch (this.PauseState) {
                    case E_PauseState.NotPaused:
                        Time.timeScale = 1;
                        break;
                    case E_PauseState.Paused:
                    case E_PauseState.EnhancementChoices:
                    default:
                        yield break;
                }
            }

            if (this.QuickPauseCoroutine != null) this.StopCoroutine(this.QuickPauseCoroutine);
            if (this.QuickPauseTweener is { active: true }) DOTween.Kill(this.QuickPauseTweener);

            this.QuickPauseTweener = null;
            this.QuickPauseCoroutine = this.StartCoroutine(_Coroutine());
        }

        private void ToggleComponents(bool pausedOverlay) {
            if (this.PausedOverlay.activeInHierarchy != pausedOverlay) {
                //this.PausedImage.texture = this.PausedFrame;
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
                // Can't exist paused state manually from these
                case E_PauseState.EnhancementChoices:
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
