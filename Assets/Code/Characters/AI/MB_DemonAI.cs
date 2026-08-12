using Code.Characters.Enemies;
using MyBox;
using UnityEngine;

namespace Code.Characters.AI {
    public class MB_DemonAI : AMB_AI {
        #region Members
        [Foldout("MB_DemonAI", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_Demon m_Demon;
        #endregion

        #region Getters / Setters
        private MB_Demon Demon { get => this.m_Demon; set => this.m_Demon = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.Demon = this.GetComponent<MB_Demon>();
        }
        #endregion

        protected override void UpdateBehaviour() {
            if (this.Demon.CanUseSpell()) {
                this.Demon.UseSpell();
            }
        }

        protected override Vector2 GetMovementDirection() => Vector2.zero;

        protected override Vector2 GetAimDirection() => Vector2.zero;
    }
}
