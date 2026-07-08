using Code.UI.Text;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.UI.Damage {
    public class MB_Damage : MB_Text {
        #region Members
        [Foldout("MB_Damage", true)]
        [SerializeField] private protected Color m_NonCriticalColor;
        [SerializeField] private protected Color m_CriticalColor;
        [SerializeField] private protected Color m_DodgeColor;
        [SerializeField] private protected Color m_HealColor;
        #endregion

        #region Getters / Setters
        private Color NonCriticalColor { get => this.m_NonCriticalColor; }
        private Color CriticalColor { get => this.m_CriticalColor; }
        private Color DodgeColor { get => this.m_DodgeColor; }
        private Color HealColor { get => this.m_HealColor; }
        #endregion

        #region Unity methods
        private void Start() {
            this.InSeconds(1, () => Destroy(this.gameObject));
        }
        #endregion

        public void SetDamage(float value, bool critical) {
            string text = SC_Utils.FormatNumber(value, decimals: 0, prefix: true);
            if (critical) text += "!";
            text = critical
                ? text.Color(this.CriticalColor)
                : text.Color(this.NonCriticalColor);
            text = text.VOffset(height: 4, delay: 0f, duration: .25f, offset: 0.03f, loop: false, progressive: true);
            this.SetText(text);
        }

        public void SetHeal(float value) {
            string text = SC_Utils.FormatNumber(value, decimals: 0, prefix: true);
            text = text.Color(this.HealColor);
            text = text.VOffset(height: 4, delay: 0f, duration: .25f, offset: 0.03f, loop: false, progressive: true);
            this.Text.color = this.HealColor;
            this.SetText(text);
        }

        public void Dodge() {
            string text = "Dodged";
            text = text.Color(this.DodgeColor);
            text = text.VOffset(height: 4, delay: 0f, duration: .25f, offset: 0.03f, loop: false, progressive: true);
            this.SetText(text);
        }
    }
}
