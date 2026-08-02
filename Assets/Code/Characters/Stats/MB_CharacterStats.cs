using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace Code.Characters.Stats {
    public class MB_CharacterStats : MonoBehaviour {
        #region Members
        [Foldout("MB_CharacterStats", true)]
        [SerializeField] private protected int m_CurrentHealth;
        [SerializeField] private protected int m_MaxHealth;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected AMB_Character m_Character;
        #endregion

        #region Getters / Setters
        public int CurrentHealth {
            get => this.m_CurrentHealth;
            private set => this.m_CurrentHealth = Mathf.Clamp(value, 0, this.m_MaxHealth);
        }
        public int MissingHealth { get => this.m_MaxHealth - this.m_CurrentHealth; }
        public int MaxHealth { get => this.m_MaxHealth; }
        public float HealthRatio { get => (float)this.CurrentHealth / this.MaxHealth; }

        private AMB_Character Character { get => this.m_Character; set => this.m_Character = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        private void Start() {
            this.Character = this.GetComponent<AMB_Character>();
        }
        #endregion

        public bool IsDead() => this.CurrentHealth <= 0;

        public int TakeDamage(AMB_Character from, float value, bool critical, E_DamageSource source) {
            HashSet<Type> appliedTypes = new();
            float damageModifier = this.Character.AllEffects.Aggregate(
                0f,
                (acc, effect) => acc + effect.GetReceivedDamageModifier(from, this.Character, source, appliedTypes)
            );

            value *= 1 + damageModifier;
            value = this.Character.AllEffects.Aggregate(
                value,
                (current, effect) => effect.ApplyOnDamageReceived(from, this.Character, source, current)
            );

            int realDamageDealt = Mathf.RoundToInt(Mathf.Clamp(value, 0, this.CurrentHealth));
            this.CurrentHealth -= realDamageDealt;
            return realDamageDealt;
        }

        public int Heal(AMB_Character from, float value) {
            int realHeal = Mathf.RoundToInt(Mathf.Clamp(value, 0, this.MissingHealth));
            this.CurrentHealth += realHeal;
            return realHeal;
        }

        public (float damage, bool critical) ComputeDamage(AMB_Character target, float baseDamage, E_DamageSource source) {
            HashSet<Type> appliedTypes = new();
            float damageModifier = this.Character.AllEffects.Aggregate(
                0f,
                (acc, effect) => acc + effect.GetComputedDamageModifier(this.Character, target, source, appliedTypes)
            );

            baseDamage *= 1 + damageModifier;
            baseDamage = this.Character.AllEffects.Aggregate(
                baseDamage,
                (current, effect) => effect.ApplyOnDamageComputed(this.Character, target, source, current)
            );

            return (baseDamage, false);
        }
    }
}
