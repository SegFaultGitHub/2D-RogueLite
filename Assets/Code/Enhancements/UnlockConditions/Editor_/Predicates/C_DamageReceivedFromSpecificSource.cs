using System;
using Code.Characters;

namespace Code.Enhancements.UnlockConditions.Editor_.Predicates {
    [Serializable]
    public class C_DamageReceivedFromSpecificSource : C_DamageReceived {
        private const string IN_PORT_SOURCE = "Source";

        protected override string GetHeader() => "Has received X or more damage from source";

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            base.OnDefinePorts(context);

            context.AddInputPort<E_DamageSource>(IN_PORT_SOURCE)
                .WithDisplayName("Source")
                .Build();
        }

        public override Runtime.C_Condition TranslateNode() {
            return new Runtime.Predicates.C_DamageReceivedFromSpecificSource(
                GetInputPortValue<E_Mode>(this.GetInputPortByName(IN_PORT_MODE)),
                GetInputPortValue<int>(this.GetInputPortByName(IN_PORT_DAMAGE)),
                GetInputPortValue<E_DamageSource>(this.GetInputPortByName(IN_PORT_SOURCE))
            );
        }
    }
}
