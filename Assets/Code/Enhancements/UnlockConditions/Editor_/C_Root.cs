using System;
using Unity.GraphToolkit.Editor;

namespace Code.Enhancements.UnlockConditions.Editor_ {
    [Serializable]
    public class C_Root : Node {
        #region Members
        #endregion

        #region Getters / Setters
        #endregion

        #region Static / Readonly / Const
        #endregion

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddOutputPort<C_Condition>(C_UnlockConditionsGraph.EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
