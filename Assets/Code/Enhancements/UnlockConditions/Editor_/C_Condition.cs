using System;
using Unity.GraphToolkit.Editor;

namespace Code.Enhancements.UnlockConditions.Editor_ {
    [Serializable]
    public abstract class C_Condition : Node {
        public abstract Runtime.C_Condition TranslateNode();

        public static T GetInputPortValue<T>(IPort port)
        {
            T value = default;

            // If port is connected to another node, get value from connection
            if (port.isConnected)
            {
                switch (port.firstConnectedPort.GetNode())
                {
                    case IVariableNode variableNode:
                        variableNode.variable.TryGetDefaultValue(out value);
                        return value;
                    case IConstantNode constantNode:
                        constantNode.TryGetValue(out value);
                        return value;
                }
            }
            else
            {
                port.TryGetValue(out value);
            }

            return value;
        }
    }
}
