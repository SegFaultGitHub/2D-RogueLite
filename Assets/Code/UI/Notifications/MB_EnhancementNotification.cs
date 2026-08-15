using Code.Enhancements;
using Code.Utils;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Notifications {
    public class MB_EnhancementNotification : AMB_Notification {
        #region Members
        [Foldout("MB_EnhancementNotification", true)]
        [SerializeField] private protected RectTransform m_ContainerRect;
        [SerializeField] private protected TMP_Text m_Text;
        [SerializeField] private protected Image m_Icon;
        #endregion

        #region Getters / Setters
        private RectTransform ContainerRect { get => this.m_ContainerRect; }
        private TMP_Text Text { get => this.m_Text; }
        private Image Icon { get => this.m_Icon; }

        public override float Width { get => this.ContainerRect.sizeDelta.x; }
        public override float Height { get => this.ContainerRect.sizeDelta.y; }
        #endregion

        #region Static / Readonly / Const
        private const float OFFSET_WIDTH = 26;
        #endregion

        #region Unity methods
        #endregion

        public void SetEnhancement(AMB_Enhancement enhancement) {
            this.Text.SetText(this.Text.text.Replace("<name>", enhancement.name));
            this.Text.ForceMeshUpdate();
            this.Icon.sprite = enhancement.Sprite;
            this.Icon.rectTransform.SetWidth(this.Icon.sprite.rect.width);
            this.Icon.rectTransform.SetHeight(this.Icon.sprite.rect.height);
            this.ContainerRect.sizeDelta = new Vector2(
                Mathf.CeilToInt(OFFSET_WIDTH + this.Text.preferredWidth),
                this.ContainerRect.sizeDelta.y
            );
        }
    }
}
