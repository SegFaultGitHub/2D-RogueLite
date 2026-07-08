#if UNITY_EDITOR
using System.Linq;
using MyBox.EditorTools;
using UnityEditor;

namespace MyBox.Internal {
    public class AssetPresetPreprocessor : AssetPostprocessor {
        private void OnPreprocessAsset() {
            this.InitializePreprocessBase();
            if (AssetsPresetPreprocessBase.Instance == null) return;

            foreach (var preset in AssetsPresetPreprocessBase.Instance.Presets) {
                if (preset.Preset == null) continue;
                if (!preset.Sample(this.assetPath)) continue;
                if (!preset.Preset.CanBeAppliedTo(this.assetImporter)) continue;

                this.context.DependsOnSourceAsset(this.assetPath);
                preset.Preset.ApplyTo(this.assetImporter, preset.PropertiesToApply);
                return;
            }
        }

        private void InitializePreprocessBase() {
            if (_preprocessBaseChecked) return;
            _preprocessBaseChecked = true;
            if (AssetsPresetPreprocessBase.Instance != null) return;

            AssetsPresetPreprocessBase.Instance = MyScriptableObject.LoadAssetsFromResources<AssetsPresetPreprocessBase>().FirstOrDefault();

            if (AssetsPresetPreprocessBase.Instance != null) return;

            AssetsPresetPreprocessBase.Instance = MyScriptableObject.LoadAssets<AssetsPresetPreprocessBase>().SingleOrDefault();
        }

        private static bool _preprocessBaseChecked;
    }
}
#endif
