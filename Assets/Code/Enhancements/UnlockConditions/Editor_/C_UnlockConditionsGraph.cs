using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace Code.Enhancements.UnlockConditions.Editor_ {
    [Serializable]
    [Graph(ASSET_EXTENSION)]
    internal class C_UnlockConditionsGraph : Graph {
        public const string EXECUTION_PORT_DEFAULT_NAME = "ExecutionPort";
        internal const string ASSET_EXTENSION = "eucg"; // EnhancementUnlockConditionsGraph

        [MenuItem("Assets/Create/Enhancements/Unlock Conditions", false)]
        private static void CreateAssetFile() {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<C_UnlockConditionsGraph>();
        }

        public override void OnGraphChanged(GraphLogger infos) {
            base.OnGraphChanged(infos);
            this.CheckGraphErrors(infos);
        }

        private void CheckGraphErrors(GraphLogger infos) {
            List<C_Root> rootNodes = this.GetNodes().OfType<C_Root>().ToList();
            switch (rootNodes.Count) {
                case 0:
                    infos.LogError("Unlock Conditions is missing a Root Node.", this);
                    break;
                case 1:
                    List<IPort> outPorts = new();
                    rootNodes[0].GetOutputPortByName(EXECUTION_PORT_DEFAULT_NAME).GetConnectedPorts(outPorts);
                    if (outPorts.Count != 1) {
                        infos.LogError("Root Node should have exactly one output.", rootNodes[0]);
                    }

                    break;
                case > 1:
                    foreach (C_Root startNode in rootNodes.Skip(1)) {
                        infos.LogError(
                            "Unlock Conditions only supports one Root Node. Only the first created one will be used.",
                            startNode
                        );
                    }

                    break;
            }

            List<C_And> andNodes = this.GetNodes().OfType<C_And>().ToList();
            foreach (C_And andNode in andNodes) {
                List<IPort> outPorts = new();
                andNode.GetOutputPortByName(EXECUTION_PORT_DEFAULT_NAME).GetConnectedPorts(outPorts);
                if (outPorts.Count < 2) {
                    infos.LogWarning("And Node should have at least 2 sub-conditions.", andNode);
                }

                for (int i = 0; i < outPorts.Count; i++) {
                    infos.Log(i, outPorts[i]);
                }
            }

            List<C_Or> orNodes = this.GetNodes().OfType<C_Or>().ToList();
            foreach (C_Or orNode in orNodes) {
                List<IPort> outPorts = new();
                orNode.GetOutputPortByName(EXECUTION_PORT_DEFAULT_NAME).GetConnectedPorts(outPorts);
                if (outPorts.Count < 2) {
                    infos.LogWarning("Or Node should have at least 2 sub-conditions.", orNode);
                }

                for (int i = 0; i < outPorts.Count; i++) {
                    infos.Log(i, outPorts[i]);
                }
            }

            List<C_Not> notNodes = this.GetNodes().OfType<C_Not>().ToList();
            foreach (C_Not notNode in notNodes) {
                List<IPort> outPorts = new();
                notNode.GetOutputPortByName(EXECUTION_PORT_DEFAULT_NAME).GetConnectedPorts(outPorts);
                if (outPorts.Count != 1) {
                    infos.LogError("Not Node must have exactly 1 sub-condition.", notNode);
                }
            }
        }
    }
}
