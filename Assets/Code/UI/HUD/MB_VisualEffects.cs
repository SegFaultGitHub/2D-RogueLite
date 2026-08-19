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
        [SerializeField] private protected GameObject m_Berserk;
        [SerializeField] private protected GameObject m_Focus;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected bool m_HasConfused;
        [ReadOnly][SerializeField] private protected bool m_HasPoisoned;
        [ReadOnly][SerializeField] private protected bool m_HasBurning;
        [ReadOnly][SerializeField] private protected bool m_HasBlind;
        [ReadOnly][SerializeField] private protected bool m_HasShortSighted;
        [ReadOnly][SerializeField] private protected bool m_HasBerserk;
        [ReadOnly][SerializeField] private protected bool m_HasFocus;
        #endregion

        #region Getters / Setters
        private GameObject Confused { get => this.m_Confused; }
        private GameObject Poisoned { get => this.m_Poisoned; }
        private GameObject Burning { get => this.m_Burning; }
        private GameObject Blind { get => this.m_Blind; }
        private GameObject ShortSighted { get => this.m_ShortSighted; }
        private GameObject Berserk { get => this.m_Berserk; }
        private GameObject Focus { get => this.m_Focus; }

        private bool HasConfused { get => this.m_HasConfused; set => this.m_HasConfused = value; }
        private bool HasPoisoned { get => this.m_HasPoisoned; set => this.m_HasPoisoned = value; }
        private bool HasBurning { get => this.m_HasBurning; set => this.m_HasBurning = value; }
        private bool HasBlind { get => this.m_HasBlind; set => this.m_HasBlind = value; }
        private bool HasShortSighted { get => this.m_HasShortSighted; set => this.m_HasShortSighted = value; }
        private bool HasBerserk { get => this.m_HasBerserk; set => this.m_HasBerserk = value; }
        private bool HasFocus { get => this.m_HasFocus; set => this.m_HasFocus = value; }
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
            bool previousBerserk = this.Berserk.activeSelf;
            bool previousFocus = this.Focus.activeSelf;
            this.HasConfused = false;
            this.HasPoisoned = false;
            this.HasBurning = false;
            this.HasBlind = false;
            this.HasShortSighted = false;
            this.HasBerserk = false;
            this.HasFocus = false;

            foreach (AMB_Effect effect in character.Effects.Value) {
                if (effect.GetType() == typeof(MB_Confused)) this.HasConfused = true;
                if (effect.GetType() == typeof(MB_Poison)) this.HasPoisoned = true;
                if (effect.GetType() == typeof(MB_Burning)) this.HasBurning = true;
                if (effect.GetType() == typeof(MB_Blind)) this.HasBlind = true;
                if (effect.GetType() == typeof(MB_ShortSighted)) this.HasShortSighted = true;
                if (effect.GetType() == typeof(MB_Berserk)) this.HasBerserk = true;
                if (effect.GetType() == typeof(MB_Focus)) this.HasFocus = true;
            }

            if (!previousConfused && this.HasConfused) this.Confused.transform.SetAsLastSibling();
            this.Confused.SetActive(this.HasConfused);

            if (!previousPoisoned && this.HasPoisoned) this.Poisoned.transform.SetAsLastSibling();
            this.Poisoned.SetActive(this.HasPoisoned);

            if (!previousBurning && this.HasBurning) this.Burning.transform.SetAsLastSibling();
            this.Burning.SetActive(this.HasBurning);

            if (!previousBlind && this.HasBlind) this.Blind.transform.SetAsLastSibling();
            this.Blind.SetActive(this.HasBlind);

            if (!previousShortSighted && this.HasShortSighted) this.ShortSighted.transform.SetAsLastSibling();
            this.ShortSighted.SetActive(this.HasShortSighted);

            if (!previousBerserk && this.HasBerserk) this.Berserk.transform.SetAsLastSibling();
            this.Berserk.SetActive(this.HasBerserk);

            if (!previousFocus && this.HasFocus) this.Focus.transform.SetAsLastSibling();
            this.Focus.SetActive(this.HasFocus);
        }
    }
}
