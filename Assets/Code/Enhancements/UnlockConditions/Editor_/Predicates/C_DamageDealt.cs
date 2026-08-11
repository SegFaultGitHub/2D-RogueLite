using System;

namespace Code.Enhancements.UnlockConditions.Editor_.Predicates {
    [Serializable]
    public class C_DamageDealt : C_Predicate {
        protected const string IN_PORT_DAMAGE = "Damage";

        protected override string GetHeader() => "Has dealt X or more damage";

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(IN_PORT_DAMAGE)
                .WithDisplayName("Damage")
                .Build();
        }

        public override Runtime.C_Condition TranslateNode() {
            return new Runtime.Predicates.C_DamageDealt(
                GetInputPortValue<E_Mode>(this.GetInputPortByName(IN_PORT_MODE)),
                GetInputPortValue<int>(this.GetInputPortByName(IN_PORT_DAMAGE))
            );
        }
    }
}
