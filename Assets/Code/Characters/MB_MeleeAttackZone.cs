using System;
using Code.Characters.Enemies;
using Code.Characters.Players;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Characters {
    public class MB_MeleeAttackZone : MonoBehaviour {
        #region Members
        [Foldout("MB_MeleeAttackZone", true)]
        [SerializeField] private protected AMB_Player m_Player;

        [SerializeField] private protected float m_Range;
        [SerializeField] private protected float m_Angle;
        [SerializeField] private protected PolygonCollider2D m_Collider;
        [SerializeField] private protected Transform m_TrailBox;
        [SerializeField] private protected TrailRenderer m_Trail;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected AMB_Enemy m_Enemy;
        #endregion

        #region Getters / Setters
        private float Range { get => this.m_Range; }
        private float Angle { get => this.m_Angle; }
        private PolygonCollider2D Collider { get => this.m_Collider; }
        private Transform TrailBox { get => this.m_TrailBox; }
        private TrailRenderer Trail { get => this.m_Trail; }

        private AMB_Enemy Enemy { get => this.m_Enemy; set => this.m_Enemy = value; }
        #endregion

        #region Static / Readonly / Const
        private const int POINTS_COUNT = 10;
        #endregion

        #region Unity methods
        private void OnValidate() {
            this.Collider.pathCount = 1;

            float angle = -this.Angle / 2;
            float stepAngle = this.Angle / (POINTS_COUNT - 1);
            Vector2[] points = new Vector2[POINTS_COUNT + 1];
            for (int i = 1; i < POINTS_COUNT + 1; i++) {
                points[i] = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * this.Range, Mathf.Sin(angle * Mathf.Deg2Rad) * this.Range);
                angle += stepAngle;
            }

            this.Trail.widthMultiplier = this.Range / 2;
            this.Trail.transform.localPosition = new Vector3(this.Range * .75f, 0, 0);

            this.Collider.SetPath(0, points);
        }

        private void Start() {
            this.Enemy = this.GetComponentInParent<AMB_Enemy>();

            float angle = SC_Utils.Rate(.5f)
                ? -this.Angle / 2
                : this.Angle / 2;
            this.TrailBox.localEulerAngles = new Vector3(0, 0, angle);
        }

        private void FixedUpdate() {
            this.transform.right = this.m_Player.Center.position - this.Enemy.Center.position;
        }
        #endregion

        [ButtonMethod]
        protected void UseMainSpell() {
            float swingDuration = .125f;
            float currentAngle = this.TrailBox.localEulerAngles.z % 360;
            float startAngle = currentAngle > 180
                ? currentAngle - 360
                : currentAngle;
            float destinationAngle = startAngle > 0
                ? -this.Angle / 2
                : this.Angle / 2;

            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() => this.Trail.emitting = true)
                .Append(
                    DOTween.To( //
                        () => startAngle,
                        z => this.TrailBox.localEulerAngles = new Vector3(0, 0, z),
                        destinationAngle,
                        swingDuration
                    )
                )
                .AppendCallback(() => this.Trail.emitting = false);
        }
    }
}
