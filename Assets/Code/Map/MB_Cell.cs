using System;
using System.Collections.Generic;
using Code.Managers;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Map {
    [SelectionBase]
    public class MB_Cell : MonoBehaviour {
        [Serializable]
        private protected class C_Sprites {
            [SerializeField] private protected List<Sprite> m_DungeonSprites;
            [SerializeField] private protected List<Sprite> m_DesertSprites;
            [SerializeField] private protected List<Sprite> m_ForestSprites;
            [SerializeField] private protected List<Sprite> m_IceSprites;
            [SerializeField] private protected List<Sprite> m_CaveSprites;

            public List<Sprite> DungeonSprites { get => this.m_DungeonSprites; }
            public List<Sprite> DesertSprites { get => this.m_DesertSprites; }
            public List<Sprite> ForestSprites { get => this.m_ForestSprites; }
            public List<Sprite> IceSprites { get => this.m_IceSprites; }
            public List<Sprite> CaveSprites { get => this.m_CaveSprites; }
        }

        #region Members
        [Foldout("MB_Cell", true)]
        [SerializeField] private protected MB_Node m_Node;

        [SerializeField] private protected bool m_IsHole;
        [ConditionalField(nameof(m_IsHole), false, false)][SerializeField] private protected bool m_HasObstacle;

        [ConditionalField(nameof(m_IsHole), false, false)][SerializeField] private protected Transform m_SpriteBox;

        [SerializeField][ConditionalField(nameof(m_IsHole), false, false)] private protected C_Sprites m_GroundSprites;
        [SerializeField][ConditionalField(nameof(m_IsHole), false, false)] private protected C_Sprites m_WallSprites;
        [SerializeField][ConditionalField(nameof(m_HasObstacle))] private protected C_Sprites m_ObstacleSprites;

        [SerializeField][ConditionalField(nameof(m_IsHole), false, false)] private protected SpriteRenderer m_GroundSprite;
        [SerializeField][ConditionalField(nameof(m_IsHole), false, false)] private protected SpriteRenderer m_WallSprite;
        [SerializeField][ConditionalField(nameof(m_HasObstacle))] private protected SpriteRenderer m_ObstacleSprite;

        [SerializeField] private protected bool m_HasLight;
        [SerializeField][ConditionalField(nameof(m_HasLight), false, true)] private protected Transform m_LightSource;
        [SerializeField][ConditionalField(nameof(m_HasLight), false, true)] private protected float m_LightSize;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        [ReadOnly][SerializeField] private protected float m_Height;
        #endregion

        #region Getters / Setters
        public MB_Node Node { get => this.m_Node; }

        public bool HasObstacle { get => this.m_HasObstacle; }
        public bool IsHole { get => this.m_IsHole; }

        private Transform SpriteBox { get => this.m_SpriteBox; }

        private C_Sprites GroundSprites { get => this.m_GroundSprites; }
        private C_Sprites WallSprites { get => this.m_WallSprites; }
        private C_Sprites ObstacleSprites { get => this.m_ObstacleSprites; }

        private SpriteRenderer GroundSprite { get => this.m_GroundSprite; }
        private SpriteRenderer WallSprite { get => this.m_WallSprite; }
        private SpriteRenderer ObstacleSprite { get => this.m_ObstacleSprite; }

        private bool HasLight { get => this.m_HasLight; }
        private Transform LightSource { get => this.m_LightSource; }
        private float LightSize { get => this.m_LightSize; }

        private MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        public float Height { get => this.m_Height; private set => this.m_Height = value; }
        #endregion

        #region Static / Readonly / Const
        private const float BOUNCE_DURATION = .25f;
        private const float SHOW_HEIGHT = 5f;
        #endregion

        #region Unity methods
        private void Awake() {
            this.ObjectsManager = FindFirstObjectByType<MB_ObjectsManager>(FindObjectsInactive.Include);
            if (!this.IsHole) {
                List<Sprite> groundSprites = MB_InitializationManager.BIOME switch {
                    E_Biome.Dungeon => this.GroundSprites.DungeonSprites,
                    E_Biome.Desert => this.GroundSprites.DesertSprites,
                    E_Biome.Forest => this.GroundSprites.ForestSprites,
                    E_Biome.Ice => this.GroundSprites.IceSprites,
                    E_Biome.Cave => this.GroundSprites.CaveSprites,
                    E_Biome.None => throw new ArgumentOutOfRangeException(),
                    _ => throw new ArgumentOutOfRangeException()
                };
                this.GroundSprite.sprite = SC_Utils.Sample(groundSprites);
                List<Sprite> wallSprites = MB_InitializationManager.BIOME switch {
                    E_Biome.Dungeon => this.WallSprites.DungeonSprites,
                    E_Biome.Desert => this.WallSprites.DesertSprites,
                    E_Biome.Forest => this.WallSprites.ForestSprites,
                    E_Biome.Ice => this.WallSprites.IceSprites,
                    E_Biome.Cave => this.WallSprites.CaveSprites,
                    E_Biome.None => throw new ArgumentOutOfRangeException(),
                    _ => throw new ArgumentOutOfRangeException()
                };
                this.WallSprite.sprite = SC_Utils.Sample(wallSprites);

                if (MB_InitializationManager.BIOME == E_Biome.Dungeon) {
                    this.GroundSprite.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 4) * 90);
                }

                this.GroundSprite.transform.localScale = new Vector3(
                    this.GroundSprite.transform.localScale.x
                    * (SC_Utils.Rate(.5f)
                        ? -1
                        : 1),
                    this.GroundSprite.transform.localScale.y,
                    this.GroundSprite.transform.localScale.z
                );

                this.GroundSprite.transform.localScale = new Vector3(
                    this.GroundSprite.transform.localScale.x,
                    this.GroundSprite.transform.localScale.y
                    * (SC_Utils.Rate(.5f)
                        ? -1
                        : 1),
                    this.GroundSprite.transform.localScale.z
                );
            }

            if (this.HasObstacle) {
                List<Sprite> obstacleSprites = MB_InitializationManager.BIOME switch {
                    E_Biome.Dungeon => this.ObstacleSprites.DungeonSprites,
                    E_Biome.Desert => this.ObstacleSprites.DesertSprites,
                    E_Biome.Forest => this.ObstacleSprites.ForestSprites,
                    E_Biome.Ice => this.ObstacleSprites.IceSprites,
                    E_Biome.Cave => this.ObstacleSprites.CaveSprites,
                    E_Biome.None => throw new ArgumentOutOfRangeException(),
                    _ => throw new ArgumentOutOfRangeException()
                };
                this.ObstacleSprite.sprite = SC_Utils.Sample(obstacleSprites);
                this.ObstacleSprite.transform.localScale = new Vector3(
                    this.ObstacleSprite.transform.localScale.x
                    * (SC_Utils.Rate(.5f)
                        ? -1
                        : 1),
                    this.ObstacleSprite.transform.localScale.y,
                    this.ObstacleSprite.transform.localScale.z
                );
            }
        }
        #endregion

        public void Show(float duration) {
            if (this.IsHole) return;
            this.GroundSprite.color = new Color(this.GroundSprite.color.r, this.GroundSprite.color.g, this.GroundSprite.color.b, 0);
            this.WallSprite.color = new Color(this.WallSprite.color.r, this.WallSprite.color.g, this.WallSprite.color.b, 0);

            if (this.HasLight) this.ObjectsManager.FOVManager.Unregister(this.LightSource);
            this.SetHeight(-SHOW_HEIGHT);

            Sequence sequence = DOTween.Sequence();
            if (this.HasObstacle) {
                this.ObstacleSprite.transform.localPosition = new Vector3(0, SHOW_HEIGHT / 2, 0);
                this.ObstacleSprite.color = new Color(
                    this.ObstacleSprite.color.r,
                    this.ObstacleSprite.color.g,
                    this.ObstacleSprite.color.b,
                    0
                );
            }

            sequence.Append(
                    DOTween.To( //
                        () => 0f,
                        a => {
                            this.GroundSprite.color = new Color(
                                this.GroundSprite.color.r,
                                this.GroundSprite.color.g,
                                this.GroundSprite.color.b,
                                a
                            );
                            this.WallSprite.color = new Color(this.WallSprite.color.r, this.WallSprite.color.g, this.WallSprite.color.b, a);
                        },
                        1f,
                        duration
                    )
                )
                .Join(
                    DOTween.To( //
                            () => this.Height,
                            this.SetHeight,
                            0f,
                            duration
                        )
                        .SetEase(Ease.OutSine)
                );
            if (this.HasObstacle) {
                sequence.Join(
                        DOTween.To( //
                                () => 0f,
                                a => this.ObstacleSprite.color = new Color(
                                    this.ObstacleSprite.color.r,
                                    this.ObstacleSprite.color.g,
                                    this.ObstacleSprite.color.b,
                                    a
                                ),
                                1f,
                                duration
                            )
                    )
                    .Join(
                        DOTween.To( //
                                () => this.ObstacleSprite.transform.localPosition.y,
                                y => this.ObstacleSprite.transform.localPosition = new Vector3(
                                    this.ObstacleSprite.transform.localPosition.x,
                                    y,
                                    this.ObstacleSprite.transform.localPosition.z
                                ),
                                0f,
                                duration
                            )
                            .SetEase(Ease.OutSine)
                    );
            }

            if (this.HasLight) {
                sequence.AppendCallback(() => this.ObjectsManager.FOVManager.Register(this.LightSource, this.LightSize, .5f));
            }
        }

        public void Bounce(float delay, float height, float bounceDurationRatio) {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(delay)
                .Append(
                    DOTween.To( //
                            () => 0f,
                            this.SetHeight,
                            height,
                            BOUNCE_DURATION * bounceDurationRatio
                        )
                        .SetEase(Ease.OutSine)
                )
                .Append(
                    DOTween.To( //
                            () => height,
                            this.SetHeight,
                            0f,
                            BOUNCE_DURATION * bounceDurationRatio
                        )
                        .SetEase(Ease.InSine)
                );
        }

        private void SetHeight(float y) {
            this.SpriteBox.localPosition = new Vector3(0, SC_Utils.Round(y, .125f), -y + .5f);
            this.Height = y;
        }
    }
}
