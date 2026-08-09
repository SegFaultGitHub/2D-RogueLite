using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace Code.Enhancements.UnlockConditions.Editor_ {
    [Serializable]
    public class C_Or : C_Condition {
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
            Runtime.C_Or or = new();

            List<IPort> outPorts = new();
            this.GetOutputPortByName(C_UnlockConditionsGraph.EXECUTION_PORT_DEFAULT_NAME).GetConnectedPorts(outPorts);
            foreach (IPort port in outPorts) {
                C_Condition condition = port.GetNode() as C_Condition;
                or.Or.Add(condition!.TranslateNode());
            }

            return or;
        }

        public override Runtime.C_Condition TranslateNodeGlobal() {
            Runtime.C_Or or = new();

            List<IPort> outPorts = new();
            this.GetOutputPortByName(C_UnlockConditionsGraph.EXECUTION_PORT_DEFAULT_NAME).GetConnectedPorts(outPorts);
            foreach (IPort port in outPorts) {
                C_Condition condition = port.GetNode() as C_Condition;
                if (condition is C_Predicate predicate && predicate.GetMode() == E_Mode.CurrentRun) {
                    continue;
                }
                or.Or.Add(condition!.TranslateNodeGlobal());
            }

            return or;
        }
    }
}
