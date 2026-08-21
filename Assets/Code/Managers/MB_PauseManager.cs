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
            EnhancementChoices,
            EnhancementList
        }

        #region Members
        [Foldout("MB_PauseManager", true)]
        [SerializeField] private protected MB_Cursor m_Cursor;
        [SerializeField] private protected RawImage m_Screenshot;
        [SerializeField] private protected GameObject[] m_PausedOverlays;
        [SerializeField] private protected GameObject m_EnhancementList;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        [ReadOnly][SerializeField] private protected E_PauseState m_PauseState;
        #endregion

        #region Getters / Setters
        private MB_Cursor Cursor { get => this.m_Cursor; }
        private RawImage Screenshot { get => this.m_Screenshot; }
        private GameObject[] PausedOverlays { get => this.m_PausedOverlays; }
        private GameObject EnhancementList { get => this.m_EnhancementList; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        public E_PauseState PauseState { get => this.m_PauseState; private set => this.m_PauseState = value; }

        private Tweener QuickPauseTweener { get; set; }
        private Coroutine QuickPauseCoroutine { get; set; }
        private Tweener BlurTweener { get; set; }
        private string QuickPauseGuid { get; set; }
        #endregion

        #region Static / Readonly / Const
        private static readonly int BLUR_AMOUNT = Shader.PropertyToID("_BlurAmount");
        private const float PAUSE_ENTER_EXIT_DURATION = .25f;
        #endregion

        #region Unity methods
        private void Awake() {
            this.Screenshot.material = new  Material(this.Screenshot.material);
        }
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

            this.Cursor.gameObject.SetActive(false);
            this.OnEndOfFrame(() => {
                    this.ObjectsManager.ScreenshotManager.Screenshot();
                    this.Cursor.gameObject.SetActive(true);

                    switch (pauseState) {
                        case E_PauseState.Paused:
                            this.ToggleComponents(pausedOverlay: true, enhancementList: false);
                            break;
                        case E_PauseState.EnhancementChoices:
                            this.ToggleComponents(pausedOverlay: false, enhancementList: false);
                            break;
                        case E_PauseState.EnhancementList:
                            this.ToggleComponents(pausedOverlay: false, enhancementList: true);
                            break;
                        case E_PauseState.NotPaused:
                        default: throw new ArgumentOutOfRangeException(nameof(pauseState), pauseState, null);
                    }

                    Time.timeScale = 0;
                    this.PauseState = pauseState;
                    this.ObjectsManager.AudioManager.SetBackgroundMusicVolume(.5f);
                    this.ObjectsManager.AudioManager.SetSoundEffectsVolume(.5f);

                    if (this.BlurTweener is { active: true }) DOTween.Kill(this.BlurTweener);
                    this.BlurTweener = DOTween.To( //
                            () => this.Screenshot.material.GetFloat(BLUR_AMOUNT),
                            amount => this.Screenshot.material.SetFloat(BLUR_AMOUNT, amount),
                            1f,
                            PAUSE_ENTER_EXIT_DURATION
                        )
                        .SetEase(Ease.OutExpo)
                        .SetUpdate(true)
                        .OnComplete(() => this.BlurTweener = null);
                }
            );
        }

        public void Unpause() {
            E_PauseState previousPauseState = this.PauseState;
            if (this.PauseState is E_PauseState.NotPaused) return;

            this.ObjectsManager.AudioManager.SetBackgroundMusicVolume(1);
            this.ObjectsManager.AudioManager.SetSoundEffectsVolume(1);

            this.ToggleComponents(pausedOverlay: false, enhancementList: false);

            switch (previousPauseState) {
                case E_PauseState.Paused:
                case E_PauseState.EnhancementChoices:
                case E_PauseState.EnhancementList:
                    if (this.BlurTweener is { active: true }) DOTween.Kill(this.BlurTweener);
                    this.BlurTweener = DOTween.To( //
                            () => this.Screenshot.material.GetFloat(BLUR_AMOUNT),
                            amount => this.Screenshot.material.SetFloat(BLUR_AMOUNT, amount),
                            0f,
                            PAUSE_ENTER_EXIT_DURATION
                        )
                        .SetEase(Ease.OutExpo)
                        .SetUpdate(true)
                        .OnComplete(() => {
                                this.BlurTweener = null;
                                Time.timeScale = 1;
                                this.PauseState = E_PauseState.NotPaused;
                            }
                        );
                    break;
                case E_PauseState.NotPaused:
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void QuickPause(float duration) {
            IEnumerator _Coroutine() {
                switch (this.PauseState) {
                    case E_PauseState.NotPaused:
                        string guid = Guid.NewGuid().ToString();
                        this.QuickPauseGuid = guid;
                        const float t = 1;

                        this.QuickPauseTweener = DOTween.To( //
                                () => this.QuickPauseGuid != guid
                                    ? t // Prevents overlapping animation conflicts
                                    : Time.timeScale,
                                timeScale => {
                                    // Prevents overlapping animation conflicts
                                    if (this.QuickPauseGuid != guid) return;
                                    Time.timeScale = timeScale;
                                },
                                .25f,
                                duration
                            )
                            .SetEase(Ease.OutExpo)
                            .SetUpdate(true)
                            .OnComplete(() => this.QuickPauseTweener = null);
                        break;
                    case E_PauseState.Paused:
                    case E_PauseState.EnhancementChoices:
                    case E_PauseState.EnhancementList:
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
                    case E_PauseState.EnhancementList:
                    default:
                        yield break;
                }
            }

            if (this.QuickPauseCoroutine != null) this.StopCoroutine(this.QuickPauseCoroutine);
            if (this.QuickPauseTweener is { active: true }) DOTween.Kill(this.QuickPauseTweener);

            this.QuickPauseTweener = null;
            this.QuickPauseCoroutine = this.StartCoroutine(_Coroutine());
        }

        private void ToggleComponents(bool pausedOverlay, bool enhancementList) {
            foreach (GameObject overlay in this.PausedOverlays) {
                if (overlay.activeInHierarchy != pausedOverlay) {
                    overlay.SetActive(pausedOverlay);
                }
            }

            this.EnhancementList.SetActive(enhancementList);
        }

        private void TogglePause() {
            switch (this.PauseState) {
                case E_PauseState.NotPaused:
                    this.Pause(E_PauseState.Paused);
                    break;
                case E_PauseState.Paused:
                case E_PauseState.EnhancementList:
                    this.Unpause();
                    break;
                // Can't exist paused state manually from these
                case E_PauseState.EnhancementChoices:
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        [ButtonMethod]
        public void Foo() => this.Pause(E_PauseState.EnhancementList);

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
