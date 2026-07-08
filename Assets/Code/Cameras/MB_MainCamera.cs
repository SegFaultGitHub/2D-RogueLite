using System;
using Code.Managers;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Code.Cameras {
    public class MB_MainCamera : MonoBehaviour {
        #region Members
        [Foldout("MB_MainCamera", true)]
        [SerializeField] private protected Camera m_Camera;
        [SerializeField] private protected Camera m_UICamera;

        [SerializeField] private protected float m_MaxAimDistance;

        [SerializeField] private protected VolumeProfile m_Volume;
        [ReadOnly][SerializeField] private protected Vignette m_Vignette;
        [SerializeField] private protected Color m_DamageColor;
        [SerializeField] private protected Color m_HealColor;
        [ReadOnly][SerializeField] private protected Color m_NeutralColor;
        [SerializeField] private protected float m_DamageIntensity;
        [SerializeField] private protected float m_HealIntensity;
        [ReadOnly][SerializeField] private protected float m_NeutralIntensity;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;

        [ReadOnly][SerializeField] private protected Vector2 m_Position;
        [ReadOnly][SerializeField] private protected Vector2 m_PositionOffset;

        [ReadOnly][SerializeField] private protected float m_PixelSize;
        [ReadOnly][SerializeField] private protected Vector3 m_ShakeOffset;
        #endregion

        #region Getters / Setters
        public Camera Camera { get => this.m_Camera; private set => this.m_Camera = value; }
        public Camera UICamera { get => this.m_UICamera; private set => this.m_UICamera = value; }

        private float MaxAimDistance { get => this.m_MaxAimDistance; }

        private VolumeProfile Volume { get => this.m_Volume; }
        private Vignette Vignette { get => this.m_Vignette; set => this.m_Vignette = value; }
        private Color DamageColor { get => this.m_DamageColor; }
        private Color HealColor { get => this.m_HealColor; }
        private Color NeutralColor { get => this.m_NeutralColor; set => this.m_NeutralColor = value; }
        private float DamageIntensity { get => this.m_DamageIntensity; }
        private float HealIntensity { get => this.m_HealIntensity; }
        private float NeutralIntensity { get => this.m_NeutralIntensity; set => this.m_NeutralIntensity = value; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }

        private Vector2 Position { get => this.m_Position; set => this.m_Position = value; }
        private Vector2 PositionOffset { get => this.m_PositionOffset; set => this.m_PositionOffset = value; }

        public float PixelSize { get => this.m_PixelSize; private set => this.m_PixelSize = value; }
        private Vector3 ShakeOffset { get => this.m_ShakeOffset; set => this.m_ShakeOffset = value; }

        private Sequence ShakeSequence { get; set; }
        private Sequence ColorSequence { get; set; }
        private Sequence PixelSizeSequence { get; set; }
        private string ColorGuid { get; set; }
        #endregion

        #region Static / Readonly / Const
        private const float POSITION_ROUND = 0.0625f;
        private const float SIZE_RATIO = 67.5f;
        private const float MAX_AIM_DISTANCE_RATIO = 8f;
        private const float SHAKE_POWER = .25f;
        private const float DAMAGE_DURATION = 1f;
        #endregion

        #region Unity methods
        private void OnApplicationQuit() {
            this.Vignette.intensity.Override(this.NeutralIntensity);
            this.Vignette.color.Override(this.NeutralColor);
        }

        private void Update() {
            this.Camera.orthographicSize = SIZE_RATIO / this.PixelSize;
            this.UICamera.orthographicSize = SIZE_RATIO / this.PixelSize;
            Vector2 characterPosition = this.ObjectsManager.Player.transform.position.ToVector2();

            if (this.ObjectsManager.Player.PlayerController.Active) {
                Vector2 direction = this.ObjectsManager.Player.PlayerController.AimPosition - characterPosition;
                float maxDistance = this.MaxAimDistance * MAX_AIM_DISTANCE_RATIO / this.PixelSize;

                if (direction.magnitude > maxDistance) direction = direction.normalized * maxDistance;

                this.PositionOffset = Vector2.Lerp(this.PositionOffset, direction, 1.2f * Time.deltaTime);
                this.Position = Vector2.Lerp(
                    this.Position,
                    new Vector2(characterPosition.x + this.PositionOffset.x, characterPosition.y + this.PositionOffset.y),
                    5f * Time.deltaTime
                );
                this.transform.position = new Vector3(
                    this.Position.x + this.ShakeOffset.x,
                    this.Position.y + this.ShakeOffset.y,
                    this.transform.position.z
                );
            } else {
                this.Position = characterPosition;
                this.PositionOffset = Vector2.zero;
                this.transform.position = new Vector3(characterPosition.x, characterPosition.y, this.transform.position.z);
            }
        }
        #endregion

        public void Initialize() {
            this.Volume.TryGet(out Vignette vignette);
            this.Vignette = vignette;
            this.NeutralIntensity = vignette.intensity.GetValue<float>();
            this.NeutralColor = vignette.color.GetValue<Color>();
            this.Vignette.intensity.Override(this.NeutralIntensity);
            this.Vignette.color.Override(this.NeutralColor);
        }

        public void PostInitialize() { }

        public void Aim(Vector2 characterPosition, Vector2 direction) { }

        public static float GetScreenRatio() => (float)Screen.width / Screen.height;

        public Vector2 GetScreenSize() {
            float y = this.Camera.orthographicSize * 2;
            float screenRatio = GetScreenRatio();
            float x = y * screenRatio;

            return new Vector2(x, y);
        }

        public float DEBUG_PixelSize;
        public float DEBUG_Duration;
        public Ease DEBUG_Ease;

        [ButtonMethod]
        public void SetPixelSize() => this.SetPixelSize((int)this.DEBUG_PixelSize, this.DEBUG_Duration, this.DEBUG_Ease);

        public void SetPixelSize(int pixelSize, float duration, Ease ease) {
            DOTween.Kill(this.PixelSizeSequence);
            this.PixelSizeSequence = DOTween.Sequence();
            this.PixelSizeSequence.Append(
                DOTween.To( //
                        () => this.PixelSize,
                        size => this.PixelSize = size,
                        pixelSize,
                        duration
                    )
                    .SetEase(ease)
            );
        }

        public void Shake(float duration, float power = 1) {
            DOTween.Kill(this.ShakeSequence);
            this.ShakeOffset = Vector3.zero;

            Vector3 shake = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            this.ShakeSequence = DOTween.Sequence();
            this.ShakeSequence.Append(
                    DOTween.To( //
                        () => Vector3.zero,
                        vector => this.ShakeOffset = vector,
                        shake.normalized * SHAKE_POWER * power,
                        duration
                    )
                )
                .Append(
                    DOTween.To( //
                        () => this.ShakeOffset,
                        vector => this.ShakeOffset = vector,
                        Vector3.zero,
                        duration
                    )
                );
        }

        [ButtonMethod]
        public void Damage() => this.FlashColor(this.DamageColor, this.DamageIntensity);

        [ButtonMethod]
        public void Heal() => this.FlashColor(this.HealColor, this.HealIntensity);

        private void FlashColor(Color color, float intensity) {
            if (this.ColorSequence != null) DOTween.Kill(this.ColorSequence);

            string guid = Guid.NewGuid().ToString();
            this.ColorGuid = guid;

            this.Vignette.intensity.Override(intensity);
            this.Vignette.color.Override(color);

            this.ColorSequence = DOTween.Sequence();
            this.ColorSequence.SetUpdate(true)
                .Append(
                    DOTween.To(
                        () => guid == this.ColorGuid
                            ? this.Vignette.color.value
                            : color,
                        c => {
                            if (guid != this.ColorGuid) return;
                            this.Vignette.color.Override(c);
                        },
                        this.NeutralColor,
                        DAMAGE_DURATION
                    )
                )
                .Join(
                    DOTween.To(
                        () => guid == this.ColorGuid
                            ? this.Vignette.intensity.value
                            : intensity,
                        i => {
                            if (guid != this.ColorGuid) return;
                            this.Vignette.intensity.Override(i);
                        },
                        this.NeutralIntensity,
                        DAMAGE_DURATION
                    )
                );
        }

        public Vector2 GetShaderPosition(Vector2 worldPosition, bool round = true) {
            Vector2 screenPosition = round
                ? this.ObjectsManager.MainCamera.Camera.WorldToScreenPoint(SC_Utils.Round(worldPosition, .125f))
                : this.ObjectsManager.MainCamera.Camera.WorldToScreenPoint(worldPosition);
            return new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);
        }
    }
}
