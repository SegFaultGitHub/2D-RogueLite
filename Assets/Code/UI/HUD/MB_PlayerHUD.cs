using Code.Characters.Players;
using Code.Enhancements;
using Code.Managers;
using Code.UI.Misc;
using MyBox;
using UnityEngine;

namespace Code.UI.HUD {
    public class MB_PlayerHUD : MonoBehaviour {
        #region Members
        [Foldout("MB_PlayerHUD", true)]
        [SerializeField] private protected MB_HealthBar m_HealthBar;
        [SerializeField] private protected MB_ProgressBar m_MainSpellBar;
        [SerializeField] private protected MB_ProgressBar m_SecondarySpellBar;

        [SerializeField] private protected GameObject m_MagePortrait;
        [SerializeField] private protected GameObject m_RangerPortrait;
        [SerializeField] private protected GameObject m_KnightPortrait;

        [SerializeField] private protected MB_EnhancementDescription m_EnhancementDescription;
        [SerializeField] private protected Transform m_EnhancementLinesParent;
        [SerializeField] private protected MB_EnhancementLine m_EnhancementLinePrefab;
        [SerializeField] private protected MB_Enhancement m_EnhancementPrefab;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        [ReadOnly][SerializeField] private protected CollectionWrapperList<MB_Enhancement> m_Enhancements;
        #endregion

        #region Getters / Setters
        private MB_HealthBar HealthBar { get => this.m_HealthBar; }
        private MB_ProgressBar MainSpellBar { get => this.m_MainSpellBar; }
        private MB_ProgressBar SecondarySpellBar { get => this.m_SecondarySpellBar; }

        private GameObject MagePortrait { get => this.m_MagePortrait; }
        private GameObject RangerPortrait { get => this.m_RangerPortrait; }
        private GameObject KnightPortrait { get => this.m_KnightPortrait; }

        private MB_EnhancementDescription EnhancementDescription { get => this.m_EnhancementDescription; }
        private Transform EnhancementLinesParent { get => this.m_EnhancementLinesParent; }
        private MB_EnhancementLine EnhancementLinePrefab { get => this.m_EnhancementLinePrefab; }
        private MB_Enhancement EnhancementPrefab { get => this.m_EnhancementPrefab; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        private CollectionWrapperList<MB_Enhancement> Enhancements { get => this.m_Enhancements; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void Initialize() {
            for (int i = 0; i < AMB_Player.DEFAULT_MAX_ENHANCEMENTS; i++) {
                this.AddEnhancementSlot();
            }
            this.HideEnhancement();
        }

        public void PostInitialize() { }

        public void AddEnhancementSlot() {
            int currentBonusCount = 0;
            for (int i = 0; i < this.EnhancementLinesParent.childCount; i++) {
                currentBonusCount += this.EnhancementLinesParent.GetChild(i).GetComponent<MB_EnhancementLine>().Count;
            }

            for (int i = this.EnhancementLinesParent.childCount;
                 i < Mathf.Floor(currentBonusCount / (float)MB_EnhancementLine.SIZE) + 1;
                 i++) {
                Instantiate(this.EnhancementLinePrefab, this.EnhancementLinesParent);
            }

            MB_Enhancement enhancement = Instantiate(
                this.EnhancementPrefab,
                this.EnhancementLinesParent.GetChild(this.EnhancementLinesParent.childCount - 1)
            );
            this.Enhancements.Add(enhancement);
        }

        public MB_Enhancement AddEnhancement(AMB_Enhancement enhancement) {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (MB_Enhancement uiEnhancement in this.Enhancements.Value) {
                if (uiEnhancement.Enhancement == null) {
                    uiEnhancement.SetEnhancement(enhancement);
                    return uiEnhancement;
                }
            }

            return null;
        }

        public void UpdateEnhancement(AMB_Enhancement enhancement) {
            this.Enhancements.Value.Find(e => e.Enhancement == enhancement).SetLevel(enhancement.EffectiveLevel);
        }

        public void ShowEnhancement(MB_Enhancement enhancement) {
            this.EnhancementDescription.gameObject.SetActive(true);
            this.EnhancementDescription.SetEnhancement(enhancement.Enhancement);
        }

        public void HideEnhancement() {
            this.EnhancementDescription.gameObject.SetActive(false);
        }

        public void SetClass(E_Class playerClass) {
            this.MagePortrait.SetActive(playerClass == E_Class.Mage);
            this.RangerPortrait.SetActive(playerClass == E_Class.Ranger);
            this.KnightPortrait.SetActive(playerClass == E_Class.Knight);
        }

        public void SetHealth(float currentHealth, float maxHealth) {
            this.HealthBar.SetHealth(currentHealth, maxHealth);
        }

        public void SetMainSpellRatio(float ratio) => this.MainSpellBar.SetRatio(ratio);

        public void SetSecondarySpellRatio(float ratio) => this.SecondarySpellBar.SetRatio(ratio);

        #region Input
        private PlayerInputs PlayerInputs { get; set; }

        protected void OnEnable() {
            this.PlayerInputs = new PlayerInputs();
            this.PlayerInputs.Actions.Enable();
        }

        protected void OnDisable() {
            this.PlayerInputs.Actions.Disable();
        }
        #endregion
    }
}
