using System;
using System.Collections.Generic;
using System.IO;
using Code.Serializer;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Managers {
    public class MB_ScreenshotManager : MonoBehaviour {
        #region Members
        [Foldout("MB_ScreenshotManager", true)]
        [SerializeField] private protected Camera m_ScreenshotCamera;
        [SerializeField] private protected List<Canvas> m_Canvases;

        [SerializeField] private protected RenderTexture m_ScreenshotRenderTexture;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private Camera ScreenshotCamera { get => this.m_ScreenshotCamera; }
        private List<Canvas> Canvases { get => this.m_Canvases; }

        private RenderTexture ScreenshotRenderTexture { get => this.m_ScreenshotRenderTexture; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void Initialize() { }

        public void PostInitialize() { }

        [ButtonMethod]
        public Texture2D Screenshot() {
            this.ScreenshotCamera.enabled = true;
            this.Resize();

            string epoch = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            // Directory.CreateDirectory(SC_Serializer.GetPersistentPath("screenshots"));
            // ScreenCapture.CaptureScreenshot(SC_Serializer.GetPersistentPath($"screenshots/screenshot-{epoch}.png"));

            Dictionary<Canvas, Camera> canvasCameras = new();

            foreach (Canvas canvas in this.Canvases) {
                canvasCameras.TryAdd(canvas, canvas.worldCamera);
                canvas.worldCamera = this.ScreenshotCamera;
            }

            this.ScreenshotCamera.Render();
            RenderTexture.active = this.ScreenshotCamera.targetTexture;

            Texture2D screenshot = new(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();

            foreach (Canvas canvas in this.Canvases) {
                canvas.worldCamera = canvasCameras[canvas];
            }

            this.ScreenshotCamera.enabled = false;
            return screenshot;
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

        private void Resize() {
            this.ScreenshotCamera.orthographicSize = this.ObjectsManager.MainCamera.Camera.orthographicSize;
            this.ScreenshotRenderTexture.Release();
            this.ScreenshotRenderTexture.width = Screen.width;
            this.ScreenshotRenderTexture.height = Screen.height;
            this.ScreenshotRenderTexture.Create();
            this.ScreenshotCamera.ResetAspect();
        }
    }
}
