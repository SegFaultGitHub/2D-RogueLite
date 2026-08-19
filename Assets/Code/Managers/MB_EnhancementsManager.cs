using System;
using System.Collections.Generic;
using System.Linq;
using Code.Enhancements;
using Code.UI.HUD;
using Code.Utils;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
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
        [Foldout("MB_EnhancementsManager", true)][SerializeField]
        private List<MB_Enhancement> m_Enhancements;
        [SerializeField]
        private MB_EnhancementChoice m_EnhancementChoicePrefab;
        [FormerlySerializedAs("m_HUDCanvas")]
        [SerializeField]
        private Transform m_ChoicesParent;
        [SerializeField]
        private protected Button m_RerollButton;

        [Separator("Read only")]
        [ReadOnly]
        [SerializeField]
        private protected MB_ObjectsManager m_ObjectsManager;
        [ReadOnly]
        [SerializeField]
        private protected int m_RerollsRemaining;
        [ReadOnly]
        [SerializeField]
        private protected bool m_Locked = true;
        [ReadOnly]
        [SerializeField]
        private protected bool m_AlreadyChoosing;
        #endregion

        #region Getters / Setters
        public List<MB_Enhancement> Enhancements { get => this.m_Enhancements; }
        private MB_EnhancementChoice EnhancementChoicePrefab { get => this.m_EnhancementChoicePrefab; }
        private Transform ChoicesParent { get => this.m_ChoicesParent; }
        private Button RerollButton { get => this.m_RerollButton; set => this.m_RerollButton = value; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        private int RerollsRemaining { get => this.m_RerollsRemaining; set => this.m_RerollsRemaining = value; }
        private bool Locked { get => this.m_Locked; set => this.m_Locked = value; }
        private bool AlreadyChoosing { get => this.m_AlreadyChoosing; set => this.m_AlreadyChoosing = value; }
        #endregion

        #region Static / Readonly / Const
        private const int DEFAULT_REROLLS = 1;
        #endregion

        #region Unity methods
        #endregion

        public void Initialize() { }

        public void PostInitialize() {
            foreach (MB_Enhancement unlockEnhancement in this.Enhancements) {
                unlockEnhancement.Unlocked = unlockEnhancement.Enhancement.UnlockCondition.CheckGlobal(this.ObjectsManager);
            }
        }

        public void CheckUnlocks() {
            foreach (MB_Enhancement unlockEnhancement in this.Enhancements.Where(unlockEnhancement => !unlockEnhancement.Unlocked)) {
                unlockEnhancement.Unlocked = unlockEnhancement.Enhancement.UnlockCondition.CheckGlobal(this.ObjectsManager);

                if (unlockEnhancement.Unlocked) {
                    Debug.Log($"{unlockEnhancement.Enhancement.EnhancementName} unlocked!");
                    this.ObjectsManager.NotificationsContainer.CreateNotification(unlockEnhancement.Enhancement);
                    this.ObjectsManager.EnhancementList.UnlockEnhancement(unlockEnhancement.Enhancement.Enhancement);
                }
            }
        }

        public void GetChoices() => this.GetChoices(3, 1, 3, true, false);

        public void GetChoices(int count, int minLevel, int maxLevel, bool first = false, bool moveToNextRoom = true) {
            if (this.AlreadyChoosing) return;
            this.AlreadyChoosing = true;

            this.Until(
                () => this.ObjectsManager.PauseManager.PauseState == MB_PauseManager.E_PauseState.NotPaused,
                () => {
                    List<C_WeightedObject<AMB_Enhancement>> availableEnhancements = new();
                    foreach (MB_Enhancement enhancement in this.Enhancements) {
                        if (enhancement.Weight > 0
                            && this.ObjectsManager.Player.CanAddEnhancement(enhancement.Enhancement)
                            && enhancement.Enhancement.UnlockCondition.Check(this.ObjectsManager))
                            availableEnhancements.Add(
                                new C_WeightedObject<AMB_Enhancement> {
                                    Weight = enhancement.Weight,
                                    Obj = enhancement.Enhancement
                                }
                            );
                    }

                    if (availableEnhancements.Count == 0) {
                        if (moveToNextRoom) this.ObjectsManager.RoomManager.NextRoom();
                        return;
                    }

                    if (first) {
                        this.ObjectsManager.PauseManager.Pause(MB_PauseManager.E_PauseState.EnhancementChoices);

                        this.RerollButton.gameObject.SetActive(true);
                        this.RerollsRemaining = DEFAULT_REROLLS;
                        this.UpdateRerollButton();
                        this.ObjectsManager.DissolveManager.Show(
                            this.RerollButton.transform,
                            true,
                            MB_DissolveManager.E_Position.AfterBlur,
                            () => {
                                this.RerollButton.gameObject.SetActive(true);
                            }
                        );
                        this.RerollButton.gameObject.SetActive(false);
                    }

                    this.Locked = true;

                    List<C_WeightedObject<AMB_Enhancement>> enhancements = SC_Utils.Sample(availableEnhancements, count);
                    List<MB_EnhancementChoice> enhancementChoices = new();
                    foreach (C_WeightedObject<AMB_Enhancement> weightedObject in enhancements) {
                        AMB_Enhancement newEnhancement = Instantiate(weightedObject.Obj, this.transform);
                        newEnhancement.Level = Random.Range(minLevel, maxLevel + 1);
                        AMB_Enhancement existingEnhancement = this.ObjectsManager.Player.GetUpgradableEnhancement(newEnhancement);
                        MB_EnhancementChoice choice = Instantiate(this.EnhancementChoicePrefab, this.ChoicesParent);
                        choice.SetEnhancement(newEnhancement, existingEnhancement);
                        choice.OnClickStartAction = () => {
                            this.ObjectsManager.DissolveManager.Hide(
                                this.RerollButton.transform,
                                true,
                                MB_DissolveManager.E_Position.AfterBlur,
                                () => this.RerollButton.gameObject.SetActive(false)
                            );
                            this.RerollButton.gameObject.SetActive(false);
                        };
                        choice.OnClickEndAction = () => {
                            this.ObjectsManager.PauseManager.Unpause();
                            this.AlreadyChoosing = false;
                            if (moveToNextRoom) {
                                this.ObjectsManager.RoomManager.NextRoom();
                            }
                        };
                        enhancementChoices.Add(choice);
                    }

                    this.ObjectsManager.DissolveManager.Show(
                        this.ChoicesParent,
                        true,
                        MB_DissolveManager.E_Position.AfterBlur,
                        () => {
                            enhancementChoices.ForEach(e => {
                                    e.gameObject.SetActive(true);
                                    e.Ready = true;
                                    this.Locked = false;
                                }
                            );
                        }
                    );
                    enhancementChoices.ForEach(e => e.gameObject.SetActive(false));
                }
            );
        }

        [ButtonMethod]
        public void Reroll() => this.Reroll(3, 1, 3);

        public void Reroll(int count, int minLevel, int maxLevel) {
            if (this.Locked || this.RerollsRemaining <= 0) return;

            this.RerollsRemaining--;
            this.UpdateRerollButton();
            this.Locked = true;
            List<MB_EnhancementChoice> choices = this.ChoicesParent.GetComponentsInChildren<MB_EnhancementChoice>(true).ToList();
            this.ObjectsManager.DissolveManager.Hide(
                this.ChoicesParent,
                true,
                MB_DissolveManager.E_Position.AfterBlur,
                () => this.InSeconds(.125f, () => this.GetChoices(count, minLevel, maxLevel))
            );
            foreach (MB_EnhancementChoice choice in choices) {
                choice.Ready = false;
                Destroy(choice.Choice.gameObject);
                Destroy(choice.gameObject);
            }
        }

        private void UpdateRerollButton() {
            this.RerollButton.interactable = this.RerollsRemaining > 0;
        }
    }
}
