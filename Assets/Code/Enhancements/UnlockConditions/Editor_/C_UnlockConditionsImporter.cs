using System.Linq;
using Code.Enhancements.UnlockConditions.Runtime;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Code.Enhancements.UnlockConditions.Editor_ {
    [ScriptedImporter(1, C_UnlockConditionsGraph.ASSET_EXTENSION)]
    internal class C_UnlockConditionsImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            C_UnlockConditionsGraph graph = GraphDatabase.LoadGraphForImporter<C_UnlockConditionsGraph>(ctx.assetPath);
            if (graph == null) {
                Debug.LogError($"Failed to load Unlock Conditions graph asset: {ctx.assetPath}");
                return;
            }

            C_Root root = graph.GetNodes().OfType<C_Root>().FirstOrDefault();
            if (root == null) {
                return;
            }

            C_UnlockConditionsRuntimeGraph runtimeAsset = ScriptableObject.CreateInstance<C_UnlockConditionsRuntimeGraph>();
            runtimeAsset.Root = new Runtime.C_Root(root);
            ctx.AddObjectToAsset("RuntimeAsset", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
        }
    }
}
