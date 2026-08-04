using System;
using System.Collections.Generic;
using System.Linq;
using Code.Enhancements;
using MyBox;
using UnityEngine;

namespace Code.Managers {
    public class MB_UnlockManager : MonoBehaviour {
        [Serializable]
        public class MB_UnlockEnhancement {
            [SerializeField] private AMB_Enhancement m_Enhancement;

            [Separator("Read only")]
            [ReadOnly][SerializeField] private bool m_Unlocked;

            public AMB_Enhancement Enhancement { get => this.m_Enhancement; }
            public bool Unlocked { get => this.m_Unlocked; set => this.m_Unlocked = value; }
        }

        #region Members
        [Foldout("MB_UnlockManager", true)]
        [SerializeField] private List<MB_UnlockEnhancement> m_Enhancements;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private List<MB_UnlockEnhancement> Enhancements { get => this.m_Enhancements; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void Initialize() {
            foreach (MB_UnlockEnhancement unlockEnhancement in this.Enhancements) {
                unlockEnhancement.Unlocked = unlockEnhancement.Enhancement.UnlockCondition.Check(this.ObjectsManager);
            }}

        public void PostInitialize() { }

        public void CheckUnlocks() {
            foreach (MB_UnlockEnhancement unlockEnhancement in this.Enhancements.Where(unlockEnhancement => !unlockEnhancement.Unlocked)) {
                unlockEnhancement.Unlocked = unlockEnhancement.Enhancement.UnlockCondition.Check(this.ObjectsManager);

                if (unlockEnhancement.Unlocked) {
                    Debug.Log($"{unlockEnhancement.Enhancement.EnhancementName} unlocked!");
                }
            }
        }
    }
}
