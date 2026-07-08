using Code.Characters;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Spells.Enemies {
    public class MB_SlimeMelee : AMB_PositionalSpell {
        #region Members
        [Foldout("MB_SlimeMelee", true)]
        [SerializeField] private protected float m_Duration;
        #endregion

        #region Getters / Setters
        private float Duration { get => this.m_Duration; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Start() {
            base.Start();

            this.InSeconds(this.Duration, this.Destroyer.Destroy);
        }
        #endregion

        public override void TriggerEnter(AMB_Character character) {
            E_SpellCollisionFlag flag = base.Collide(character);
            if (flag is not E_SpellCollisionFlag.Character and not E_SpellCollisionFlag.CharacterInvulnerable) return;

            character.Knockback(character.transform.position.ToVector2() - this.To, this.HitMetadata.KnockbackForce);
        }

        public override void TriggerStay(AMB_Character character) { }

        public override void TriggerExit(AMB_Character character) { }
    }
}
