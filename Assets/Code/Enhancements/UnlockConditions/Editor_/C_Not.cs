using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace Code.Enhancements.UnlockConditions.Editor_ {
    [Serializable]
    public class C_Not : C_Condition {
        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddInputPort<C_Condition>(C_UnlockConditionsGraph.EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort<C_Condition>(C_UnlockConditionsGraph.EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }

        public override Runtime.C_Condition TranslateNode() {
            Runtime.C_Not not = new();

            List<IPort> outPorts = new();
            this.GetOutputPortByName(C_UnlockConditionsGraph.EXECUTION_PORT_DEFAULT_NAME).GetConnectedPorts(outPorts);
            C_Condition condition = outPorts[0].GetNode() as C_Condition;
            not.Not = condition!.TranslateNode();

            return not;
        }

        public override Runtime.C_Condition TranslateNodeGlobal() {
            Runtime.C_Not not = new();

            List<IPort> outPorts = new();
            this.GetOutputPortByName(C_UnlockConditionsGraph.EXECUTION_PORT_DEFAULT_NAME).GetConnectedPorts(outPorts);
            C_Condition condition = outPorts[0].GetNode() as C_Condition;
            not.Not = condition!.TranslateNodeGlobal();

            return not;
        }
    }
}
