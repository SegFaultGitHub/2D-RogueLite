using System;
using Code.Utils;
using MyBox;
using UnityEditor;
using UnityEngine;

namespace Code.Spells {
    public abstract class AMB_RotatingSpell : AMB_MovingSpell {
        private protected enum E_RotationType {
            Duration,
            Angle
        }
        private protected enum E_DirectionType {
            Static,
            Angle,
            Direction
        }

        #region Members
        [Foldout("AMB_RotatingSpell", true)]
        [SerializeField] private protected float m_RotationSpeed; // per second
        [SerializeField] private protected float m_Distance;

        [SerializeField] private protected bool m_FollowAim;

        [SerializeField] private protected float m_InitialOffsetAngle;

        [SerializeField] private protected bool m_RandomRotation;
        [SerializeField] private protected E_RotationType m_RotationType;
        [ConditionalField(nameof(m_RotationType), false, E_RotationType.Duration)]
        [SerializeField] private protected float m_RotationDuration;
        [ConditionalField(nameof(m_RotationType), false, E_RotationType.Angle)]
        [SerializeField] private protected float m_RotationAngle;

        [SerializeField] private protected E_DirectionType m_DirectionType;
        [ConditionalField(nameof(m_DirectionType), true, E_DirectionType.Static)]
        [SerializeField] private protected float m_DirectionLerp;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Transform m_Follow;
        [ReadOnly][SerializeField] private protected float m_Angle;
        [ReadOnly][SerializeField] private protected bool m_StopFollowing;
        [ReadOnly][SerializeField] private protected float m_BaseAngle;
        [ConditionalField(nameof(m_RotationType), false, E_RotationType.Angle)]
        [ReadOnly][SerializeField] private protected float m_TotalAngle;
        [ReadOnly][SerializeField] private protected float m_OffsetAngle;

        [ConditionalField(nameof(m_DirectionType), false, E_DirectionType.Direction)]
        [ReadOnly][SerializeField] private protected Vector2 m_PreviousPosition;
        #endregion

        #region Getters / Setters
        protected float RotationSpeed { get => this.m_RotationSpeed; set => this.m_RotationSpeed = value; }
        protected float Distance { get => this.m_Distance; set => this.m_Distance = value; }

        private bool FollowAim { get => this.m_FollowAim; }

        private float InitialOffsetAngle { get => this.m_InitialOffsetAngle; }

        private bool RandomRotation { get => this.m_RandomRotation; }
        private E_RotationType RotationType { get => this.m_RotationType; }
        public float RotationDuration { get => this.m_RotationDuration; }
        protected float RotationAngle { get => this.m_RotationAngle; }

        private E_DirectionType DirectionType { get => this.m_DirectionType; }
        private float DirectionLerp { get => this.m_DirectionLerp; }

        public Transform Follow { get => this.m_Follow; set => this.m_Follow = value; }
        protected float Angle { get => this.m_Angle; private set => this.m_Angle = value; }
        private bool StopFollowing { get => this.m_StopFollowing; set => this.m_StopFollowing = value; }
        private float BaseAngle { get => this.m_BaseAngle; set => this.m_BaseAngle = value; }
        protected float TotalAngle { get => this.m_TotalAngle; private set => this.m_TotalAngle = value; }
        private float OffsetAngle { get => this.m_OffsetAngle; set => this.m_OffsetAngle = value; }
        private float TrueAngle { get => this.Angle + this.OffsetAngle; }

        private Vector2 PreviousPosition { get => this.m_PreviousPosition; set => this.m_PreviousPosition = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Start() {
            base.Start();

            if (this.RandomRotation && SC_Utils.Rate(.5f)) this.RotationSpeed *= -1;

            switch (this.RotationType) {
                case E_RotationType.Duration:
                    this.InSeconds(this.RotationDuration, () => this.Collide(null, true));
                    break;
                case E_RotationType.Angle:
                    this.TotalAngle = 0;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected override void Update() {
            base.Update();

            if (this.StopFollowing) return;

            if (this.Follow == null) {
                this.StopFollowing = true;
                this.Destroy();
                return;
            }

            float rotationSpeed = this.RotationSpeed * Time.deltaTime;

            this.TotalAngle += Mathf.Abs(rotationSpeed);
            this.OffsetAngle = this.FollowAim
                ? this.Character.BaseController.AimAngle - this.BaseAngle
                : 0;
            this.SetAngle(this.Angle + rotationSpeed);

            if (this.RotationType == E_RotationType.Angle && this.TotalAngle >= this.RotationAngle) {
                this.StopFollowing = true;
                this.Collide(null, true);
            }
        }
        #endregion

        public virtual void SetInitialAngle(float angle, bool reverseRotation) {
            this.BaseAngle = angle;
            if (reverseRotation) {
                this.SetAngle(angle - this.InitialOffsetAngle);
                this.RotationSpeed *= -1;
            } else {
                this.SetAngle(angle + this.InitialOffsetAngle);
            }
        }

        private void SetAngle(float angle) {
            this.Angle = angle;
            this.Direction = SC_Utils.Rotate(new Vector2(this.Distance, 0), this.TrueAngle);
            this.transform.position = this.Follow.position.ToVector2() + SC_Utils.Rotate(new Vector2(this.Distance, 0), this.TrueAngle);

            switch (this.DirectionType) {
                case E_DirectionType.Static:
                    break;
                case E_DirectionType.Angle:
                    this.Sprites.transform.eulerAngles = Vector3.Lerp(
                        this.Sprites.transform.eulerAngles,
                        new Vector3(0, 0, this.TrueAngle),
                        this.DirectionLerp
                    );
                    break;
                case E_DirectionType.Direction:
                    Vector2 currentPosition = this.transform.position;
                    this.Sprites.up = Vector2.Lerp(this.Sprites.up, currentPosition - this.PreviousPosition, this.DirectionLerp);
                    this.PreviousPosition = currentPosition;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
