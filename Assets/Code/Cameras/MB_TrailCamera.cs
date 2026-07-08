using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Cameras {
    public class MB_TrailCamera : MonoBehaviour {
        #region Members
        [Foldout("MB_TrailCamera", true)]
        [SerializeField] private protected Camera m_MainCamera;
        [SerializeField] private protected RawImage m_TrailImage;

        [SerializeField] private protected RenderTexture m_TrailRenderTexture;
        
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Camera m_Camera;
        [ReadOnly][SerializeField] private protected Material m_TrailMaterial;

        [ReadOnly][SerializeField] private protected int m_PreviousScreenWidth;
        [ReadOnly][SerializeField] private protected int m_PreviousScreenHeight;
        #endregion

        #region Getters / Setters
        private Camera MainCamera { get => this.m_MainCamera; }
        private RawImage TrailImage { get => this.m_TrailImage; }

        private RenderTexture TrailRenderTexture { get => this.m_TrailRenderTexture; }

        private Camera Camera { get => this.m_Camera; set => this.m_Camera = value; }
        private Material TrailMaterial { get => this.m_TrailMaterial; set => this.m_TrailMaterial = value; }

        private int PreviousScreenWidth { get => this.m_PreviousScreenWidth; set => this.m_PreviousScreenWidth = value; }
        private int PreviousScreenHeight { get => this.m_PreviousScreenHeight; set => this.m_PreviousScreenHeight = value; }
        #endregion

        #region Static / Readonly / Const
        private static readonly int ORTHOGRAPHIC_SIZE = Shader.PropertyToID("_OrthographicSize");
        #endregion

        #region Unity methods
        private void Start() {
            this.Camera = this.GetComponent<Camera>();
            this.TrailMaterial = this.TrailImage.material;
            this.ResizeRenderTexture();
            this.PreviousScreenWidth = Screen.width;
            this.PreviousScreenHeight = Screen.height;
        }

        private void FixedUpdate() {
            this.Camera.orthographicSize = this.MainCamera.orthographicSize;
            this.TrailMaterial.SetFloat(ORTHOGRAPHIC_SIZE, this.MainCamera.orthographicSize);

            if (this.PreviousScreenWidth != Screen.width || this.PreviousScreenHeight != Screen.height)
                this.ResizeRenderTexture();

            this.PreviousScreenWidth = Screen.width;
            this.PreviousScreenHeight = Screen.height;
        }
        #endregion

        private void ResizeRenderTexture() {
            this.TrailRenderTexture.Release();
            this.TrailRenderTexture.width = Screen.width;
            this.TrailRenderTexture.height = Screen.height;
            this.TrailRenderTexture.Create();
            this.Camera.ResetAspect();
        }
    }
}
