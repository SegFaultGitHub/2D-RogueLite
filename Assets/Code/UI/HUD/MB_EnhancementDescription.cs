using System;
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
            string nameString = enhancement.EnhancementName.Name();
            string levelString = $"{enhancement.EffectiveLevel} / {enhancement.MaxLevel}";
            string description = enhancement.GetDescription().LineHeight(8);

            this.SetData(nameString, levelString, description);
        }

        protected void SetData(string nameString, string levelString, string descriptionString) {
            this.NameText.SetText(nameString);
            this.LevelText.SetText(levelString);
            this.DescriptionText.SetText(descriptionString);

            this.PaperRect.SetHeight((this.DescriptionText.GetLineCount() + 4) * 8);
        }
    }

    public static class SC_MB_EnhancementDescriptionExtensions {
        public static string AsList(this string[] s, int level, Func<string, string> colorCurrent) {
            string result = "[".Yellow();

            for (int i = 0; i < s.Length; i++) {
                if (i > 0) result += ", ".Yellow();

                result += i == level - 1
                    ? colorCurrent(s[i])
                    : s[i].Yellow();
            }

            result += "]".Yellow();

            return result //
                .NoBreak()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);
        }

        public static string Name(this string s) =>
            s.VOffset(height: 2, delay: 0, offset: 0.0625f, duration: 0.25f, loop: true, loopDelay: 5, progressive: true);

        public static string PositiveEffect(this string s) =>
            s //
                .Green()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);

        public static string NegativeEffect(this string s) =>
            s //
                .Red()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);
    }
}
