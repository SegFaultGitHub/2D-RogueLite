using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Characters.Effects;
using Code.Managers;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Characters.Controllers {
    public abstract class AMB_BaseController : MonoBehaviour {
        protected enum E_DashDirection {
            Movement,
            Aim
        }

        #region Members
        [Foldout("AMB_BaseController", true)]
        [SerializeField] private protected float m_MovementSpeed;
        [SerializeField] private protected TrailRenderer m_DashTrail;
        [SerializeField] private protected List<Collider2D> m_SpellColliders;
        [SerializeField] private protected List<Collider2D> m_EnvironmentColliders;
        [SerializeField] private protected LayerMask m_CharactersLayers;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;

        [ReadOnly][SerializeField] private protected Animator m_Animator;
        [ReadOnly][SerializeField] private protected Rigidbody2D m_Rigidbody;

        [ReadOnly][SerializeField] private protected bool m_Active = true;
        [ReadOnly][SerializeField] private protected AMB_Character m_Character;
        [ReadOnly][SerializeField] private protected Vector2 m_AimDirection;
        [ReadOnly][SerializeField] private protected Vector2 m_ForceAimDirection;
        [ReadOnly][SerializeField] private protected Vector2 m_MovementDirection;
        [ReadOnly][SerializeField] private protected bool m_Knockbacked;
        [ReadOnly][SerializeField] private protected bool m_Dashing;
        [ReadOnly][SerializeField] private protected E_DashDirection m_DashDirection;

        private Coroutine KnockbackCoroutine { get; set; }
        private Coroutine DashCoroutine { get; set; }
        #endregion

        #region Getters / Setters
        protected float MovementSpeed { get => this.m_MovementSpeed; set => this.m_MovementSpeed = value; }
        private TrailRenderer DashTrail { get => this.m_DashTrail; }
        protected List<Collider2D> SpellColliders { get => this.m_SpellColliders; }
        protected List<Collider2D> EnvironmentColliders { get => this.m_EnvironmentColliders; }
        protected LayerMask CharactersLayers { get => this.m_CharactersLayers; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }

        protected Animator Animator { get => this.m_Animator; private set => this.m_Animator = value; }
        private Rigidbody2D Rigidbody { get => this.m_Rigidbody; set => this.m_Rigidbody = value; }

        public bool Active { get => this.m_Active; set => this.m_Active = value; }
        protected AMB_Character Character { get => this.m_Character; private set => this.m_Character = value; }
        public Vector2 AimDirection { get => this.m_AimDirection; protected set => this.m_AimDirection = value; }
        public Vector2 ForceAimDirection { get => this.m_ForceAimDirection; set => this.m_ForceAimDirection = value; }
        protected Vector2 MovementDirection { get => this.m_MovementDirection; set => this.m_MovementDirection = value; }
        private bool Knockbacked { get => this.m_Knockbacked; set => this.m_Knockbacked = value; }
        private bool Dashing { get => this.m_Dashing; set => this.m_Dashing = value; }
        protected E_DashDirection DashDirection { get => this.m_DashDirection; set => this.m_DashDirection = value; }

        public float AimAngle { get => Vector2.SignedAngle(Vector2.right, this.AimDirection); }
        
        private int DisabledCollisionStack { get; set; }
        #endregion

        #region Static / Readonly / Const
        protected static readonly int DIRECTION = Animator.StringToHash("Direction");
        private static readonly int MOVING = Animator.StringToHash("Moving");
        private const float KNOCKBACK_DURATION = AMB_Character.INVULNERABILITY_DURATION;
        private const float DASH_DURATION = .2f;
        #endregion

        #region Unity methods
        protected virtual void Awake() {
            this.Character = this.GetComponent<AMB_Character>();
            this.Rigidbody = this.GetComponentInChildren<Rigidbody2D>();
            this.Animator = this.GetComponent<Animator>();

            this.ObjectsManager = FindFirstObjectByType<MB_ObjectsManager>(FindObjectsInactive.Include);
        }

        protected virtual void FixedUpdate() {
            if (this.Knockbacked) {
                this.Rigidbody.linearVelocity = Vector2.Lerp(
                    this.Rigidbody.linearVelocity,
                    this.MovementDirection * this.GetMovementSpeed(),
                    0.1f
                );
                this.Animator.SetBool(MOVING, false);
            } else if (this.Dashing) {
                this.Rigidbody.linearVelocity = Vector2.Lerp(
                    this.Rigidbody.linearVelocity,
                    this.MovementDirection * this.GetMovementSpeed(),
                    0.05f
                );
                this.Animator.SetBool(MOVING, false);
            } else {
                Vector2 movementDirection = this.Active
                    ? this.MovementDirection
                    : Vector2.zero;
                this.Animator.SetBool(MOVING, movementDirection.magnitude > 0.1f);
                this.Rigidbody.linearVelocity = movementDirection * this.GetMovementSpeed();
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Vector3 position = this.transform.position;
            Gizmos.DrawLine(position, new Vector3(this.MovementDirection.x, this.MovementDirection.y) + position);
        }
        #endif
        #endregion

        protected virtual void Aim(Vector2 position) { }

        public void Knockback(Vector2 direction, float force) {
            if (this.KnockbackCoroutine != null) this.StopCoroutine(this.KnockbackCoroutine);

            IEnumerator _Knockback() {
                yield return new WaitForSeconds(KNOCKBACK_DURATION);
                this.OnKnockbackEnd();
            }
            this.Knockbacked = true;
            this.KnockbackCoroutine = this.StartCoroutine(_Knockback());
            this.Rigidbody.linearVelocity = direction * force;
        }

        private void OnKnockbackEnd() {
            this.Knockbacked = false;
        }

        public void Dash() {
            if (this.DashCoroutine != null) this.StopCoroutine(this.DashCoroutine);

            IEnumerator _Dash() {
                yield return new WaitForSeconds(DASH_DURATION);
                this.OnDashEnd();
            }
            this.Dashing = true;
            this.Character.OnDashStart();
            this.Character.Invulnerable = true;
            this.DashTrail.emitting = true;
            this.DashCoroutine = this.StartCoroutine(_Dash());
            this.Rigidbody.linearVelocity = //
                this.DashDirection switch {
                    E_DashDirection.Movement when this.MovementDirection != Vector2.zero => this.MovementDirection.normalized,
                    E_DashDirection.Movement when this.MovementDirection == Vector2.zero => this.AimDirection.normalized,
                    E_DashDirection.Aim => this.AimDirection.normalized,
                    _ => throw new ArgumentOutOfRangeException()
                }
                * this.GetMovementSpeed()
                * 6;
            this.DisableCollisions();
        }

        private void OnDashEnd() {
            this.Dashing = false;
            this.Character.OnDashEnd();
            this.InSeconds(AMB_Character.INVULNERABILITY_DURATION, () => this.Character.Invulnerable = false);
            if (this.DashTrail != null) {
                // Delay trail ends for smoother result
                this.InSeconds(.1f, () => {
                    if (this.Dashing) return;
                    this.DashTrail.emitting = false;
                });
            }
            this.EnableCollisions();
        }

        public void EnableCollisions() {
            this.DisabledCollisionStack--;

            if (this.DisabledCollisionStack > 0) return;
            
            foreach (Collider2D spellCollider in this.SpellColliders)
                spellCollider.enabled = true;
            foreach (Collider2D environmentCollider in this.EnvironmentColliders)
                environmentCollider.excludeLayers = 0;
        }
        
        public void DisableCollisions() {
            this.DisabledCollisionStack++;
            
            foreach (Collider2D spellCollider in this.SpellColliders)
                spellCollider.enabled = false;
            foreach (Collider2D environmentCollider in this.EnvironmentColliders)
                environmentCollider.excludeLayers = this.CharactersLayers;
        }

        private float GetMovementSpeed() {
            float speedModifier = this.Character.SpeedRatio.Value.Aggregate(1f, (acc, curr) => acc * curr.Ratio);
            speedModifier = this.Character.AllEffects.Aggregate(speedModifier, (current, effect) => effect.ApplyToMovementSpeed(this.Character, current));

            return speedModifier * this.MovementSpeed;
        }
    }
}
