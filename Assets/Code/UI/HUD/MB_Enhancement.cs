using Code.Enhancements;
using Code.Utils;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace Code.UI.HUD {
    public class MB_Enhancement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        #region Members
        [Foldout("MB_Enhancement", true)]
        [SerializeField] private protected GameObject m_EnabledFrameFrame;
        [SerializeField] private protected GameObject m_DisabledFrameFrame;

        [SerializeField] private protected Image m_Icon;
        [SerializeField] private protected TMP_Text m_LevelText;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_PlayerHUD m_PlayerHUD;
        [ReadOnly][SerializeField] private protected AMB_Enhancement m_Enhancement;
        #endregion

        #region Getters / Setters
        private GameObject EnabledFrame { get => this.m_EnabledFrameFrame; }
        private GameObject DisabledFrame { get => this.m_DisabledFrameFrame; }

        private Image Icon { get => this.m_Icon; }
        private TMP_Text LevelText { get => this.m_LevelText; }

        private MB_PlayerHUD PlayerHUD { get => this.m_PlayerHUD; set => this.m_PlayerHUD = value; }
        public AMB_Enhancement Enhancement { get => this.m_Enhancement; private set => this.m_Enhancement = value; }

        private Coroutine ShowCoroutine { get; set; }
        #endregion

        #region Static / Readonly / Const
        private const float SHOW_DELAY = .5f;
        #endregion

        #region Unity methods
        private void Awake() {
            this.PlayerHUD = this.GetComponentInParent<MB_PlayerHUD>();
            this.SetDisabled();
        }
        #endregion

        private void SetEnabled() {
            this.EnabledFrame.SetActive(true);
            this.DisabledFrame.SetActive(false);
        }

        private void SetDisabled() {
            this.EnabledFrame.SetActive(false);
            this.DisabledFrame.SetActive(true);
        }

        public void SetEnhancement(AMB_Enhancement enhancement) {
            this.SetEnabled();
            this.Enhancement = enhancement;
            this.SetLevel(enhancement.EffectiveLevel);
            this.Icon.sprite = enhancement.Sprite;
            this.Icon.rectTransform.SetWidth(this.Icon.sprite.rect.width);
            this.Icon.rectTransform.SetHeight(this.Icon.sprite.rect.height);
        }

        public void SetLevel(int level) => this.LevelText.text = level.ToString();

        public void OnPointerEnter(PointerEventData eventData) {
            this.ShowCoroutine = this.InSeconds(SHOW_DELAY, () => this.PlayerHUD.ShowEnhancement(this));
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (this.ShowCoroutine != null) this.StopCoroutine(this.ShowCoroutine);

            this.ShowCoroutine = null;
            this.PlayerHUD.HideEnhancement();
        }
    }
}
