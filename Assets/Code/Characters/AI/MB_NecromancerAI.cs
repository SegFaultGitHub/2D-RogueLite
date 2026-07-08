using Code.Characters.Enemies;
using Code.Map;
using Code.Utils;
using MyBox;
using UnityEditor;
using UnityEngine;

namespace Code.Characters.AI {
    public class MB_NecromancerAI : AMB_AI {
        #region Members
        [Foldout("MB_NecromancerAI", true)]
        [SerializeField] private protected float m_FocusDuration;
        [SerializeField] private protected float m_RestDuration;
        [SerializeField] private protected RangedFloat m_SummonDistance;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_Necromancer m_Necromancer;

        [ReadOnly][SerializeField] private protected bool m_Focusing;
        [ReadOnly][SerializeField] private protected bool m_Resting;
        #endregion

        #region Getters / Setters
        private float FocusDuration { get => this.m_FocusDuration; }
        private float RestDuration { get => this.m_RestDuration; }
        private RangedFloat SummonDistance { get => this.m_SummonDistance; }

        private MB_Necromancer Necromancer { get => this.m_Necromancer; set => this.m_Necromancer = value; }

        private bool Focusing { get => this.m_Focusing; set => this.m_Focusing = value; }
        private bool Resting { get => this.m_Resting; set => this.m_Resting = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.Necromancer = this.GetComponent<MB_Necromancer>();
        }

        #if UNITY_EDITOR
        protected override void OnDrawGizmos() {
            base.OnDrawGizmos();

            Handles.color = Color.red;
            Handles.DrawWireArc(this.transform.position, Vector3.forward, Vector3.down, 360, this.SummonDistance.Min);
            Handles.DrawWireArc(this.transform.position, Vector3.forward, Vector3.down, 360, this.SummonDistance.Max);
        }
        #endif
        #endregion


        protected override void UpdateBehaviour() {
            if (!this.Resting && !this.Focusing) {
                if (this.Necromancer.CanUseSummonSpell()) this.UseSummonSpell();
                else if (this.Necromancer.CanUseScreamSpell()) this.UseScreamSpell();
            }
        }

        private void UseSummonSpell() {
            this.Decision.MovementDirection *= 0;
            this.Focusing = true;
            this.Necromancer.Focus(true);
            this.InSeconds(
                this.FocusDuration,
                () => {
                    this.Necromancer.Focus(false);
                    this.Focusing = false;
                    this.Resting = true;
                    float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                    float distance = Random.Range(this.SummonDistance.Min, this.SummonDistance.Max);
                    C_Node node1 = this.ObjectsManager.RoomManager.Room.GetNearestGroundNode(
                        this.Necromancer.transform.position.ToVector2() + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance
                    );
                    angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                    distance = Random.Range(this.SummonDistance.Min, this.SummonDistance.Max);
                    C_Node node2 = this.ObjectsManager.RoomManager.Room.GetNearestGroundNode(
                        this.Necromancer.transform.position.ToVector2() + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance
                    );
                    this.Necromancer.UseSummonSpell(node1.WorldPosition, node2.WorldPosition);
                    this.InSeconds(this.RestDuration, () => this.Resting = false);
                }
            );
        }

        private void UseScreamSpell() {
            this.Decision.MovementDirection *= 0;
            this.Focusing = true;
            this.Necromancer.Focus(true);
            this.InSeconds(
                this.FocusDuration,
                () => {
                    this.Necromancer.Focus(false);
                    this.Focusing = false;
                    this.Resting = true;
                    this.Necromancer.UseScreamSpell(this.Necromancer.transform.position.ToVector2());
                    this.InSeconds(this.RestDuration, () => this.Resting = false);
                }
            );
        }

        protected override Vector2 GetMovementDirection() => Vector2.zero;

        protected override Vector2 GetAimDirection() => Vector2.zero;
    }
}
