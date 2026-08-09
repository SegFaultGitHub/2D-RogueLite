using System;
using Code.Enhancements.UnlockConditions.Runtime.Predicates;
using Unity.GraphToolkit.Editor;

namespace Code.Enhancements.UnlockConditions.Editor_ {
    [Serializable]
    public abstract class C_Predicate : C_Condition {
        protected const string IN_PORT_MODE = "Mode";

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddInputPort<C_Condition>(C_UnlockConditionsGraph.EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName($"<b>{this.GetHeader()}</b>")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddInputPort<E_Mode>(IN_PORT_MODE)
                .WithDisplayName("Mode")
                .Build();
        }

        protected virtual string GetHeader() { return string.Empty; }

        public E_Mode GetMode() => GetInputPortValue<E_Mode>(this.GetInputPortByName(IN_PORT_MODE));

        public override Runtime.C_Condition TranslateNodeGlobal() {
            return this.GetMode() == E_Mode.CurrentRun
                ? new C_Bool(E_Mode.Global, true)
                : this.TranslateNode();
        }
    }
}
