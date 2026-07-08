using System;
using Code.Characters;
using Code.Characters.Players;

namespace Code.Spells.Knight {
    public class MB_SwordSwing : AMB_RotatingSpell {
        #region Members
        // [Foldout("MB_SwordSwing", true)]
        #endregion

        #region Getters / Setters
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Start() {
            base.Start();
            this.Character.GetComponent<MB_Knight>().HideSword();
        }
        #endregion

        public override E_SpellCollisionFlag Collide(AMB_Character character, bool expire = false) {
            E_SpellCollisionFlag flag = character == this.Character
                ? E_SpellCollisionFlag.IgnoreCollision
                : base.Collide(character, expire);

            switch (flag) {
                case E_SpellCollisionFlag.Character:
                case E_SpellCollisionFlag.CharacterInvulnerable:
                    // this.ObjectsManager.AudioManager.PlayGenericHit();
                    break;
                case E_SpellCollisionFlag.Expire:
                    this.Character.GetComponent<MB_Knight>().ShowSword();
                    break;
                case E_SpellCollisionFlag.Environment:
                case E_SpellCollisionFlag.IgnoreCollision:
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            return flag;
        }

        public override void SetInitialAngle(float angle, bool reverseRotation) {
            base.SetInitialAngle(angle, this.Character.GetComponent<MB_Knight>().SwordDirection);
            this.Character.GetComponent<MB_Knight>().SwordDirection = !this.Character.GetComponent<MB_Knight>().SwordDirection;
        }
    }
}
