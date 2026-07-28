using Code.UI.Misc;
using Code.Utils;
using MyBox;
using TMPro;
using UnityEngine;

namespace Code.UI.HUD {
    public class MB_HealthBar : MB_ProgressBar {
        #region Members
        [Foldout("MB_HealthBar", true)]
        [SerializeField] private protected TMP_Text m_CurrentHealth;
        [SerializeField] private protected TMP_Text m_MaxHealth;
        #endregion

        #region Getters / Setters
        private TMP_Text CurrentHealth { get => this.m_CurrentHealth; }
        private TMP_Text MaxHealth { get => this.m_MaxHealth; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void SetHealth(float currentHealth, float maxHealth) {
            this.CurrentHealth.SetText(SC_Utils.FormatNumber(currentHealth));
            this.MaxHealth.SetText(SC_Utils.FormatNumber(maxHealth));
            this.SetRatio(currentHealth / maxHealth);
        }
    }
}
