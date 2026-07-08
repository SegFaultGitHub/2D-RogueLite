using Code.Characters;
using MyBox;
using UnityEngine;

namespace Code.Spells {
    public abstract class AMB_PositionalSpell : AMB_Spell {
        public enum E_LineOfSight {
            ThroughWallsAndHoles,
            ThroughHoles,
            ThroughNothing
        }

        #region Members
        [Foldout("AMB_PositionalSpell", true)]
        [SerializeField] private protected float m_MaxDistance;
        [SerializeField] private protected E_LineOfSight m_LineOfSight;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Vector2 m_From;
        [ReadOnly][SerializeField] private protected Vector2 m_To;
        #endregion

        #region Getters / Setters
        public float MaxDistance { get => this.m_MaxDistance; }
        public E_LineOfSight LineOfSight { get => this.m_LineOfSight; }

        protected Vector2 From { get => this.m_From; private set => this.m_From = value; }
        protected Vector2 To { get => this.m_To; private set => this.m_To = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public virtual void SetPositions(Vector2 from, Vector2 to) {
            this.From = from;
            this.To = to;
        }

        public abstract void TriggerEnter(AMB_Character character);
        public abstract void TriggerStay(AMB_Character character);
        public abstract void TriggerExit(AMB_Character character);
    }
}
