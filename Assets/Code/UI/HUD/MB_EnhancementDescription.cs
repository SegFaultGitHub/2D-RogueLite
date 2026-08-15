using System;
using System.Linq;
using Code.Enhancements;
using Code.UI.Text;
using Code.Utils;
using JetBrains.Annotations;
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
        public static string AsList(this string[] s, int level, string suffix, Func<string, string> colorCurrent) {
            string result = "";

            if (s.All(s1 => s1 == s[0])) {
                result = colorCurrent($"{s[0]}{suffix}");
            } else {
                result = "[".Yellow();

                for (int i = 0; i < s.Length; i++) {
                    if (i > 0) result += ", ".Yellow();

                    result += i == level - 1
                        ? colorCurrent(s[i])
                        : s[i].Yellow();
                }

                result += $"]{suffix}".Yellow();
            }

            return result //
                .NoBreak()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);
        }

        public static string Name(this string s) =>
            s.VOffset(height: 2, delay: 0, offset: 0.0625f, duration: 0.25f, loop: true, loopDelay: 5, progressive: false);

        public static string PositiveEffect(this string s) =>
            s //
                .Green()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);

        public static string NegativeEffect(this string s) =>
            s //
                .Red()
                .VOffset(height: 2, delay: 0, offset: .125f, duration: .5f, loop: true, loopDelay: 5, progressive: false);

        public static string Percentage(this float f, bool suffix = true) => $"{SC_Utils.FormatNumber(f * 100f, decimals: 0)}{(suffix ? "%" : "")}";
        public static string Duration(this float f) => SC_Utils.FormatNumber(f, decimals: 1);
        public static string Duration(this int i) => SC_Utils.FormatNumber(i, decimals: 1);
        public static string Damage(this float f) => SC_Utils.FormatNumber(f, decimals: 0);
        public static string Damage(this int i) => SC_Utils.FormatNumber(i, decimals: 0);
        public static string Count(this int i) => SC_Utils.FormatNumber(i, decimals: 0);
    }
}
