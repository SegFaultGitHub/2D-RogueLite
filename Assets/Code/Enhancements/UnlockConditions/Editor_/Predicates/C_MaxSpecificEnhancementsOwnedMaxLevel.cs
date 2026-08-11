using System;

namespace Code.Enhancements.UnlockConditions.Editor_.Predicates {
    [Serializable]
    public class C_MaxSpecificEnhancementsOwnedMaxLevel : C_MaxEnhancementsOwned {
        private const string IN_PORT_ENHANCEMENT = "Enhancement";

        protected override string GetHeader() => "Owns less than X of the specific enhancement maxed out";

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            base.OnDefinePorts(context);

            context.AddInputPort<E_Enhancement>(IN_PORT_ENHANCEMENT)
                .WithDisplayName("Enhancement")
                .Build();
        }

        public override Runtime.C_Condition TranslateNode() {
            return new Runtime.Predicates.C_MaxSpecificEnhancementsOwnedMaxLevel(
                GetInputPortValue<E_Mode>(this.GetInputPortByName(IN_PORT_MODE)),
                GetInputPortValue<int>(this.GetInputPortByName(IN_PORT_COUNT)),
                GetInputPortValue<E_Enhancement>(this.GetInputPortByName(IN_PORT_ENHANCEMENT))
            );
        }
    }
}
