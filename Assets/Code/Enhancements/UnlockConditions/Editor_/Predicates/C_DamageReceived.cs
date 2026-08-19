using System;

namespace Code.Enhancements.UnlockConditions.Editor_.Predicates {
    [Serializable]
    public class C_DamageReceived : C_Predicate {
        protected const string IN_PORT_DAMAGE = "Damage";

        protected override string GetHeader() => "Has received X or more damage";

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            base.OnDefinePorts(context);

            context.AddInputPort<float>(IN_PORT_DAMAGE)
                .WithDisplayName("Damage")
                .Build();
        }

        public override Runtime.C_Condition TranslateNode() {
            return new Runtime.Predicates.C_DamageReceived(
                GetInputPortValue<E_Mode>(this.GetInputPortByName(IN_PORT_MODE)),
                GetInputPortValue<float>(this.GetInputPortByName(IN_PORT_DAMAGE))
            );
        }
    }
}
