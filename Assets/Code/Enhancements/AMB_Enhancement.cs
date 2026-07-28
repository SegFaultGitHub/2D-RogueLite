using System;
using System.Collections.Generic;
using Code.Characters;
using Code.UI.Text;
using MyBox;
using UnityEngine;

namespace Code.Enhancements {
    public abstract class AMB_Enhancement : MonoBehaviour, I_Effect {
        #region Members
        [Foldout("AMB_Enhancement", true)]
        [SerializeField] private protected string m_EnhancementName;
        [SerializeField] private protected int m_MaxLevel;
        [SerializeField][TextArea(10, 20)] private protected string m_Description;
        [SerializeField] private protected Sprite m_Sprite;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected int m_Level;
        #endregion

        #region Getters / Setters
        public string EnhancementName { get => this.m_EnhancementName; }
        public int MaxLevel { get => this.m_MaxLevel; }
        protected string Description { get => this.m_Description; }
        public Sprite Sprite { get => this.m_Sprite; }

        public int Level { get => this.m_Level; set => this.m_Level = value; }

        public int EffectiveLevel { get => Mathf.Min(this.Level, this.MaxLevel); }
        public bool IsMaxLevel { get => this.EffectiveLevel == this.MaxLevel; }
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
        public virtual void ApplyOnDamageInflicted(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, float value) { }
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

        public virtual void OnApply(AMB_Character character) { }
        public virtual void OnRemove(AMB_Character character) { }
    }
}
