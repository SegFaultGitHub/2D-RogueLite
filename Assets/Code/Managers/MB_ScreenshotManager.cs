using System;
using System.Collections.Generic;
using System.IO;
using Code.Serializer;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.RenderGraphModule;

namespace Code.Managers {
    public class MB_ScreenshotManager : MonoBehaviour {
        #region Members
        [Foldout("MB_ScreenshotManager", true)]
        [SerializeField] private protected Camera m_ScreenshotCamera;
        [SerializeField] private protected Camera m_UIScreenshotCamera;
        [SerializeField] private protected List<Canvas> m_Canvases;
        [SerializeField] private protected Canvas m_UIScreenshotCanvas;

        [SerializeField] private protected RenderTexture m_ScreenshotRenderTexture;
        [SerializeField] private protected RenderTexture m_UIScreenshotRenderTexture;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private Camera ScreenshotCamera { get => this.m_ScreenshotCamera; }
        private Camera UIScreenshotCamera { get => this.m_UIScreenshotCamera; }
        private List<Canvas> Canvases { get => this.m_Canvases; }
        private Canvas UIScreenshotCanvas { get => this.m_UIScreenshotCanvas; }

        private RenderTexture ScreenshotRenderTexture { get => this.m_ScreenshotRenderTexture; }
        private RenderTexture UIScreenshotRenderTexture { get => this.m_UIScreenshotRenderTexture; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void Initialize() { }

        public void PostInitialize() { }

        [ButtonMethod]
        public void Screenshot() {
            this.ScreenshotCamera.enabled = true;
            this.Resize(this.ScreenshotCamera, this.ScreenshotRenderTexture);

            string epoch = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            // Directory.CreateDirectory(SC_Serializer.GetPersistentPath("screenshots"));
            // ScreenCapture.CaptureScreenshot(SC_Serializer.GetPersistentPath($"screenshots/screenshot-{epoch}.png"));

            Dictionary<Canvas, Camera> canvasCameras = new();

            foreach (Canvas canvas in this.Canvases) {
                canvasCameras.TryAdd(canvas, canvas.worldCamera);
                canvas.worldCamera = this.ScreenshotCamera;
            }

            this.ScreenshotCamera.Render();
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = this.ScreenshotCamera.targetTexture;

            Texture2D screenshot = new(Screen.width, Screen.height, TextureFormat.RGBA32, false, false);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
            screenshot.Apply();
            //ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);

            foreach (Canvas canvas in this.Canvases) {
                canvas.worldCamera = canvasCameras[canvas];
            }

            this.ScreenshotCamera.enabled = false;
            RenderTexture.active = previous;
        }

        public List<Transform> TEMP_gos;

        [ButtonMethod]
        public RenderTexture ScreenshotUIComponents() => this.ScreenshotUIComponents(this.TEMP_gos);

        public RenderTexture ScreenshotUIComponents(ICollection<Transform> uiComponents) {
            RenderTexture renderTexture = new(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            this.UIScreenshotCamera.targetTexture = renderTexture;
            this.UIScreenshotCamera.enabled = true;
            this.Resize(this.UIScreenshotCamera, renderTexture);

            Dictionary<Transform, (Transform, int)> componentParents = new();
            Dictionary<Transform, bool> componentStates = new();

            foreach (Transform uiComponent in uiComponents) {
                componentParents.TryAdd(uiComponent, (uiComponent.parent, uiComponent.GetSiblingIndex()));
                componentStates.TryAdd(uiComponent, uiComponent.gameObject.activeSelf);
                uiComponent.SetParent(this.UIScreenshotCanvas.transform);
                uiComponent.gameObject.SetActive(true);
            }

            this.UIScreenshotCamera.Render();
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;

            // Texture2D screenshot = new(Screen.width, Screen.height, TextureFormat.RGBA32, false, false);
            // screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
            // screenshot.Apply(false, false);

            foreach (Transform uiComponent in uiComponents) {
                uiComponent.SetParent(componentParents[uiComponent].Item1);
                uiComponent.SetSiblingIndex(componentParents[uiComponent].Item2);
                uiComponent.gameObject.SetActive(false);
                uiComponent.gameObject.SetActive(componentStates[uiComponent]);
            }

            this.UIScreenshotCamera.enabled = false;
            RenderTexture.active = previous;
            return renderTexture;
        }

        #region Input
        private PlayerInputs PlayerInputs { get; set; }

        protected void OnEnable() {
            this.PlayerInputs = new PlayerInputs();
            this.PlayerInputs.Actions.Enable();

            this.PlayerInputs.Actions.Screenshot.performed += this.ScreenshotInput;
        }

        protected void OnDisable() {
            this.PlayerInputs.Actions.Screenshot.performed -= this.ScreenshotInput;

            this.PlayerInputs.Actions.Disable();
        }

        private void ScreenshotInput(InputAction.CallbackContext _) => this.Screenshot();
        #endregion

        private void Resize(Camera renderCamera, RenderTexture renderTexture) {
            renderCamera.orthographicSize = this.ObjectsManager.MainCamera.Camera.orthographicSize;
            renderTexture.Release();
            Debug.Log(Screen.width + " x " + Screen.height);
            renderTexture.width = Screen.width;
            renderTexture.height = Screen.height;
            renderTexture.Create();
            renderCamera.ResetAspect();
        }
    }
}
