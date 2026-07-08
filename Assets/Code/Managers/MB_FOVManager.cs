using System;
using System.Linq;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Managers {
    public class MB_FOVManager : MonoBehaviour {
        [Serializable]
        private protected class C_Follow {
            [SerializeField] private protected Transform m_Follow;
            [SerializeField] private protected Vector2 m_WorldPosition;
            [SerializeField] private protected float m_Size;
            [SerializeField] private protected bool m_Active;
            [SerializeField] private protected float m_Duration;
            [SerializeField] private string m_Guid;

            public Transform Follow { get => this.m_Follow; set => this.m_Follow = value; }
            public Vector2 WorldPosition { get => this.m_WorldPosition; set => this.m_WorldPosition = value; }
            public float Size { get => this.m_Size; set => this.m_Size = value; }
            public bool Active { get => this.m_Active; set => this.m_Active = value; }
            public float Duration { get => this.m_Duration; set => this.m_Duration = value; }
            public string Guid { get => this.m_Guid; set => this.m_Guid = value; }

            public Sequence Sequence { get; set; }

            public C_Follow() {
                this.Active = false;
            }

            public bool ShouldStopFollowing() {
                return this.Active && (this.Follow == null || !this.Follow.gameObject.activeInHierarchy);
            }
        }

        #region Members
        [Foldout("MB_FOVManager", true)]
        [SerializeField] private protected float m_DefaultPlayerFOVSize;

        [SerializeField] private protected Image m_NoiseBackground;
        [ReadOnly][SerializeField] private protected Material m_NoiseBackgroundMaterial;
        [SerializeField] private protected Image m_FOV;
        [ReadOnly][SerializeField] private protected Material m_FOVMaterial;
        [SerializeField] private protected Color[] m_NoiseColors; // 120E23 / 201A38

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        [ReadOnly][SerializeField] private protected float m_PlayerFOVSize;
        [ReadOnly][SerializeField] private protected CollectionWrapper<C_Follow> m_Follow;
        #endregion

        #region Getters / Setters
        private float DefaultPlayerFOVSize { get => this.m_DefaultPlayerFOVSize; }

        private Image NoiseBackground { get => this.m_NoiseBackground; }
        private Material NoiseBackgroundMaterial { get => this.m_NoiseBackgroundMaterial; set => this.m_NoiseBackgroundMaterial = value; }
        private Image FOV { get => this.m_FOV; }
        private Material FOVMaterial { get => this.m_FOVMaterial; set => this.m_FOVMaterial = value; }
        private Color[] NoiseColors { get => this.m_NoiseColors; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        public float PlayerFOVSize { get => this.m_PlayerFOVSize; set => this.m_PlayerFOVSize = value; }
        private CollectionWrapper<C_Follow> Follow { get => this.m_Follow; }

        private int[] NextNoiseColors { get; set; }
        private string PlayerTweenGuid { get; set; }
        #endregion

        #region Static / Readonly / Const
        private static readonly int OFFSET = Shader.PropertyToID("_Offset");
        private static readonly int COLOR1 = Shader.PropertyToID("_Color1");
        private static readonly int COLOR2 = Shader.PropertyToID("_Color2");
        private static readonly int ORTHOGRAPHIC_SIZE = Shader.PropertyToID("_OrthographicSize");
        private static readonly int POSITION_PLAYER = Shader.PropertyToID("_Position_Player");
        private static readonly int SIZE_PLAYER = Shader.PropertyToID("_Size_Player");
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
            Shader.PropertyToID("_Position_15"),
            Shader.PropertyToID("_Position_16"),
            Shader.PropertyToID("_Position_17"),
            Shader.PropertyToID("_Position_18"),
            Shader.PropertyToID("_Position_19"),
            Shader.PropertyToID("_Position_20"),
            Shader.PropertyToID("_Position_21"),
            Shader.PropertyToID("_Position_22"),
            Shader.PropertyToID("_Position_23"),
            Shader.PropertyToID("_Position_24"),
            Shader.PropertyToID("_Position_25"),
            Shader.PropertyToID("_Position_26"),
            Shader.PropertyToID("_Position_27"),
            Shader.PropertyToID("_Position_28"),
            Shader.PropertyToID("_Position_29"),
            Shader.PropertyToID("_Position_30"),
            Shader.PropertyToID("_Position_31")
        };
        private static readonly int[] SIZES = {
            Shader.PropertyToID("_Size_0"),
            Shader.PropertyToID("_Size_1"),
            Shader.PropertyToID("_Size_2"),
            Shader.PropertyToID("_Size_3"),
            Shader.PropertyToID("_Size_4"),
            Shader.PropertyToID("_Size_5"),
            Shader.PropertyToID("_Size_6"),
            Shader.PropertyToID("_Size_7"),
            Shader.PropertyToID("_Size_8"),
            Shader.PropertyToID("_Size_9"),
            Shader.PropertyToID("_Size_10"),
            Shader.PropertyToID("_Size_11"),
            Shader.PropertyToID("_Size_12"),
            Shader.PropertyToID("_Size_13"),
            Shader.PropertyToID("_Size_14"),
            Shader.PropertyToID("_Size_15"),
            Shader.PropertyToID("_Size_16"),
            Shader.PropertyToID("_Size_17"),
            Shader.PropertyToID("_Size_18"),
            Shader.PropertyToID("_Size_19"),
            Shader.PropertyToID("_Size_20"),
            Shader.PropertyToID("_Size_21"),
            Shader.PropertyToID("_Size_22"),
            Shader.PropertyToID("_Size_23"),
            Shader.PropertyToID("_Size_24"),
            Shader.PropertyToID("_Size_25"),
            Shader.PropertyToID("_Size_26"),
            Shader.PropertyToID("_Size_27"),
            Shader.PropertyToID("_Size_28"),
            Shader.PropertyToID("_Size_29"),
            Shader.PropertyToID("_Size_30"),
            Shader.PropertyToID("_Size_31")
        };
        #endregion

        #region Unity methods
        private void OnApplicationQuit() {
            this.NoiseBackgroundMaterial.SetColor(COLOR1, this.NoiseColors[0]);
            this.NoiseBackgroundMaterial.SetColor(COLOR2, this.NoiseColors[1]);
            this.NoiseBackgroundMaterial.SetVector(OFFSET, Vector4.zero);
            this.FOVMaterial.SetColor(COLOR1, this.NoiseColors[0]);
            this.FOVMaterial.SetColor(COLOR2, this.NoiseColors[1]);
            this.FOVMaterial.SetVector(OFFSET, Vector4.zero);
            this.FOVMaterial.SetVector(POSITION_PLAYER, new Vector4(.5f, .5f));
            this.FOVMaterial.SetFloat(SIZE_PLAYER, this.DefaultPlayerFOVSize);

            for (int i = 0; i < this.Follow.Length; i++) {
                this.FOVMaterial.SetFloat(SIZES[i], 0);
                this.FOVMaterial.SetVector(POSITIONS[i], Vector4.zero);
            }
        }

        private void Awake() {
            this.FOVMaterial = this.FOV.material;
            this.PlayerFOVSize = this.DefaultPlayerFOVSize;
            this.NoiseBackgroundMaterial = this.NoiseBackground.material;
        }

        private void Start() {
            this.NextNoiseColors = new[] { 0, 1 };
            this.Color1Loop();
            this.Color2Loop();
            this.FOVMaterial.SetFloat(SIZE_PLAYER, this.DefaultPlayerFOVSize);
        }

        private void Update() {
            this.FOVMaterial.SetVector(
                POSITION_PLAYER,
                this.ObjectsManager.MainCamera.GetShaderPosition(this.ObjectsManager.Player.Center.position.ToVector2())
            );
            this.FOVMaterial.SetFloat(ORTHOGRAPHIC_SIZE, this.ObjectsManager.MainCamera.Camera.orthographicSize);

            for (int i = 0; i < this.Follow.Length; i++) {
                if (this.Follow[i].ShouldStopFollowing()) this.Unregister(i);

                if (this.Follow[i].Follow != null) {
                    this.Follow[i].WorldPosition =
                        this.ObjectsManager.MainCamera.GetShaderPosition(this.Follow[i].Follow.position.ToVector2());
                }

                this.FOVMaterial.SetFloat(SIZES[i], this.Follow[i].Size);
                this.FOVMaterial.SetVector(POSITIONS[i], this.Follow[i].WorldPosition);
            }

            Vector2 offset = this.ObjectsManager.MainCamera.transform.position.ToVector2() * .75f;
            this.FOVMaterial.SetVector(OFFSET, offset);
            this.NoiseBackgroundMaterial.SetVector(OFFSET, offset);
        }
        #endregion

        public void Initialize() {
            this.Follow.Value = new C_Follow[32];
            for (int i = 0; i < this.Follow.Length; i++) {
                this.Follow[i] = new C_Follow();
            }
        }

        public void PostInitialize() { }

        private void Color1Loop() {
            DOTween.To( //
                    () => this.NoiseBackgroundMaterial.GetColor(COLOR1),
                    color => {
                        this.NoiseBackgroundMaterial.SetColor(COLOR1, color);
                        this.FOVMaterial.SetColor(COLOR1, color);
                    },
                    this.NoiseColors[this.NextNoiseColors[0]],
                    Random.Range(20f, 40f)
                )
                .OnComplete(() => {
                        this.NextNoiseColors[0] = (this.NextNoiseColors[0] + 1) % 2;
                        this.Color1Loop();
                    }
                );
        }

        private void Color2Loop() {
            DOTween.To( //
                    () => this.NoiseBackgroundMaterial.GetColor(COLOR2),
                    color => {
                        this.NoiseBackgroundMaterial.SetColor(COLOR2, color);
                        this.FOVMaterial.SetColor(COLOR2, color);
                    },
                    this.NoiseColors[this.NextNoiseColors[1]],
                    Random.Range(20f, 40f)
                )
                .OnComplete(() => {
                        this.NextNoiseColors[1] = (this.NextNoiseColors[1] + 1) % 2;
                        this.Color2Loop();
                    }
                );
        }

        public void Register(Transform transformToFollow, float size, float duration) {
            if (this.Follow.Value.Any(follow => follow.Follow == transformToFollow && follow.Size != 0)) {
                return;
            }

            int index = -1;
            int maxScore = 1;
            for (int i = this.Follow.Length - 1; i >= 0; i--) {
                int score = 0;
                if (this.Follow[i].Size == 0) score += 2;
                if (!this.Follow[i].Active) score += 1;

                if (score >= maxScore) {
                    index = i;
                    maxScore = score;
                }
            }

            if (index == -1) {
                Debug.LogError("Max follow targets reached!");
                return;
            }

            DOTween.Kill(this.Follow[index].Sequence);

            string guid = Guid.NewGuid().ToString();
            const float f = 0.001f;

            this.Follow[index].Follow = transformToFollow;
            this.Follow[index].Duration = duration;
            this.Follow[index].Guid = guid;
            this.Follow[index].Sequence = DOTween.Sequence();
            this.Follow[index].Active = true;

            this.Follow[index]
                .Sequence.Append(
                    DOTween.To( //
                            () => this.Follow[index].Guid != guid
                                ? f // Prevents overlapping animation conflicts
                                : 0.001f,
                            x => {
                                // Prevents overlapping animation conflicts
                                if (this.Follow[index].Guid != guid) return;
                                this.Follow[index].Size = x;
                            },
                            size,
                            duration
                        )
                        .SetEase(Ease.OutSine)
                );
        }

        public void Unregister(Transform transformToFollow) {
            for (int i = 0; i < this.Follow.Length; i++) {
                if (this.Follow[i].Follow == transformToFollow) this.Unregister(i);
            }
        }

        public void Unregister(int index) {
            DOTween.Kill(this.Follow[index].Sequence);

            string guid = Guid.NewGuid().ToString();
            const float f = 0;

            this.Follow[index].Follow = null;
            this.Follow[index].Active = false;
            this.Follow[index].Guid = guid;
            this.Follow[index].Sequence = DOTween.Sequence();

            this.Follow[index]
                .Sequence.Append(
                    DOTween.To( //
                            () => this.Follow[index].Guid != guid
                                ? f // Prevents overlapping animation conflicts
                                : this.Follow[index].Size,
                            size => {
                                // Prevents overlapping animation conflicts
                                if (this.Follow[index].Guid != guid) return;
                                this.Follow[index].Size = size;
                            },
                            0,
                            this.Follow[index].Duration
                        )
                        .SetEase(Ease.OutSine)
                );
            this.Follow[index]
                .Sequence.OnComplete(() => {
                        if (this.Follow[index].Guid != guid) return;
                        this.Follow[index] = new C_Follow();
                    }
                );
        }

        public void SetPlayerFOVSize(float size, float duration, Ease ease) {
            string guid = Guid.NewGuid().ToString();
            this.PlayerTweenGuid = guid;
            DOTween.To(
                    () => this.FOVMaterial.GetFloat(SIZE_PLAYER),
                    x => {
                        if (this.PlayerTweenGuid == guid) {
                            this.FOVMaterial.SetFloat(SIZE_PLAYER, x);
                            this.PlayerFOVSize = x;
                        }
                    },
                    size,
                    duration
                )
                .SetEase(ease);
        }

        public void SetDefaultPlayerFOVSize(float duration, Ease ease) => this.SetPlayerFOVSize(this.DefaultPlayerFOVSize, duration, ease);
    }
}
