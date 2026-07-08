using Code.Enhancements;
using Code.UI.Text;
using MyBox;
using UnityEngine;

namespace Code.UI.HUD {
    public class MB_EnhancementDescription : MonoBehaviour {
        #region Members
        [Foldout("MB_EnhancementDescription", true)]
        [SerializeField] private protected MB_Text m_NameText;
        [SerializeField] private protected MB_Text m_LevelText;
        [SerializeField] private protected MB_Text m_DescriptionText;

        [SerializeField] private protected RectTransform m_PaperRect;
        #endregion

        #region Getters / Setters
        private MB_Text NameText { get => this.m_NameText; }
        private MB_Text LevelText { get => this.m_LevelText; }
        private MB_Text DescriptionText { get => this.m_DescriptionText; }

        private RectTransform PaperRect { get => this.m_PaperRect; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void SetEnhancement(AMB_Enhancement enhancement) {
            string nameString = enhancement.EnhancementName.VOffset(
                height: 2,
                delay: 0,
                offset: 0.0625f,
                duration: 0.25f,
                loop: true,
                loopDelay: 5,
                progressive: true
            );
            string levelString = $"{enhancement.EffectiveLevel} / {enhancement.MaxLevel}";

            this.NameText.SetText(nameString);
            this.LevelText.SetText(levelString);
            this.DescriptionText.SetText(enhancement.GetDescription());

            this.PaperRect.SetHeight((this.DescriptionText.GetLineCount() + 4) * 8);
        }
    }
}
