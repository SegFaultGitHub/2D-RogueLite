using System;
using Code.Utils;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Misc {
    public class MB_ProgressBar : MonoBehaviour {
        #region Members
        [Foldout("MB_ProgressBar", true)]
        [SerializeField] private protected Image m_Bar;
        [SerializeField] private protected bool m_PixelPerfect;
        [ConditionalField(nameof(m_PixelPerfect), false, false)]
        [SerializeField] private protected float m_Lerp;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Animator m_Animator;
        [ReadOnly][SerializeField] private protected float m_Ratio;
        #endregion

        #region Getters / Setters
        private Image Bar { get => this.m_Bar; }
        private bool PixelPerfect { get => this.m_PixelPerfect; }
        private float Lerp { get => this.m_Lerp; }

        private Animator Animator { get => this.m_Animator; set => this.m_Animator = value; }
        private float Ratio { get => this.m_Ratio; set => this.m_Ratio = value; }

        private float Width { get => this.Bar.rectTransform.rect.width; }
        #endregion

        #region Static / Readonly / Const
        private static readonly int RANDOM = Animator.StringToHash("Random");
        private static readonly int SHAKE = Animator.StringToHash("Shake");
        #endregion

        #region Unity methods
        private void Awake() {
            this.Animator = this.GetComponent<Animator>();
        }

        private void FixedUpdate() {
            if (!this.PixelPerfect) {
                this.Bar.fillAmount = Mathf.Lerp(this.Bar.fillAmount, this.Ratio, this.Lerp);
            }
        }
        #endregion

        public void ForceSetRatio(float ratio) {
            this.m_Ratio = ratio;
            this.Bar.fillAmount = this.Ratio;
        }

        public void SetRatio(float ratio) {
            this.Ratio = ratio;
            if (this.PixelPerfect) {
                this.Bar.fillAmount = Mathf.Round(this.Ratio * this.Width) / this.Width;
            }
        }

        public void Shake() {
            this.Animator.SetBool(RANDOM, SC_Utils.Rate(.5f));
            this.Animator.SetTrigger(SHAKE);
        }
    }
}
