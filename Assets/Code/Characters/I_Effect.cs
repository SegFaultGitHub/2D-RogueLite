using System;
using System.Collections.Generic;

namespace Code.Characters {
    public interface I_Effect {
        public float ApplyOnDamageComputed(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, float value);
        public float ApplyOnDamageReceived(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, float value);
        public void ApplyOnDamageTaken(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, float value);
        public float ApplyToMovementSpeed(AMB_Character character, float speed);

        public float GetComputedDamageModifier(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, HashSet<Type> appliedTypes);
        public float GetReceivedDamageModifier(AMB_Character dealer, AMB_Character receiver, E_DamageSource damageSource, HashSet<Type> appliedTypes);
        public float GetCooldownModifier(AMB_Character character);

        public void OnApply(AMB_Character character);
        public void OnRemove(AMB_Character character);
    }
}
