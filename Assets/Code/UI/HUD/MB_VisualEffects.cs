using Code.Characters;
using Code.Characters.Effects;
using MyBox;
using UnityEngine;

namespace Code.UI.HUD {
    public class MB_VisualEffects : MonoBehaviour {
        #region Members
        [Foldout("MB_VisualEffects", true)]
        [SerializeField] private protected GameObject m_Confused;
        [SerializeField] private protected GameObject m_Poisoned;
        [SerializeField] private protected GameObject m_Burning;
        [SerializeField] private protected GameObject m_Blind;
        [SerializeField] private protected GameObject m_ShortSighted;

        [Separator("Read only")]
        [SerializeField] private protected bool m_IsConfused;
        [SerializeField] private protected bool m_IsPoisoned;
        [SerializeField] private protected bool m_IsBurning;
        [SerializeField] private protected bool m_IsBlind;
        [SerializeField] private protected bool m_IsShortSighted;
        #endregion

        #region Getters / Setters
        private GameObject Confused { get => this.m_Confused; }
        private GameObject Poisoned { get => this.m_Poisoned; }
        private GameObject Burning { get => this.m_Burning; }
        private GameObject Blind { get => this.m_Blind; }
        private GameObject ShortSighted { get => this.m_ShortSighted; }

        private bool IsConfused { get => this.m_IsConfused; set => this.m_IsConfused = value; }
        private bool IsPoisoned { get => this.m_IsPoisoned; set => this.m_IsPoisoned = value; }
        private bool IsBurning { get => this.m_IsBurning; set => this.m_IsBurning = value; }
        public bool IsBlind { get => this.m_IsBlind; set => this.m_IsBlind = value; }
        private bool IsShortSighted { get => this.m_IsShortSighted; set => this.m_IsShortSighted = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void UpdateVisuals(AMB_Character character) {
            bool previousConfused = this.Confused.activeSelf;
            bool previousPoisoned = this.Poisoned.activeSelf;
            bool previousBurning = this.Burning.activeSelf;
            bool previousBlind = this.Blind.activeSelf;
            bool previousShortSighted = this.ShortSighted.activeSelf;
            this.IsConfused = false;
            this.IsPoisoned = false;
            this.IsBurning = false;
            this.IsBlind = false;
            this.IsShortSighted = false;

            foreach (AMB_Effect effect in character.Effects.Value) {
                if (effect.GetType() == typeof(MB_Confused)) this.IsConfused = true;
                if (effect.GetType() == typeof(MB_Poison)) this.IsPoisoned = true;
                if (effect.GetType() == typeof(MB_Burning)) this.IsBurning = true;
                if (effect.GetType() == typeof(MB_Blind)) this.IsBlind = true;
                if (effect.GetType() == typeof(MB_ShortSighted)) this.IsShortSighted = true;
            }

            if (!previousConfused && this.IsConfused) this.Confused.transform.SetAsLastSibling();
            this.Confused.SetActive(this.IsConfused);

            if (!previousPoisoned && this.IsPoisoned) this.Poisoned.transform.SetAsLastSibling();
            this.Poisoned.SetActive(this.IsPoisoned);

            if (!previousBurning && this.IsBurning) this.Burning.transform.SetAsLastSibling();
            this.Burning.SetActive(this.IsBurning);

            if (!previousBlind && this.IsBlind) this.Blind.transform.SetAsLastSibling();
            this.Blind.SetActive(this.IsBlind);

            if (!previousShortSighted && this.IsShortSighted) this.ShortSighted.transform.SetAsLastSibling();
            this.ShortSighted.SetActive(this.IsShortSighted);
        }
    }
}
