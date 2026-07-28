using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Misc {
    public class MB_ProgressBar : MonoBehaviour {
        #region Members
        [Foldout("MB_ProgressBar", true)]
        [SerializeField] private protected Image m_Bar;
        [SerializeField] private protected Image m_DelayedBackground;
        [SerializeField] private protected float m_Delay;
        [SerializeField] private protected float m_DelayDuration;
        [SerializeField] private protected bool m_PixelPerfect;
        [SerializeField] private protected float m_Lerp;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Animator m_Animator;
        [ReadOnly][SerializeField] private protected float m_Ratio;
        [ReadOnly][SerializeField] private protected float m_CurrentRatio;
        #endregion

        #region Getters / Setters
        private Image Bar { get => this.m_Bar; }
        private Image DelayedBackground { get => this.m_DelayedBackground; }
        private float Delay { get => this.m_Delay; }
        private float DelayDuration { get => this.m_DelayDuration; }
        private bool PixelPerfect { get => this.m_PixelPerfect; }
        private float Lerp { get => this.m_Lerp; }

        private Animator Animator { get => this.m_Animator; set => this.m_Animator = value; }
        private float Ratio { get => this.m_Ratio; set => this.m_Ratio = value; }
        private float CurrentRatio { get => this.m_CurrentRatio; set => this.m_CurrentRatio = value; }

        private float Width { get => this.Bar.rectTransform.rect.width; }

        private Coroutine DelayedBackgroundCoroutine { get; set; }
        private Tweener DelayedBackgroundTweener { get; set; }
        #endregion

        #region Static / Readonly / Const
        private static readonly int RANDOM = Animator.StringToHash("Random");
        private static readonly int SHAKE = Animator.StringToHash("Shake");
        #endregion

        #region Unity methods
        private void Awake() {
            this.Animator = this.GetComponent<Animator>();
            this.Ratio = 1;
        }

        private void FixedUpdate() {
            this.CurrentRatio = Mathf.Lerp(this.CurrentRatio, this.Ratio, this.Lerp);
            this.Bar.fillAmount = this.PixelPerfect
                ? Mathf.Round(this.CurrentRatio * this.Width) / this.Width
                : this.CurrentRatio;
        }
        #endregion

        public void ForceSetRatio(float ratio) {
            this.Ratio = ratio;
            this.CurrentRatio = ratio;
            this.Bar.fillAmount = ratio;
            this.DelayedBackground.fillAmount = ratio;

            if (this.DelayedBackgroundCoroutine != null) this.StopCoroutine(this.DelayedBackgroundCoroutine);
            if (this.DelayedBackgroundTweener is { active: true }) DOTween.Kill(this.DelayedBackgroundTweener);
            this.DelayedBackgroundCoroutine = null;
            this.DelayedBackgroundTweener = null;
        }

        public float TEMP_Ratio;

        [ButtonMethod]
        public void SetRatio() => this.SetRatio(this.TEMP_Ratio);

        public void SetRatio(float ratio) {
            this.Ratio = ratio;

            if (this.DelayedBackgroundCoroutine != null) this.StopCoroutine(this.DelayedBackgroundCoroutine);
            if (this.DelayedBackgroundTweener is { active: true }) DOTween.Kill(this.DelayedBackgroundTweener);

            this.DelayedBackgroundTweener = null;
            this.DelayedBackgroundCoroutine = this.InSeconds(
                this.Delay,
                () => {
                    this.DelayedBackgroundTweener = DOTween.To( //
                            () => this.DelayedBackground.fillAmount,
                            fillAmount => {
                                this.DelayedBackground.fillAmount = this.PixelPerfect
                                    ? Mathf.Round(fillAmount * this.Width) / this.Width
                                    : fillAmount;
                            },
                            ratio,
                            this.DelayDuration
                        )
                        .SetEase(Ease.OutExpo)
                        .OnComplete(() => this.DelayedBackgroundTweener = null);
                }
            );
        }

        public void Shake() {
            this.Animator.SetBool(RANDOM, SC_Utils.Rate(.5f));
            this.Animator.SetTrigger(SHAKE);
        }
    }
}
