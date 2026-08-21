using System;
using System.Collections.Generic;
using System.Linq;
using Code.Enhancements;
using Code.Managers;
using Code.UI.HUD;
using Code.UI.Text;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.UI.EnhancementList {
    public class MB_EnhancementList : MonoBehaviour {
        #region Members
        [Foldout("MB_EnhancementList", true)]
        [SerializeField] private protected MB_Enhancement m_EnhancementPrefab;
        [SerializeField] private protected Transform m_Grid;
        [SerializeField] private protected MB_EnhancementDescription m_EnhancementDescription;
        [SerializeField] private protected MB_UnlockConditions m_UnlockConditions;
        [SerializeField] private protected string m_UnlockedCountBaseText;
        [SerializeField] private protected MB_Text m_UnlockedCountText;
        [SerializeField] private protected List<Color> m_UnlockedColors;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;

        [ReadOnly][SerializeField] private protected Dictionary<E_Enhancement, MB_Enhancement> m_Enhancements;
        #endregion

        #region Getters / Setters
        private MB_Enhancement EnhancementPrefab { get => this.m_EnhancementPrefab; }
        private Transform Grid { get => this.m_Grid; }
        private MB_EnhancementDescription EnhancementDescription { get => this.m_EnhancementDescription; }
        private MB_UnlockConditions UnlockConditions { get => this.m_UnlockConditions; }
        private string UnlockedCountBaseText { get => this.m_UnlockedCountBaseText; }
        private MB_Text UnlockedCountText { get => this.m_UnlockedCountText; }
        private List<Color> UnlockedColors { get => this.m_UnlockedColors; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }

        private Dictionary<E_Enhancement, MB_Enhancement> Enhancements { get => this.m_Enhancements; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        private void Start() {
            this.UpdateUnlocked();
        }

        private void OnEnable() {
            this.HideDescriptionAndUnlockConditions();
        }
        #endregion

        public void Initialize() { }

        public void PostInitialize() {
            foreach (MB_EnhancementsManager.MB_Enhancement enhancement in this.ObjectsManager.EnhancementsManager.Enhancements) {
                this.AddEnhancement(enhancement);
            }
        }

        public void UnlockEnhancement(E_Enhancement enhancement) {
            this.Enhancements[enhancement].SetUnlocked();
            this.UpdateUnlocked();
        }

        private void AddEnhancement(MB_EnhancementsManager.MB_Enhancement enhancement) {
            MB_Enhancement uiEnhancement = Instantiate(this.EnhancementPrefab, this.Grid);
            this.Enhancements[enhancement.Enhancement.Enhancement] = uiEnhancement;
            uiEnhancement.SetEnhancement(enhancement);
            if (enhancement.Unlocked)
                uiEnhancement.SetUnlocked();
            else
                uiEnhancement.SetLocked();
        }

        [ButtonMethod]
        private void UpdateUnlocked() {
            int current = this.Enhancements.Values.Count(e => e.Enhancement.Unlocked);
            int total = this.Enhancements.Values.Count;

            int index = Mathf.Clamp(
                Mathf.FloorToInt(SC_Utils.MapFrom(0, total, 0, this.UnlockedColors.Count - 1, current)),
                0,
                this.UnlockedColors.Count - 1
            );

            string text = this.UnlockedCountBaseText //
                .Replace("<current>", SC_Utils.FormatNumber(current).Color(this.UnlockedColors[index]))
                .Replace("<total>", SC_Utils.FormatNumber(total));
            this.UnlockedCountText.SetText(text);
        }

        public void ShowDescription(MB_Enhancement enhancement) {
            this.HideDescriptionAndUnlockConditions();
            this.EnhancementDescription.gameObject.SetActive(true);
            this.EnhancementDescription.SetEnhancement(enhancement.Enhancement.Enhancement);
        }

        public void ShowUnlockConditions(MB_Enhancement enhancement) {
            this.HideDescriptionAndUnlockConditions();
            this.UnlockConditions.gameObject.SetActive(true);
            this.UnlockConditions.SetEnhancement(enhancement.Enhancement.Enhancement);
        }

        private void HideDescriptionAndUnlockConditions() {
            this.EnhancementDescription.gameObject.SetActive(false);
            this.UnlockConditions.gameObject.SetActive(false);
        }
    }
}
