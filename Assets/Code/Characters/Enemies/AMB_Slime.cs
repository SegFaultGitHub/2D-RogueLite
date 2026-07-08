using Code.Characters.AI;
using Code.Characters.Controllers.Enemies;
using Code.Spells;
using Code.Spells.Enemies;
using MyBox;
using UnityEngine;

namespace Code.Characters.Enemies {
    public abstract class AMB_Slime : AMB_Enemy {
        #region Members
        [Foldout("AMB_Slime", true)]
        [SerializeField] private protected MB_SlimeMelee m_SlimeMelee;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_SlimeAI m_SlimeAI;
        [ReadOnly][SerializeField] private protected MB_SlimeController m_SlimeController;
        #endregion

        #region Getters / Setters
        private MB_SlimeMelee SlimeMelee { get => this.m_SlimeMelee; }

        private MB_SlimeAI SlimeAI { get => this.m_SlimeAI; set => this.m_SlimeAI = value; }
        public MB_SlimeController SlimeController { get => this.m_SlimeController; private set => this.m_SlimeController = value; }

        public override E_Enemy Enemy { get => E_Enemy.SmallSlime; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();

            this.SlimeAI = this.GetComponent<MB_SlimeAI>();
            this.SlimeController = this.GetComponent<MB_SlimeController>();
        }
        #endregion

        public void Jump() {
            this.SlimeController.StartJump(this.SlimeAI.GetJumpDirection());
        }

        public void UseSpell() {
            this.UseSpell(this.SlimeMelee, this.transform.position);
        }

        protected override void PlayHurtSoundEffect() => this.ObjectsManager.AudioManager.PlaySlimeHurt();
    }
}
