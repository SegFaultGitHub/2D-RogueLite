using System;
using System.Collections.Generic;
using System.Linq;
using Code.Characters;
using Code.Characters.Enemies;
using Code.Characters.Players;
using Code.Characters.Stats;
using Code.Managers;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Spells {
    [RequireComponent(typeof(MB_DestroyOnAnimationEvent))]
    public abstract class AMB_Spell : MonoBehaviour {
        #region Members
        [Foldout("AMB_Spell", true)]
        [SerializeField] private protected E_SpellPosition m_SpellPosition;
        [SerializeField] private protected E_CollisionTarget m_CollisionTarget;
        [SerializeField] private protected C_HitMetadata m_HitMetadata;
        [SerializeField] private protected float m_Cooldown;

        [SerializeField] private protected bool m_CanHitMultipleTimes;
        [SerializeField] private protected bool m_TriggersInvulnerable;

        [SerializeField] private protected bool m_HasCharacterCollider;
        [ConditionalField(nameof(m_HasCharacterCollider), false, true)]
        [SerializeField] private protected Collider2D m_CharacterCollider;
        [SerializeField] private protected bool m_HasEnvironmentColliders;
        [ConditionalField(nameof(m_HasEnvironmentColliders), false, true)]
        [SerializeField] private protected CollectionWrapper<Collider2D> m_EnvironmentColliders;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected int m_Level;

        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        [ReadOnly][SerializeField] private protected AMB_Character m_Character;
        [ReadOnly][SerializeField] private protected MB_DestroyOnAnimationEvent m_Destroyer;

        [ReadOnly][SerializeField] private protected MB_CharacterStats m_BackupCharacterStats;

        [ReadOnly][SerializeField] private protected CollectionWrapperList<AMB_Character> m_AffectedCharacters = new();
        #endregion

        #region Getters / Setters
        private E_SpellPosition SpellPosition { get => this.m_SpellPosition; }
        protected E_CollisionTarget CollisionTarget { get => this.m_CollisionTarget; }
        public C_HitMetadata HitMetadata { get => this.m_HitMetadata; }
        public float Cooldown { get => this.m_Cooldown; protected set => this.m_Cooldown = value; }

        private bool CanHitMultipleTimes { get => this.m_CanHitMultipleTimes; }
        private bool TriggersInvulnerable { get => this.m_TriggersInvulnerable; }

        private bool HasCharacterCollider { get => this.m_HasCharacterCollider; }
        protected Collider2D CharacterCollider { get => this.m_CharacterCollider; }
        private bool HasEnvironmentColliders { get => this.m_HasEnvironmentColliders; }
        protected List<Collider2D> EnvironmentColliders { get => this.m_EnvironmentColliders.Value.ToList(); }

        protected int Level { get => this.m_Level; private set => this.m_Level = value; }

        protected MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; private set => this.m_ObjectsManager = value; }
        protected AMB_Character Character { get => this.m_Character; private set => this.m_Character = value; }
        protected MB_DestroyOnAnimationEvent Destroyer { get => this.m_Destroyer; private set => this.m_Destroyer = value; }

        private MB_CharacterStats BackupCharacterStats {
            get => this.m_BackupCharacterStats;
            set => this.m_BackupCharacterStats = value;
        }

        protected CollectionWrapperList<AMB_Character> AffectedCharacters { get => this.m_AffectedCharacters; }
        #endregion

        #region Static / Readonly / Const
        private const string CHARACTERS_COLLISION_LAYER = "Spells/Collide with characters - Body";
        private const string ENEMIES_COLLISION_LAYER = "Spells/Collide with enemies - Body";
        private const string ALL_COLLISION_LAYER = "Spells/Collide with everything - Body";
        private const string CHARACTERS_COLLISION_GROUND_LOW_LAYER = "Spells/Collide with characters - Ground (Low)";
        private const string CHARACTERS_COLLISION_GROUND_HIGH_LAYER = "Spells/Collide with characters - Ground (High)";
        private const string ENEMIES_COLLISION_GROUND_LOW_LAYER = "Spells/Collide with enemies - Ground (Low)";
        private const string ENEMIES_COLLISION_GROUND_HIGH_LAYER = "Spells/Collide with enemies - Ground (High)";
        private const string ALL_COLLISION_GROUND_LOW_LAYER = "Spells/Collide with everything - Ground (Low)";
        private const string ALL_COLLISION_GROUND_HIGH_LAYER = "Spells/Collide with everything - Ground (High)";

        private const string ENVIRONMENT_COLLISION_GROUND_LOW_LAYER = "Spells/Collide with environment - Ground (Low)";
        private const string ENVIRONMENT_COLLISION_GROUND_HIGH_LAYER = "Spells/Collide with environment - Ground (High)";
        #endregion

        #region Unity methods
        protected virtual void Awake() {
            this.ObjectsManager = FindFirstObjectByType<MB_ObjectsManager>(FindObjectsInactive.Include);
            this.transform.SetParent(this.ObjectsManager.SpellsTransform);
            this.Destroyer = this.GetComponent<MB_DestroyOnAnimationEvent>();
        }

        protected virtual void Start() { }
        #endregion

        public virtual E_SpellCollisionFlag Collide(AMB_Character character, bool expire = false) {
            if (!this.CanHitMultipleTimes) {
                if (this.AffectedCharacters.Contains(character)) return E_SpellCollisionFlag.IgnoreCollision;
                this.AffectedCharacters.Add(character);
            }

            if (expire) return E_SpellCollisionFlag.Expire;
            if (character == null) return E_SpellCollisionFlag.Environment;
            if (character.Invulnerable) return E_SpellCollisionFlag.CharacterInvulnerable;

            if (this.HitMetadata.Damage > 0) {
                (float damage, bool critical) = this.Character == null
                    ? this.BackupCharacterStats.ComputeDamage(character, this.HitMetadata.Damage, E_DamageSource.Spell)
                    : this.Character.ComputeDamage(character, this.HitMetadata.Damage, E_DamageSource.Spell);
                character.TakeDamage(this.TriggersInvulnerable, true, damage, critical, this.Character, E_DamageSource.Spell);
            }

            return E_SpellCollisionFlag.Character;
        }

        public virtual void ExitCollision(AMB_Character character) { }

        public virtual void SetCharacter(AMB_Character character) {
            this.Character = character;
            this.BackupCharacterStats = character.CharacterStats;

            if (this.HasEnvironmentColliders) {
                switch (this.SpellPosition) {
                    case E_SpellPosition.Air:
                        foreach (Collider2D environmentCollider in this.EnvironmentColliders)
                            environmentCollider.SetLayerRecursively(ENVIRONMENT_COLLISION_GROUND_HIGH_LAYER);
                        break;
                    case E_SpellPosition.GroundLow:
                    case E_SpellPosition.GroundHigh:
                        foreach (Collider2D environmentCollider in this.EnvironmentColliders)
                            environmentCollider.SetLayerRecursively(ENVIRONMENT_COLLISION_GROUND_LOW_LAYER);
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            if (this.HasCharacterCollider) {
                string enemiesCollisionLayer = this.SpellPosition switch {
                    E_SpellPosition.Air => ENEMIES_COLLISION_LAYER,
                    E_SpellPosition.GroundLow => ENEMIES_COLLISION_GROUND_LOW_LAYER,
                    E_SpellPosition.GroundHigh => ENEMIES_COLLISION_GROUND_HIGH_LAYER,
                    _ => throw new ArgumentOutOfRangeException()
                };
                string charactersCollisionLayer = this.SpellPosition switch {
                    E_SpellPosition.Air => CHARACTERS_COLLISION_LAYER,
                    E_SpellPosition.GroundLow => CHARACTERS_COLLISION_GROUND_LOW_LAYER,
                    E_SpellPosition.GroundHigh => CHARACTERS_COLLISION_GROUND_HIGH_LAYER,
                    _ => throw new ArgumentOutOfRangeException()
                };
                string allCollisionLayer = this.SpellPosition switch {
                    E_SpellPosition.Air => ALL_COLLISION_LAYER,
                    E_SpellPosition.GroundLow => ALL_COLLISION_GROUND_LOW_LAYER,
                    E_SpellPosition.GroundHigh => ALL_COLLISION_GROUND_HIGH_LAYER,
                    _ => throw new ArgumentOutOfRangeException()
                };

                switch (character) {
                    case AMB_Player:
                        switch (this.CollisionTarget) {
                            case E_CollisionTarget.Enemies:
                                this.CharacterCollider?.SetLayerRecursively(enemiesCollisionLayer);
                                break;
                            case E_CollisionTarget.Allies:
                                this.CharacterCollider?.SetLayerRecursively(charactersCollisionLayer);
                                break;
                            case E_CollisionTarget.Everyone:
                                this.CharacterCollider?.SetLayerRecursively(allCollisionLayer);
                                break;
                            default: throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case AMB_Enemy:
                        switch (this.CollisionTarget) {
                            case E_CollisionTarget.Enemies:
                                this.CharacterCollider?.SetLayerRecursively(charactersCollisionLayer);
                                break;
                            case E_CollisionTarget.Allies:
                                this.CharacterCollider?.SetLayerRecursively(enemiesCollisionLayer);
                                break;
                            case E_CollisionTarget.Everyone:
                                this.CharacterCollider?.SetLayerRecursively(allCollisionLayer);
                                break;
                            default: throw new ArgumentOutOfRangeException();
                        }

                        break;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        public virtual void SetLevel(int level) => this.Level = level;

        public virtual void AddBaseDamage(float damage) { }
    }
}
