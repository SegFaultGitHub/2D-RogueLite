#if UNITY_6000_0_OR_NEWER

using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

namespace Unified.UniversalBlur.Runtime.PassData
{
    public class RenderGraphPassData : IPassData
    {
        public TextureHandle ColorSource;
        public TextureHandle Source;
        public TextureHandle Destination;

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
#endif