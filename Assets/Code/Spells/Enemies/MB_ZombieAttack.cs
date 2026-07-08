using System;
using Code.Characters;
using Code.Characters.Effects;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Spells.Enemies {
    public class MB_ZombieAttack : AMB_RotatingSpell {
        #region Members
        [Foldout("MB_ZombieAttack", true)]
        [SerializeField] private protected MB_Poison m_PoisonPrefab;

        [SerializeField] private protected float m_DamagePerTick;
        [SerializeField] private protected int m_TickCount;
        #endregion

        #region Getters / Setters
        private MB_Poison PoisonPrefab { get => this.m_PoisonPrefab; }

        private float DamagePerTick { get => this.m_DamagePerTick; }
        private int TickCount { get => this.m_TickCount; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Start() {
            base.Start();
            this.ObjectsManager.AudioManager.PlayZombieAttack();
        }
        #endregion

        public override E_SpellCollisionFlag Collide(AMB_Character character, bool expire = false) {
            E_SpellCollisionFlag flag = character == this.Character
                ? E_SpellCollisionFlag.IgnoreCollision
                : base.Collide(character, expire);

            switch (flag) {
                case E_SpellCollisionFlag.Character:
                case E_SpellCollisionFlag.CharacterInvulnerable:
                    MB_Poison poison = Instantiate(this.PoisonPrefab);
                    poison.SetDamage(this.DamagePerTick, this.TickCount);
                    character.AddEffect(poison, this.Character);
                    // this.ObjectsManager.AudioManager.PlayGenericHit();
                    break;
                case E_SpellCollisionFlag.Expire:
                case E_SpellCollisionFlag.Environment:
                case E_SpellCollisionFlag.IgnoreCollision:
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            return flag;
        }

        public override void SetInitialAngle(float angle, bool reverseRotation) => base.SetInitialAngle(angle, SC_Utils.Rate(.5f));
    }
}
