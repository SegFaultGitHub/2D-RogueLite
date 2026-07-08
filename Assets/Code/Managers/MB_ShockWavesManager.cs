using System;
using Code.Cameras;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Managers {
    public class MB_ShockWavesManager : MonoBehaviour {
        [Serializable]
        private protected class C_ShockWave {
            [SerializeField] private protected Vector2 m_WorldPosition;
            [SerializeField] private float m_ExpiresAt;
            [SerializeField] private string m_Guid;

            [SerializeField] private float m_SizeRatio;
            [SerializeField] private float m_MaxSize;
            [SerializeField] private float m_Strength;

            [SerializeField] private bool m_Active;

            public Vector2 WorldPosition { get => this.m_WorldPosition; set => this.m_WorldPosition = value; }
            public float ExpiresAt { get => this.m_ExpiresAt; set => this.m_ExpiresAt = value; }
            public string Guid { get => this.m_Guid; set => this.m_Guid = value; }

            public float SizeRatio { get => this.m_SizeRatio; set => this.m_SizeRatio = value; }
            public float MaxSize { get => this.m_MaxSize; set => this.m_MaxSize = value; }
            public float Strength { get => this.m_Strength; set => this.m_Strength = value; }

            public bool Active { get => this.m_Active; set => this.m_Active = value; }

            public Sequence Sequence { get; set; }

            public float TTL { get => this.ExpiresAt - Time.time; }

            public C_ShockWave() {
                this.Active = false;
            }
        }

        #region Members
        [Foldout("MB_ShockWavesManager", true)]
        [SerializeField] private protected Transform m_ShockWaveTransform;
        [SerializeField] private protected SpriteRenderer m_ShockWave;
        [ReadOnly][SerializeField] private protected Material m_ShockWaveMaterial;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        [ReadOnly][SerializeField] private protected CollectionWrapper<C_ShockWave> m_ShockWaves;
        #endregion

        #region Getters / Setters
        private Transform ShockWaveTransform { get => this.m_ShockWaveTransform; }
        private SpriteRenderer ShockWave { get => this.m_ShockWave; set => this.m_ShockWave = value; }
        private Material ShockWaveMaterial { get => this.m_ShockWaveMaterial; set => this.m_ShockWaveMaterial = value; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        private CollectionWrapper<C_ShockWave> ShockWaves { get => this.m_ShockWaves; set => this.m_ShockWaves = value; }
        #endregion

        #region Static / Readonly / Const
        private static readonly int[] POSITIONS = {
            Shader.PropertyToID("_Position_0"),
            Shader.PropertyToID("_Position_1"),
            Shader.PropertyToID("_Position_2"),
            Shader.PropertyToID("_Position_3"),
            Shader.PropertyToID("_Position_4"),
            Shader.PropertyToID("_Position_5"),
            Shader.PropertyToID("_Position_6"),
            Shader.PropertyToID("_Position_7"),
            Shader.PropertyToID("_Position_8"),
            Shader.PropertyToID("_Position_9"),
            Shader.PropertyToID("_Position_10"),
            Shader.PropertyToID("_Position_11"),
            Shader.PropertyToID("_Position_12"),
            Shader.PropertyToID("_Position_13"),
            Shader.PropertyToID("_Position_14"),
            Shader.PropertyToID("_Position_15")
        };
        private static readonly int[] MAX_SIZES = {
            Shader.PropertyToID("_MaxSize_0"),
            Shader.PropertyToID("_MaxSize_1"),
            Shader.PropertyToID("_MaxSize_2"),
            Shader.PropertyToID("_MaxSize_3"),
            Shader.PropertyToID("_MaxSize_4"),
            Shader.PropertyToID("_MaxSize_5"),
            Shader.PropertyToID("_MaxSize_6"),
            Shader.PropertyToID("_MaxSize_7"),
            Shader.PropertyToID("_MaxSize_8"),
            Shader.PropertyToID("_MaxSize_9"),
            Shader.PropertyToID("_MaxSize_10"),
            Shader.PropertyToID("_MaxSize_11"),
            Shader.PropertyToID("_MaxSize_12"),
            Shader.PropertyToID("_MaxSize_13"),
            Shader.PropertyToID("_MaxSize_14"),
            Shader.PropertyToID("_MaxSize_15")
        };
        private static readonly int[] STRENGTHS = {
            Shader.PropertyToID("_Strength_0"),
            Shader.PropertyToID("_Strength_1"),
            Shader.PropertyToID("_Strength_2"),
            Shader.PropertyToID("_Strength_3"),
            Shader.PropertyToID("_Strength_4"),
            Shader.PropertyToID("_Strength_5"),
            Shader.PropertyToID("_Strength_6"),
            Shader.PropertyToID("_Strength_7"),
            Shader.PropertyToID("_Strength_8"),
            Shader.PropertyToID("_Strength_9"),
            Shader.PropertyToID("_Strength_10"),
            Shader.PropertyToID("_Strength_11"),
            Shader.PropertyToID("_Strength_12"),
            Shader.PropertyToID("_Strength_13"),
            Shader.PropertyToID("_Strength_14"),
            Shader.PropertyToID("_Strength_15")
        };
        private static readonly int[] SIZE_RATIOS = {
            Shader.PropertyToID("_SizeRatio_0"),
            Shader.PropertyToID("_SizeRatio_1"),
            Shader.PropertyToID("_SizeRatio_2"),
            Shader.PropertyToID("_SizeRatio_3"),
            Shader.PropertyToID("_SizeRatio_4"),
            Shader.PropertyToID("_SizeRatio_5"),
            Shader.PropertyToID("_SizeRatio_6"),
            Shader.PropertyToID("_SizeRatio_7"),
            Shader.PropertyToID("_SizeRatio_8"),
            Shader.PropertyToID("_SizeRatio_9"),
            Shader.PropertyToID("_SizeRatio_10"),
            Shader.PropertyToID("_SizeRatio_11"),
            Shader.PropertyToID("_SizeRatio_12"),
            Shader.PropertyToID("_SizeRatio_13"),
            Shader.PropertyToID("_SizeRatio_14"),
            Shader.PropertyToID("_SizeRatio_15")
        };
        private static readonly int SCREEN_RATIO = Shader.PropertyToID("_ScreenRatio");
        private static readonly int ORTHOGRAPHIC_SIZE = Shader.PropertyToID("_OrthographicSize");

        public const Ease EASE = Ease.OutSine;
        #endregion

        #region Unity methods
        private void OnApplicationQuit() {
            for (int i = 0; i < this.ShockWaves.Length; i++) {
                this.ShockWaveMaterial.SetVector(POSITIONS[i], Vector2.zero);
                this.ShockWaveMaterial.SetFloat(MAX_SIZES[i], 0);
                this.ShockWaveMaterial.SetFloat(STRENGTHS[i], 0);
                this.ShockWaveMaterial.SetFloat(SIZE_RATIOS[i], 0);
            }
        }

        private void Awake() {
            this.ShockWaveMaterial = this.ShockWave.material;
            for (int i = 0; i < this.ShockWaves.Length; i++) {
                this.ShockWaveMaterial.SetVector(POSITIONS[i], Vector2.zero);
                this.ShockWaveMaterial.SetFloat(MAX_SIZES[i], 0);
                this.ShockWaveMaterial.SetFloat(STRENGTHS[i], 0);
                this.ShockWaveMaterial.SetFloat(SIZE_RATIOS[i], 0);
            }
        }

        private void FixedUpdate() {
            this.ShockWaveTransform.position = new Vector3(
                this.ObjectsManager.MainCamera.transform.position.x,
                this.ObjectsManager.MainCamera.transform.position.y,
                this.transform.position.z
            );
            float screenRatio = MB_MainCamera.GetScreenRatio();
            float width = this.ObjectsManager.MainCamera.Camera.orthographicSize * 2;
            this.ShockWaveTransform.localScale = new Vector3(screenRatio * width, width, 1);

            this.ShockWaveMaterial.SetFloat(SCREEN_RATIO, screenRatio);
            this.ShockWaveMaterial.SetFloat(ORTHOGRAPHIC_SIZE, this.ObjectsManager.MainCamera.Camera.orthographicSize);

            for (int i = 0; i < this.ShockWaves.Length; i++) {
                if (!this.ShockWaves[i].Active) continue;

                this.ShockWaveMaterial.SetVector(
                    POSITIONS[i],
                    this.ObjectsManager.MainCamera.GetShaderPosition(this.ShockWaves[i].WorldPosition)
                );
                this.ShockWaveMaterial.SetFloat(SIZE_RATIOS[i], this.ShockWaves[i].SizeRatio);
            }
        }
        #endregion

        public void Initialize() {
            this.ShockWaves.Value = new C_ShockWave[16];
            for (int i = 0; i < this.ShockWaves.Length; i++) {
                this.ShockWaves[i] = new C_ShockWave();
            }
        }

        public void PostInitialize() { }

        [ButtonMethod]
        public void PerformShockWave(Vector2 worldPosition, float duration, float strength, float maxSize) {
            int index = 0;
            for (int i = 0; i < this.ShockWaves.Length; i++) {
                if (!this.ShockWaves[i].Active) {
                    index = i;
                    break;
                } else if (this.ShockWaves[i].TTL < this.ShockWaves[index].TTL) {
                    index = i;
                }
            }

            string guid = Guid.NewGuid().ToString();
            const float f = 0;

            if (this.ShockWaves[index].Active) {
                DOTween.Kill(this.ShockWaves[index].Sequence);
            }

            this.ShockWaves[index] = new C_ShockWave {
                WorldPosition = worldPosition,
                ExpiresAt = Time.time + duration,
                Guid = guid,

                SizeRatio = 0,
                MaxSize = maxSize,
                Strength = strength,

                Sequence = DOTween.Sequence(),
                Active = true
            };
            this.ShockWaveMaterial.SetVector(POSITIONS[index], Vector2.zero);
            this.ShockWaveMaterial.SetFloat(MAX_SIZES[index], maxSize);
            this.ShockWaveMaterial.SetFloat(STRENGTHS[index], strength);
            this.ShockWaveMaterial.SetFloat(SIZE_RATIOS[index], 0);

            this.ShockWaves[index]
                .Sequence.Append(
                    DOTween.To( //
                            () => this.ShockWaves[index].Guid != guid
                                ? f // Prevents overlapping animation conflicts
                                : this.ShockWaves[index].SizeRatio,
                            x => {
                                // Prevents overlapping animation conflicts
                                if (this.ShockWaves[index].Guid != guid) return;
                                this.ShockWaves[index].SizeRatio = x;
                            },
                            1,
                            duration
                        )
                        .SetEase(EASE)
                );

            this.ShockWaves[index]
                .Sequence.OnComplete(() => {
                        if (this.ShockWaves[index].Guid != guid) return;

                        this.ShockWaveMaterial.SetVector(POSITIONS[index], Vector2.zero);
                        this.ShockWaveMaterial.SetFloat(MAX_SIZES[index], 0);
                        this.ShockWaveMaterial.SetFloat(STRENGTHS[index], 0);
                        this.ShockWaveMaterial.SetFloat(SIZE_RATIOS[index], 0);
                        this.ShockWaves[index].Active = false;
                    }
                );
        }
    }
}
