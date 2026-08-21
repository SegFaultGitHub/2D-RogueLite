using System.Text;
using Code.UI.Text;
using MyBox;
using UnityEngine;

namespace Code.UI.EnhancementList {
    public class MB_UnlockCondition : MonoBehaviour {
        #region Members
        [Foldout("MB_UnlockCondition", true)]
        [SerializeField] private protected MB_Text m_Text;
        #endregion

        #region Getters / Setters
        public MB_Text Text { get => this.m_Text; }
        #endregion

        #region Static / Readonly / Const
        private const string SPRITE_UNLOCKED = "<sprite index=0>";
        private const string SPRITE_LOCKED = "<sprite index=1>";
        #endregion

        #region Unity methods
        #endregion

        public void Set(C_UnlockCondition unlockCondition) {
            if (unlockCondition.Secret) {
                this.Text.SetText("???".LineHeight(8));
            } else {
                StringBuilder text = new();
                text.Append(
                    unlockCondition.Unlocked
                        ? SPRITE_UNLOCKED
                        : SPRITE_LOCKED
                );
                text.Append(" ");
                text.Append(unlockCondition.Text);
                this.Text.SetText(text.ToString().LineHeight(8));
            }
        }
    }
}
