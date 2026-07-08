using System;
using System.Collections.Generic;
using Code.Managers;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Characters.Effects {
    public abstract class AMB_Effect : MonoBehaviour, I_Effect {
        #region Members
        [Foldout("AMB_Effect", true)]
        [SerializeField] private protected int m_Priority;
        [SerializeField] private protected bool m_Unique;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected AMB_Character m_Character;
        [ReadOnly][SerializeField] private protected AMB_Character m_From;

        [ReadOnly][SerializeField] private protected float m_ExpiresAt;

        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        public int Priority { get => this.m_Priority; }
        public bool Unique { get => this.m_Unique; }

        protected AMB_Character Character { get => this.m_Character; private set => this.m_Character = value; }
        protected AMB_Character From { get => this.m_From; private set => this.m_From = value; }

        private float ExpiresAt { get => this.m_ExpiresAt; set => this.m_ExpiresAt = value; }

        protected MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; private set => this.m_ObjectsManager = value; }

        public float? TTL { get => this.ExpiresAt - Time.time; }
        public bool IsPermanent { get => this.ExpiresAt < 0; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void Initialize(AMB_Character target, AMB_Character from, float? duration = null) {
            this.Character = target;
            this.From = from;
            this.ObjectsManager = FindFirstObjectByType<MB_ObjectsManager>(FindObjectsInactive.Include);
            this.ExpiresAt = duration.HasValue
                ? Time.time + duration.Value
                : -1;

            if (duration.HasValue) {
                this.InSeconds(duration.Value, () => target.RemoveEffect(this));
            }
        }

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
