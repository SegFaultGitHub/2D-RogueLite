using UnityEngine;

namespace Unified.UniversalBlur.Runtime.PassData
{
    public struct LegacyPassData : IPassData
    {
        public Texture ColorSource;
        public Texture Source;
        public Texture Destination;

        public MaterialPropertyBlock MaterialPropertyBlock;

        public BlurConfig BlurConfig;

        public Texture GetColorSource()
        {
            return this.ColorSource;
        }

        public Texture GetSource()
        {
            return this.Source;
        }

        public Texture GetDestination()
        {
            return this.Destination;
        }

        public MaterialPropertyBlock GetMaterialPropertyBlock()
        {
            return this.MaterialPropertyBlock;
        }

        public BlurConfig GetBlurConfig()
        {
            return this.BlurConfig;
        }
    }
}
