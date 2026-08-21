using System;
using UnityEngine;

namespace Code.UI.EnhancementList {
    [Serializable]
    public class C_UnlockCondition {
        #region Members
        [SerializeField] private protected string m_Text;
        [SerializeField] private protected bool m_Unlocked;
        [SerializeField] private protected int m_Indent;
        [SerializeField] private protected bool m_Secret = false;
        #endregion

        #region Getters / Setters
        public string Text { get => this.m_Text; set => this.m_Text = value; }
        public bool Unlocked { get => this.m_Unlocked; set => this.m_Unlocked = value; }
        public int Indent { get => this.m_Indent; set => this.m_Indent = value; }
        public bool Secret { get => this.m_Secret; set => this.m_Secret = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion
    }
}
