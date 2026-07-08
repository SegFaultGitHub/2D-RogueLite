using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Unified.UniversalBlur.Runtime
{
    public class UniversalBlurFeature : ScriptableRendererFeature
    {
        [Header("Blur Settings")]
        [Range(1, 12)] [SerializeField] private int iterations = 4;
        [Range(1f, 10f)] [SerializeField] private float downsample = 2.0f;
        
        [Tooltip("Enable mipmaps for more efficient blur")]
        [SerializeField] private bool enableMipMaps = true;
        // [Range(0f, 10f)] 
        [SerializeField] private float scale = 1f;
        // [Range(0f, 10f)] 
        [SerializeField] private float offset = 1f;
        
        [Space]
        
        [Header("Advanced Settings")]
        [SerializeField] private ScaleBlurWith scaleBlurWith = ScaleBlurWith.ScreenHeight;
        [SerializeField] private float scaleReferenceSize = 1080f;
        
        [Space]
        
        // [SerializeField, ShowAsPass(nameof(_material))] public int shaderPass;
        [SerializeField] private BlurType blurType;
        
        [Tooltip("For Overlay Canvas: AfterRenderingPostProcessing" +
                 "\n\nOther: BeforeRenderingTransparents (will hide transparents)")]
        [SerializeField] private RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;
        
        private float _intensity = 1.0f;
        
        [SerializeField]
        [HideInInspector]
        [Reload("Shaders/Blur.shader")]
        private Shader shader;
        
        private Material _material;
        private UniversalBlurPass _blurPass;
        private float _renderScale; 
        
        // Avoid changing intensity value, but useful for transitions
        public float Intensity
        {
            get => this._intensity;
            set => this._intensity = Mathf.Clamp(value, 0f, 1f);
        }

        /// <inheritdoc/>
        public override void Create()
        {
            this._blurPass = new UniversalBlurPass();
            this._blurPass.renderPassEvent = this.injectionPoint;
        }

        /// <inheritdoc/>
        public override void OnCameraPreCull(ScriptableRenderer renderer, in CameraData cameraData)
        {
            base.OnCameraPreCull(renderer, in cameraData);
            this._renderScale = cameraData.renderScale;
        }

        /// <inheritdoc/>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!this.TrySetShadersAndMaterials())
            {
                Debug.LogErrorFormat("{0}.AddRenderPasses(): Missing material. {1} render pass will not be added.", this.GetType().Name, this.name);
                return;
            }
            
            // Important to halt rendering here if camera is different, otherwise render textures will detect descriptor changes
            if (renderingData.cameraData.isPreviewCamera ||
                (renderingData.cameraData.isSceneViewCamera))
            {
                this._blurPass.DrawDefaultTexture();
                
                return;
            }
            
            var passData = this.GetBlurConfig(renderingData);

            this._blurPass.Setup(passData);
            
            renderer.EnqueuePass(this._blurPass);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            this._blurPass?.Dispose();
            CoreUtils.Destroy(this._material);
        }
    
        private bool TrySetShadersAndMaterials()
        {
            if (this.shader == null)
                this.shader = Shader.Find("Unify/Internal/Blur");
            
            if (this._material == null && this.shader != null)
                this._material = CoreUtils.CreateEngineMaterial(this.shader);
            
            return this._material != null;
        }
        
        private BlurConfig GetBlurConfig(in RenderingData renderingData)
        {
            var (width, height) = this.GetTargetResolution(renderingData);
            
            return new BlurConfig
            {
                Scale = this.CalculateScale(),
                
                Width = width,
                Height = height,
                
                Material = this._material,
                Intensity = this._intensity,
                Downsample = this.downsample,
                Offset = this.offset,
                BlurType = this.blurType,
                Iterations = this.iterations,
                
                EnableMipMaps = this.enableMipMaps
            };
        }

        private (int width, int height) GetTargetResolution(in RenderingData renderingData)
        {
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            
            var width =
                Mathf.RoundToInt(descriptor.width / this.downsample);
            var height =
                Mathf.RoundToInt(descriptor.height / this.downsample);

            return (width, height);
        }
        
        private float CalculateScale() =>
            this.scaleBlurWith switch
        {
            ScaleBlurWith.ScreenHeight => this.scale * (Screen.height / this.scaleReferenceSize) * this._renderScale,
            ScaleBlurWith.ScreenWidth => this.scale * (Screen.width / this.scaleReferenceSize) * this._renderScale,
            _ => this.scale
        };
    }
}