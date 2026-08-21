using System;
using System.Collections.Generic;
using Code.Enhancements;
using Code.Managers;
using Code.UI.Text;
using MyBox;
using UnityEngine;

namespace Code.UI.EnhancementList {
    public class MB_UnlockConditions : MonoBehaviour {
        #region Members
        [Foldout("MB_UnlockConditions", true)]
        [SerializeField] private protected MB_Text m_Text;
        [SerializeField] private protected RectTransform m_PaperRect;
        [SerializeField] private protected Transform m_Grid;
        [SerializeField] private protected MB_UnlockCondition m_UnlockConditionPrefab;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private MB_Text Text { get => this.m_Text; }
        private RectTransform PaperRect { get => this.m_PaperRect; }
        private Transform Grid { get => this.m_Grid; }
        private MB_UnlockCondition UnlockConditionPrefab { get => this.m_UnlockConditionPrefab; }

        private MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        private void Awake() {
            this.ObjectsManager = FindAnyObjectByType<MB_ObjectsManager>(FindObjectsInactive.Include);
        }
        #endregion

        public void SetEnhancement(AMB_Enhancement enhancement) {
            for (int i = this.Grid.childCount - 1; i >= 0; i--) {
                DestroyImmediate(this.Grid.GetChild(i).gameObject);
            }

            List<C_UnlockCondition> unlockConditions = enhancement.Foo(this.ObjectsManager);
            int y = 0;
            foreach (C_UnlockCondition unlockCondition in unlockConditions) {
                MB_UnlockCondition line = Instantiate(this.UnlockConditionPrefab, this.Grid);
                line.Set(unlockCondition);
                line.transform.localPosition = new Vector3(0, -y, 0);
                y += line.Text.GetLineCount() * 8;
            }

            this.PaperRect.SetHeight(y + 16);

            //this.Text.SetText(enhancement.Foo(this.ObjectsManager).LineHeight(8));
            //this.PaperRect.SetHeight((this.Text.GetLineCount() + 2) * 8);
        }
    }
}
