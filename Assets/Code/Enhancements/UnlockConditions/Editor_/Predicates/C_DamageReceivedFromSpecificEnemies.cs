using System;
using Code.Characters.Enemies;

namespace Code.Enhancements.UnlockConditions.Editor_.Predicates {
    [Serializable]
    public class C_DamageReceivedFromSpecificEnemies : C_DamageReceived {
        protected const string IN_PORT_ENEMY = "Enemy";

        protected override string GetHeader() => "Has received X or more damage from enemy type";

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            base.OnDefinePorts(context);

            context.AddInputPort<E_Enemy>(IN_PORT_ENEMY)
                .WithDisplayName("Enemy")
                .Build();
        }

        public override Runtime.C_Condition TranslateNode() {
            return new Runtime.Predicates.C_DamageReceivedFromSpecificEnemies(
                GetInputPortValue<E_Mode>(this.GetInputPortByName(IN_PORT_MODE)),
                GetInputPortValue<int>(this.GetInputPortByName(IN_PORT_DAMAGE)),
                GetInputPortValue<E_Enemy>(this.GetInputPortByName(IN_PORT_ENEMY))
            );
        }
    }
}
