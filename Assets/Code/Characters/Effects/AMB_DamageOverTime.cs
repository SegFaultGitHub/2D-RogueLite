using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Characters.Effects {
    public abstract class AMB_DamageOverTime : AMB_Effect {
        #region Members
        [Foldout("AMB_DamageOverTime", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected float m_DamagePerTick;
        [ReadOnly][SerializeField] private protected int m_TickCount;
        #endregion

        #region Getters / Setters
        private float DamagePerTick { get => this.m_DamagePerTick; set => this.m_DamagePerTick = value; }
        private int TickCount { get => this.m_TickCount; set => this.m_TickCount = value; }

        public abstract float TickInterval { get; }
        protected abstract E_DamageSource DamageSource { get; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        private void Start() => this.Until(() => this.Character != null, this.DealDamage);
        #endregion

        public void SetDamage(float damagePerTick, int tickCount) {
            this.DamagePerTick = damagePerTick;
            this.TickCount = tickCount;
        }

        private void DealDamage() {
            for (int i = 0; i < this.TickCount; i++) {
                this.InSeconds(
                    (i + 1) * this.TickInterval,
                    () => {
                        (float, bool) computedDamage = this.From.ComputeDamage(this.Character, this.DamagePerTick, this.DamageSource);
                        this.Character.TakeDamage(
                            becomeInvulnerable: false,
                            freeze: false,
                            value: computedDamage.Item1,
                            critical: computedDamage.Item2,
                            from: this.From,
                            source: this.DamageSource
                        );
                    }
                );
            }

            this.InSeconds((this.TickCount + 1) * this.TickInterval, () => this.Character.RemoveEffect(this));
        }
    }
}
