using Code.UI.Misc;
using MyBox;
using TMPro;
using UnityEngine;

namespace Code.UI.HUD {
    public class MB_BossLifeBar : MB_ProgressBar {
        #region Members
        [Foldout("MB_BossLifeBar", true)]
        [SerializeField] private protected TMP_Text m_BossNameText;
        #endregion

        #region Getters / Setters
        private TMP_Text BossNameText { get => this.m_BossNameText; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void SetBossName(string bossName) => this.BossNameText.text = bossName;
    }
}
