using Code.Characters.Players;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Characters.Controllers.Players {
    public class MB_PlayerController : AMB_BaseController {
        #region Members
        [Foldout("MB_PlayerController", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Vector2 m_AimPosition;

        [ReadOnly][SerializeField] private protected AMB_Player m_Player;
        [ReadOnly][SerializeField] private protected Camera m_Camera;
        #endregion

        #region Getters / Setters
        public Vector2 AimPosition { get => this.m_AimPosition; private set => this.m_AimPosition = value; }

        private AMB_Player Player { get => this.m_Player; set => this.m_Player = value; }
        private Camera Camera { get => this.m_Camera; set => this.m_Camera = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.Player = this.GetComponent<AMB_Player>();
        }

        private void Start() {
            this.Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            this.Aim(this.Camera.ScreenToWorldPoint(this.PlayerInputs.Actions.AimMouse.ReadValue<Vector2>()));
        }
        #endregion

        protected override void Aim(Vector2 position) {
            this.AimPosition = position;
            // Aim direction
            Vector2 characterPosition = this.Character.Center.position.ToVector2();
            Vector2 direction = position - characterPosition;

            this.AimDirection = direction;

            switch (this.AimDirection.x) {
                case >= 0:
                    this.Animator.SetInteger(DIRECTION, 0);
                    break;
                case < 0:
                    this.Animator.SetInteger(DIRECTION, 1);
                    break;
            }

            this.Player.Aim(direction);
        }

        #region Input
        private PlayerInputs PlayerInputs { get; set; }

        protected void OnEnable() {
            this.PlayerInputs = new PlayerInputs();
            this.PlayerInputs.Actions.Enable();

            this.PlayerInputs.Actions.Move.started += this.MoveInput;
            this.PlayerInputs.Actions.Move.performed += this.MoveInput;
            this.PlayerInputs.Actions.Move.canceled += this.MoveInput;

            this.PlayerInputs.Actions.Dash.started += this.DashInput;

            this.PlayerInputs.Actions.UseMainSpell.started += this.UseMainSpellInputStarted;
            this.PlayerInputs.Actions.UseMainSpell.canceled += this.UseMainSpellInputCanceled;

            this.PlayerInputs.Actions.UseSecondarySpell.started += this.UseSecondarySpellInputStarted;
            this.PlayerInputs.Actions.UseSecondarySpell.canceled += this.UseSecondarySpellInputCanceled;
        }

        protected void OnDisable() {
            this.PlayerInputs.Actions.Move.started -= this.MoveInput;
            this.PlayerInputs.Actions.Move.performed -= this.MoveInput;
            this.PlayerInputs.Actions.Move.canceled -= this.MoveInput;

            this.PlayerInputs.Actions.Dash.started -= this.DashInput;

            this.PlayerInputs.Actions.UseMainSpell.started -= this.UseMainSpellInputStarted;
            this.PlayerInputs.Actions.UseMainSpell.canceled -= this.UseMainSpellInputCanceled;

            this.PlayerInputs.Actions.UseSecondarySpell.started -= this.UseSecondarySpellInputStarted;
            this.PlayerInputs.Actions.UseSecondarySpell.canceled -= this.UseSecondarySpellInputCanceled;

            this.PlayerInputs.Actions.Disable();
        }

        private void MoveInput(InputAction.CallbackContext context) {
            if (!this.Active) {
                this.MovementDirection = Vector2.zero;
                return;
            }
            this.MovementDirection = context.ReadValue<Vector2>().normalized;
        }

        private void DashInput(InputAction.CallbackContext context) {
            if (!this.Active) return;
            this.Player.Dash();
        }

        private void UseMainSpellInputStarted(InputAction.CallbackContext _) {
            if (!this.Active) return;
            this.Player.UsingMainSpell = true;
        }

        private void UseMainSpellInputCanceled(InputAction.CallbackContext _) {
            if (!this.Active) return;
            this.Player.UsingMainSpell = false;
        }

        private void UseSecondarySpellInputStarted(InputAction.CallbackContext _) {
            if (!this.Active) return;
            this.Player.UsingSecondarySpell = true;
        }

        private void UseSecondarySpellInputCanceled(InputAction.CallbackContext _) {
            if (!this.Active) return;
            this.Player.UsingSecondarySpell = false;
        }
        #endregion
    }
}
