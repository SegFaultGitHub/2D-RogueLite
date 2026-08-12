using System;
using Code.Enhancements.UnlockConditions.Editor_;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Runtime {
    [Serializable]
    public class C_Root {
        [field: SerializeReference] public C_Condition Condition { get; set; }
        [field: SerializeReference] public C_Condition GlobalCondition { get; set; }

        public C_Root(Enhancements.UnlockConditions.Editor_.C_Root root) {
            INode node = root.GetOutputPortByName(C_UnlockConditionsGraph.EXECUTION_PORT_DEFAULT_NAME).FirstConnectedPort.GetNode();

            this.Condition = (node as Enhancements.UnlockConditions.Editor_.C_Condition)!.TranslateNode();
            this.GlobalCondition = (node as Enhancements.UnlockConditions.Editor_.C_Condition)!.TranslateNodeGlobal();
        }
    }
}
