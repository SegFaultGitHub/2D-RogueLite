using System;
using System.Collections.Generic;
using System.Linq;
using Code.Enhancements;
using Code.Managers;
using Code.UI.Text;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.UI.HUD {
    public class MB_EnhancementChoice : MB_EnhancementDescription, IPointerClickHandler {
        #region Members
        [Foldout("MB_EnhancementChoice", true)]
        [SerializeField] private protected GameObject m_New;
        [SerializeField] private protected GameObject m_Upgrade;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected AMB_Enhancement m_Choice;
        [ReadOnly][SerializeField] private protected bool m_Ready;

        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private GameObject New { get => this.m_New; }
        private GameObject Upgrade { get => this.m_Upgrade; }

        public AMB_Enhancement Choice { get => this.m_Choice; set => this.m_Choice = value; }
        public bool Ready { get => this.m_Ready; set => this.m_Ready = value; }

        private MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }

        public Action OnClickStartAction { get; set; }
        public Action OnClickEndAction { get; set; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        private void Awake() {
            this.ObjectsManager = FindFirstObjectByType<MB_ObjectsManager>(FindObjectsInactive.Include);
            this.Ready = false;
        }
        #endregion

        public void SetEnhancement(AMB_Enhancement enhancement, AMB_Enhancement currentEnhancement) {
            if (currentEnhancement == null) {
                this.SetEnhancement(enhancement);

                this.New.SetActive(true);
                this.Upgrade.SetActive(false);
            } else {
                string nameString = enhancement.EnhancementName.Name();
                int currentLevel = currentEnhancement.Level;
                int newLevel = Mathf.Clamp(enhancement.Level + currentEnhancement.Level, 1, enhancement.MaxLevel);
                string levelString =
                    $"{currentEnhancement.EffectiveLevel} / {currentEnhancement.MaxLevel} > {newLevel} / {enhancement.MaxLevel}";
                string description = enhancement.GetDescriptionWithUpgrade(currentLevel, newLevel).LineHeight(8);

                this.SetData(nameString, levelString, description);

                this.New.SetActive(false);
                this.Upgrade.SetActive(true);
            }

            this.Choice = enhancement;
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (!this.Ready) return;

            this.OnClickStartAction?.Invoke();
            List<MB_EnhancementChoice> choices = this.transform.parent.GetComponentsInChildren<MB_EnhancementChoice>(true).ToList();
            this.ObjectsManager.Player.AddEnhancement(this.Choice);
            this.ObjectsManager.DissolveManager.Hide(this.transform.parent, this.OnClickEndAction);
            foreach (MB_EnhancementChoice choice in choices) {
                choice.Ready = false;
                if (choice != this) Destroy(choice.Choice.gameObject);
                Destroy(choice.gameObject);
            }
        }
    }
}
