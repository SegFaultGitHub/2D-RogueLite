using Code.UI.Misc;
using Code.Utils;
using MyBox;
using TMPro;
using UnityEngine;

namespace Code.UI.HUD {
    public class MB_HealthBar : MB_ProgressBar {
        #region Members
        [Foldout("MB_HealthBar", true)]
        [SerializeField] private protected TMP_Text m_Text;
        #endregion

        #region Getters / Setters
        private TMP_Text Text { get => this.m_Text; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void SetHealth(float currentHealth, float maxHealth) {
            this.Text.SetText($"{SC_Utils.FormatNumber(currentHealth)} / {SC_Utils.FormatNumber(maxHealth)}");
            this.SetRatio(currentHealth / maxHealth);
        }
    }
}
