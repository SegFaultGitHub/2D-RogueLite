using Code.Spells;
using Code.Spells.Enemies;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Characters.Enemies {
    public class MB_Demon : AMB_Enemy {
        #region Members
        [Foldout("MB_Demon", true)]
        [SerializeField] private protected MB_Meteor m_Meteor;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected float m_SpellCastAt;
        [ReadOnly][SerializeField] private protected float m_SpellAvailableAt;
        #endregion

        #region Getters / Setters
        private MB_Meteor Meteor { get => this.m_Meteor; }

        private float SpellCastAt { get => this.m_SpellCastAt; set => this.m_SpellCastAt = value; }
        private float SpellAvailableAt { get => this.m_SpellAvailableAt; set => this.m_SpellAvailableAt = value; }

        public override E_Enemy Enemy { get => E_Enemy.Demon; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public bool CanUseSpell() => this.SpellAvailableAt <= Time.time;

        public void UseSpell() {
            if (!this.CanUseSpell()) return;

            this.SpellAvailableAt = Mathf.Infinity;
            for (int i = 0; i < 15; i++) {
                this.InSeconds(i / 3f,
                    () => {
                        AMB_Spell spell = this.UseSpell(this.Meteor, this.AI.PlayerPosition);

                        if (spell == null) return;

                        float cooldown = spell.Cooldown;

                        this.PlayHopAnimation();
                        this.SpellCastAt = Time.time;
                        this.SpellAvailableAt = this.SpellCastAt + cooldown;
                    });
            }
        }
    }
}
