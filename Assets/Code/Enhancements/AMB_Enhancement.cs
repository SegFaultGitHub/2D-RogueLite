using System;
using System.Collections.Generic;
using Code.Characters;
using Code.Enhancements.UnlockConditions.Runtime;
using Code.Managers;
using Code.UI.EnhancementList;
using MyBox;
using UnityEngine;

namespace Code.Enhancements {
    public abstract class AMB_Enhancement : MonoBehaviour, I_Effect {
        #region Members
        [Foldout("AMB_Enhancement", true)]
        [SerializeField] private protected int m_MaxLevel;
        [SerializeField][TextArea(10, 20)] private protected string m_Description;
        [SerializeField] private protected Sprite m_Sprite;

        [SerializeField] private protected C_UnlockConditionsRuntimeGraph m_UnlockCondition;
        [SerializeField] private protected bool m_SecretUnlockConditions;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected int m_Level;
        #endregion

        #region Getters / Setters
        public int MaxLevel { get => this.m_MaxLevel; }
        protected string Description { get => this.m_Description; }
        public Sprite Sprite { get => this.m_Sprite; }

        public C_UnlockConditionsRuntimeGraph UnlockCondition { get => this.m_UnlockCondition; }
        private bool SecretUnlockConditions { get => this.m_SecretUnlockConditions; }

        public int Level { get => this.m_Level; set => this.m_Level = value; }

        public int EffectiveLevel { get => Mathf.Min(this.Level, this.MaxLevel); }
        public bool IsMaxLevel { get => this.EffectiveLevel == this.MaxLevel; }
        public abstract E_Enhancement Enhancement { get; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public abstract string GetFullDescription();
        public abstract string GetDescriptionWithUpgrade(int currentLevel, int newLevel);
        public abstract string GetDescription();

        public virtual void ApplyOnDashStart(AMB_Character character) { }
        public virtual void ApplyOnDashEnd(AMB_Character character) { }

        public virtual float ApplyOnDamageComputed(
            AMB_Character dealer,
            AMB_Character receiver,
            E_DamageSource damageSource,
            float value
        ) =>
            value;

        public virtual float ApplyOnDamageReceived(
            AMB_Character dealer,
            AMB_Character receiver,
            E_DamageSource damageSource,
            float value
        ) =>
            value;

        public virtual void ApplyOnDamageTaken(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, float value) { }

        public virtual void ApplyOnDamageInflicted(
            AMB_Character dealer,
            AMB_Character receiver,
            E_DamageSource damageSource,
            float value
        ) { }

        public virtual float ApplyToMovementSpeed(AMB_Character character, float speed) => speed;

        public virtual float GetComputedDamageModifier(
            AMB_Character dealer,
            AMB_Character receiver,
            E_DamageSource damageSource,
            HashSet<Type> appliedTypes
        ) =>
            0;

        public virtual float GetReceivedDamageModifier(
            AMB_Character dealer,
            AMB_Character receiver,
            E_DamageSource damageSource,
            HashSet<Type> appliedTypes
        ) =>
            0;

        public virtual float GetCooldownModifier(AMB_Character character) => 0;

        public virtual float GetCriticalRateModifier(AMB_Character character, E_DamageSource damageSource) => 0;
        public virtual float GetCriticalDamageModifier(AMB_Character character, E_DamageSource damageSource) => 0;

        public virtual void OnApply(AMB_Character character) { }
        public virtual void OnNew(AMB_Character character) { }
        public virtual void OnUpgrade(AMB_Character character, int previousLevel) { }
        public virtual void OnRemove(AMB_Character character) { }

        [ButtonMethod]
        public List<C_UnlockCondition> Foo(MB_ObjectsManager objectsManager) {
            List<C_UnlockCondition> unlockConditions = new();

            if (!this.SecretUnlockConditions) {
                this.UnlockCondition.GetVerbose(objectsManager, unlockConditions);
            } else {
                unlockConditions.Add(new C_UnlockCondition { Secret = true });
            }

            return unlockConditions;
        }
    }
}
