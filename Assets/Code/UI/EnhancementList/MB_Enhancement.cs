using Code.Enhancements;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI.EnhancementList {
    public class MB_Enhancement : MonoBehaviour, IPointerEnterHandler {
        #region Members
        [Foldout("MB_Enhancement", true)]
        [SerializeField] private protected GameObject m_Unlocked;
        [SerializeField] private protected GameObject m_Locked;

        [SerializeField] private protected TMP_Text[] m_Texts;
        [SerializeField] private protected Image[] m_Icons;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_EnhancementList m_EnhancementList;
        [ReadOnly][SerializeField] private protected Managers.MB_EnhancementsManager.MB_Enhancement m_Enhancement;
        #endregion

        #region Getters / Setters
        private GameObject Unlocked { get => this.m_Unlocked; }
        private GameObject Locked { get => this.m_Locked; }

        private TMP_Text[] Texts { get => this.m_Texts; }
        private Image[] Icons { get => this.m_Icons; }

        private MB_EnhancementList EnhancementList { get => this.m_EnhancementList; set => this.m_EnhancementList = value; }
        public Managers.MB_EnhancementsManager.MB_Enhancement Enhancement {
            get => this.m_Enhancement;
            private set => this.m_Enhancement = value;
        }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        private void Awake() {
            this.EnhancementList = this.GetComponentInParent<MB_EnhancementList>();
        }
        #endregion

        public void SetEnhancement(Managers.MB_EnhancementsManager.MB_Enhancement enhancement) {
            this.Enhancement = enhancement;

            foreach (TMP_Text text in this.Texts) {
                text.text = text.text.Replace("<name>", enhancement.EnhancementName);
            }

            foreach (Image icon in this.Icons) {
                icon.sprite = enhancement.Enhancement.Sprite;
                icon.rectTransform.SetWidth(icon.sprite.rect.width);
                icon.rectTransform.SetHeight(icon.sprite.rect.height);
            }
        }

        public void SetLocked() {
            this.Unlocked.SetActive(false);
            this.Locked.SetActive(true);
        }

        [ButtonMethod]
        public void SetUnlocked() {
            this.Unlocked.SetActive(true);
            this.Locked.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!this.Enhancement.Unlocked) this.EnhancementList.ShowUnlockConditions(this);
            else this.EnhancementList.ShowDescription(this);
        }
    }
}
