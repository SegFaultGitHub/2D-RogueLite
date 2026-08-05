using System;
using System.Collections.Generic;
using System.Linq;
using Code.Enhancements;
using Code.UI.HUD;
using Code.Utils;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Managers {
    public class MB_EnhancementsManager : MonoBehaviour {
        [Serializable]
        public class MB_Enhancement {
            [SerializeField] private AMB_Enhancement m_Enhancement;
            [SerializeField] private float m_Weight;

            [Separator("Read only")]
            [ReadOnly][SerializeField] private bool m_Unlocked;

            public AMB_Enhancement Enhancement { get => this.m_Enhancement; }
            public float Weight { get => this.m_Weight; }
            public bool Unlocked { get => this.m_Unlocked; set => this.m_Unlocked = value; }
        }

        #region Members
        [Foldout("MB_EnhancementsManager", true)]
        [SerializeField] private List<MB_Enhancement> m_Enhancements;
        [SerializeField] private MB_EnhancementChoice m_EnhancementChoicePrefab;
        [SerializeField] private Transform m_HUDCanvas;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private List<MB_Enhancement> Enhancements { get => this.m_Enhancements; }
        private MB_EnhancementChoice EnhancementChoicePrefab { get => this.m_EnhancementChoicePrefab; }
        private Transform HUDCanvas { get => this.m_HUDCanvas; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void Initialize() { }

        public void PostInitialize() {
            foreach (MB_Enhancement unlockEnhancement in this.Enhancements) {
                unlockEnhancement.Unlocked = unlockEnhancement.Enhancement.UnlockCondition.Check(this.ObjectsManager);
            }
        }

        public void CheckUnlocks() {
            foreach (MB_Enhancement unlockEnhancement in this.Enhancements.Where(unlockEnhancement => !unlockEnhancement.Unlocked)) {
                unlockEnhancement.Unlocked = unlockEnhancement.Enhancement.UnlockCondition.Check(this.ObjectsManager);

                if (unlockEnhancement.Unlocked) {
                    Debug.Log($"{unlockEnhancement.Enhancement.EnhancementName} unlocked!");
                }
            }
        }

        [ButtonMethod]
        public void GetChoices() => this.GetChoices(3, 1, 3);

        public void GetChoices(int count, int minLevel, int maxLevel) {
            List<C_WeightedObject<AMB_Enhancement>> unlockedEnhancements = new();
            foreach (MB_Enhancement enhancement in this.Enhancements) {
                if (enhancement.Unlocked
                    && enhancement.Weight > 0
                    && this.ObjectsManager.Player.CanAddEnhancement(enhancement.Enhancement)
                    && enhancement.Enhancement.UnlockCondition.Check(this.ObjectsManager))
                    unlockedEnhancements.Add(
                        new C_WeightedObject<AMB_Enhancement> {
                            Weight = enhancement.Weight,
                            Obj = enhancement.Enhancement
                        }
                    );
            }

            if (unlockedEnhancements.Count == 0) return;

            List<C_WeightedObject<AMB_Enhancement>> enhancements = SC_Utils.Sample(unlockedEnhancements, count);
            List<MB_EnhancementChoice> enhancementChoices = new();
            foreach (C_WeightedObject<AMB_Enhancement> weightedObject in enhancements) {
                AMB_Enhancement newEnhancement = Instantiate(weightedObject.Obj, this.transform);
                newEnhancement.Level = Random.Range(minLevel, maxLevel + 1);
                AMB_Enhancement existingEnhancement = this.ObjectsManager.Player.GetUpgradableEnhancement(newEnhancement);
                MB_EnhancementChoice choice = Instantiate(this.EnhancementChoicePrefab, this.HUDCanvas);
                choice.SetEnhancement(newEnhancement, existingEnhancement);
                enhancementChoices.Add(choice);
            }

            enhancementChoices.ForEach(e => e.gameObject.SetActive(false));
            this.InUpdates(
                1,
                () => {
                    enhancementChoices.ForEach(e => e.gameObject.SetActive(true));
                    this.ObjectsManager.DissolveManager.Show(
                        new List<Transform> { this.HUDCanvas },
                        () => {
                            enhancementChoices.ForEach(e => {
                                    e.gameObject.SetActive(true);
                                    e.Ready = true;
                                }
                            );
                        }
                    );
                    enhancementChoices.ForEach(e => e.gameObject.SetActive(false));
                }
            );
        }
    }
}
