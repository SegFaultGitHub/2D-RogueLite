using System;
using System.Collections.Generic;
using Code.Characters;
using MyBox;
using UnityEditor;
using UnityEngine;

namespace Code.Spells {
    public abstract class AMB_MovingSpell : AMB_Spell {
        #region Members
        [Foldout("AMB_MovingSpell", true)]
        [SerializeField] private protected bool m_CharacterPiercing;
        [SerializeField] private protected bool m_EnvironmentPiercing;

        [SerializeField] private protected Transform m_Sprites;
        [SerializeField] private protected Transform m_Shadow;

        [SerializeField] private protected bool m_Animated;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Vector2 m_Direction;

        [ConditionalField(nameof(m_Animated), false, true)]
        [ReadOnly][SerializeField] private protected Animator m_Animator;
        [ReadOnly][SerializeField] private protected bool m_HasCollided;
        [ReadOnly][SerializeField] private protected Rigidbody2D m_Rigidbody;

        [ReadOnly][SerializeField] private protected List<Collider2D> m_CollidedEnvironmentColliders = new();
        #endregion

        #region Getters / Setters
        protected bool CharacterPiercing { get => this.m_CharacterPiercing; }
        protected bool EnvironmentPiercing { get => this.m_EnvironmentPiercing; }

        protected Transform Sprites { get => this.m_Sprites; }
        private Transform Shadow { get => this.m_Shadow; }

        private bool Animated { get => this.m_Animated; }

        protected Vector2 Direction { get => this.m_Direction; set => this.m_Direction = value.normalized; }

        private Animator Animator { get => this.m_Animator; set => this.m_Animator = value; }
        private bool HasCollided { get => this.m_HasCollided; set => this.m_HasCollided = value; }
        protected Rigidbody2D Rigidbody { get => this.m_Rigidbody; private set => this.m_Rigidbody = value; }

        private List<Collider2D> CollidedEnvironmentColliders { get => this.m_CollidedEnvironmentColliders; }
        #endregion

        #region Static / Readonly / Const
        private static readonly int DESTROY = Animator.StringToHash("Destroy");
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();

            this.Rigidbody = this.GetComponent<Rigidbody2D>();
            if (this.Animated) this.Animator = this.GetComponent<Animator>();
        }

        protected virtual void Update() {
            if (this.Shadow != null) this.Shadow.right = this.Direction;
        }

        private void OnCollisionEnter2D(Collision2D other) {
            Collider2D thisCollider = other.otherCollider.GetComponent<Collider2D>();
            if (!this.EnvironmentPiercing
                && this.EnvironmentColliders.Contains(thisCollider)
                && !this.CollidedEnvironmentColliders.Contains(thisCollider))
                this.CollidedEnvironmentColliders.Add(thisCollider);

            AMB_Character character = other.gameObject.GetComponentInParent<AMB_Character>();

            if (character != null
                || !this.EnvironmentPiercing && this.CollidedEnvironmentColliders.Count == this.EnvironmentColliders.Count)
                this.Collide(character);
        }

        private void OnCollisionStay2D(Collision2D collision) => this.OnCollisionEnter2D(collision);
        #endregion

        public override E_SpellCollisionFlag Collide(AMB_Character character, bool expire = false) {
            if (!this.CharacterPiercing && !this.EnvironmentPiercing && this.HasCollided) return E_SpellCollisionFlag.IgnoreCollision;
            this.HasCollided = true;

            E_SpellCollisionFlag flag = base.Collide(character, expire);

            if (flag == E_SpellCollisionFlag.Character) {
                character.Knockback(this.Direction, this.HitMetadata.KnockbackForce);
            }

            if (this.DestroyOnCollision(flag)) this.Destroy();

            return flag;
        }

        protected void Destroy() {
            Destroy(this.Rigidbody);
            Destroy(this.CharacterCollider);

            if (this.Animated) this.Animator.SetTrigger(DESTROY);
            else {
                SpriteRenderer[] sprites = this.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sprite in sprites) {
                    sprite.enabled = false;
                }

                this.Destroyer.Destroy();
            }
        }

        private bool DestroyOnCollision(E_SpellCollisionFlag collisionFlag) {
            return collisionFlag switch {
                E_SpellCollisionFlag.Character or E_SpellCollisionFlag.CharacterInvulnerable => !this.CharacterPiercing,
                E_SpellCollisionFlag.Environment => !this.EnvironmentPiercing
                                                    && this.CollidedEnvironmentColliders.Count == this.EnvironmentColliders.Count,
                E_SpellCollisionFlag.Expire => true,
                E_SpellCollisionFlag.IgnoreCollision => false,
                _ => throw new ArgumentOutOfRangeException(nameof(collisionFlag), collisionFlag, null)
            };
        }
    }
}
