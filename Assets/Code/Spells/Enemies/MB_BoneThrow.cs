using System;
using Code.Characters;
using Code.Characters.Effects;
using MyBox;
using UnityEngine;

namespace Code.Spells.Enemies {
    public class MB_BoneThrow : AMB_DirectionalSpell {
        #region Members
        [Foldout("MB_BoneThrow", true)]
        [SerializeField] private protected MB_Blind m_BlindPrefab;
        [SerializeField] private protected float m_BlindFOVSize;
        [SerializeField] private protected float m_BlindDuration;
        #endregion

        #region Getters / Setters
        private MB_Blind BlindPrefab { get => this.m_BlindPrefab; }
        private float BlindFOVSize { get => this.m_BlindFOVSize; }
        private float BlindDuration { get => this.m_BlindDuration; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Start() {
            base.Start();
            this.ObjectsManager.AudioManager.PlaySkeletonAttack();
        }
        #endregion

        public override E_SpellCollisionFlag Collide(AMB_Character character, bool expire = false) {
            E_SpellCollisionFlag flag = base.Collide(character, expire);

            switch (flag) {
                case E_SpellCollisionFlag.Character:
                case E_SpellCollisionFlag.CharacterInvulnerable:
                    MB_Blind blind = Instantiate(this.BlindPrefab);
                    blind.SetFOVSize(this.BlindFOVSize);
                    character.AddEffect(blind, this.Character, this.BlindDuration);
                    // this.ObjectsManager.AudioManager.PlayBoneHit();
                    break;
                case E_SpellCollisionFlag.Environment:
                    this.ObjectsManager.AudioManager.PlaySkeletonAttackMiss();
                    break;
                case E_SpellCollisionFlag.Expire:
                case E_SpellCollisionFlag.IgnoreCollision:
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            return flag;
        }
    }
}
