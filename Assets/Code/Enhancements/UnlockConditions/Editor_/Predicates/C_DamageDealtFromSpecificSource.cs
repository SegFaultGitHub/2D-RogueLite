using System;
using Code.Characters;

namespace Code.Enhancements.UnlockConditions.Editor_.Predicates {
    [Serializable]
    public class C_DamageDealtFromSpecificSource : C_DamageDealt {
        protected const string IN_PORT_SOURCE = "Source";

        protected override string GetHeader() => "Has dealt X or more damage from source";

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            base.OnDefinePorts(context);

            context.AddInputPort<E_DamageSource>(IN_PORT_SOURCE).WithDisplayName("Source").Build();
        }

        public override Runtime.C_Condition TranslateNode() {
            return new Runtime.Predicates.C_DamageDealtFromSpecificSource(
                GetInputPortValue<E_Mode>(this.GetInputPortByName(IN_PORT_MODE)),
                GetInputPortValue<float>(this.GetInputPortByName(IN_PORT_DAMAGE)),
                GetInputPortValue<E_DamageSource>(this.GetInputPortByName(IN_PORT_SOURCE))
            );
        }
    }
}
