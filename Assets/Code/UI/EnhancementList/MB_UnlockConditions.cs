using System;
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

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private MB_Text Text { get => this.m_Text; }
        private RectTransform PaperRect { get => this.m_PaperRect; }

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
            this.Text.SetText(enhancement.Foo(this.ObjectsManager).LineHeight(8));

            this.PaperRect.SetHeight((this.Text.GetLineCount() + 2) * 8);
        }
    }
}
