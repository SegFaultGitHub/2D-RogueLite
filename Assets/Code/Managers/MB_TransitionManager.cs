using System;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Managers {
    public class MB_TransitionManager : MonoBehaviour {
        private protected enum E_ScreenState {
            Hidden,
            Shown
        }

        #region Members
        [Foldout("MB_Transition", true)]
        [SerializeField] private protected Image m_Transition;
        [ReadOnly][SerializeField] private protected Material m_TransitionMaterial;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected E_ScreenState m_ScreenState = E_ScreenState.Hidden;
        [ReadOnly][SerializeField] private protected bool m_Transitioning;
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private Image Transition { get => this.m_Transition; }
        private Material TransitionMaterial { get => this.m_TransitionMaterial; set => this.m_TransitionMaterial = value; }

        private E_ScreenState ScreenState { get => this.m_ScreenState; set => this.m_ScreenState = value; }
        public bool Transitioning { get => this.m_Transitioning; private set => this.m_Transitioning = value; }
        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        #endregion

        #region Static / Readonly / Const
        private static readonly int SIZE = Shader.PropertyToID("_Size");
        private static readonly int POSITION = Shader.PropertyToID("_Position");
        private static readonly int ORTHOGRAPHIC_SIZE = Shader.PropertyToID("_OrthographicSize");

        public const Ease HIDE_EASE = Ease.InSine;
        public const Ease SHOW_EASE = Ease.OutSine;
        #endregion

        #region Unity methods
        private void OnApplicationQuit() {
            this.TransitionMaterial.SetFloat(SIZE, 0f);
            this.TransitionMaterial.SetVector(POSITION, new Vector4(.5f, .5f));
        }

        private void Awake() {
            this.TransitionMaterial = this.Transition.material;
        }

        private void Update() {
            this.TransitionMaterial.SetVector(
                POSITION,
                this.ObjectsManager.MainCamera.GetShaderPosition(this.ObjectsManager.Player.Center.position.ToVector2(), false)
            );
            this.TransitionMaterial.SetFloat(ORTHOGRAPHIC_SIZE, this.ObjectsManager.MainCamera.Camera.orthographicSize);
        }
        #endregion

        public void Initialize() { }

        public void PostInitialize() { }

        /***
         * Timeline:
         *  - start hide transition
         *  - action1
         *  - end hide transition
         *  - action2
         *  - wait
         *  - start show transition
         *  - action3
         *  - end show transition
         *  - action4
         */
        public void Run(
            float waitTime,
            float duration,
            bool realTime,
            Action action1 = null,
            Action action2 = null,
            Action action3 = null,
            Action action4 = null
        ) {
            this.Until(
                () => !this.Transitioning,
                () => {
                    this.Transitioning = true;
                    DOTween.Sequence()
                        .SetUpdate(realTime)
                        .AppendCallback(() => action1?.Invoke())
                        .Append(this.HideScreen(duration))
                        .AppendCallback(() => action2?.Invoke())
                        .AppendInterval(waitTime)
                        .AppendCallback(() => action3?.Invoke())
                        .Append(this.ShowScreen(duration))
                        .AppendCallback(() => action4?.Invoke())
                        .OnComplete(() => this.Transitioning = false);
                }
            );
        }

        private Tween HideScreen(float duration) =>
            DOTween.To(
                    () => this.TransitionMaterial.GetFloat(SIZE),
                    size => this.TransitionMaterial.SetFloat(SIZE, size),
                    0f,
                    this.ScreenState == E_ScreenState.Hidden
                        ? 0
                        : duration
                )
                .SetEase(HIDE_EASE)
                .OnComplete(() => this.ScreenState = E_ScreenState.Hidden);

        private Tween ShowScreen(float duration) =>
            DOTween.To(() => this.TransitionMaterial.GetFloat(SIZE), size => this.TransitionMaterial.SetFloat(SIZE, size), 1f, duration)
                .SetEase(SHOW_EASE)
                .OnComplete(() => this.ScreenState = E_ScreenState.Shown);

        // private IEnumerator HideScreen(bool realTime) {
        //     yield return new WaitUntil(() => {
        //             AnimatorStateInfo animatorStateInfo = this.Animator.GetCurrentAnimatorStateInfo(0);
        //             return animatorStateInfo.shortNameHash == SHOW_SCREEN || animatorStateInfo.shortNameHash == NONE;
        //         }
        //     );
        //
        //     this.Animator.SetTrigger(HIDE_SCREEN);
        //     float clipLength = this.Animator.GetCurrentAnimatorClipInfo(0).Length;
        //     float speed = this.Animator.GetCurrentAnimatorStateInfo(0).speed;
        //     if (realTime) yield return new WaitForSecondsRealtime(clipLength / speed);
        //     else yield return new WaitForSeconds(clipLength / speed);
        // }
        //
        // private IEnumerator ShowScreen(bool realTime) {
        //     yield return new WaitUntil(() => {
        //             AnimatorStateInfo animatorStateInfo = this.Animator.GetCurrentAnimatorStateInfo(0);
        //             return animatorStateInfo.shortNameHash == HIDE_SCREEN || animatorStateInfo.shortNameHash == NONE;
        //         }
        //     );
        //
        //     this.Animator.SetTrigger(SHOW_SCREEN);
        //     float clipLength = this.Animator.GetCurrentAnimatorClipInfo(0).Length;
        //     float speed = this.Animator.GetCurrentAnimatorStateInfo(0).speed;
        //     if (realTime) yield return new WaitForSecondsRealtime(clipLength / speed);
        //     else yield return new WaitForSeconds(clipLength / speed);
        // }
        //
        // [ButtonMethod]
        // public void Run() {
        //     this.StartCoroutine(this.Run(direction: E_Direction.North, waitTime: 0, realTime: true));
        // }
    }
}
