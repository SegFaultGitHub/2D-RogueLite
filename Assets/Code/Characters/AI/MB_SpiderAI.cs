using System.Collections;
using Code.Characters.Enemies;
using MyBox;
using UnityEngine;

namespace Code.Characters.AI {
    public class MB_SpiderAI : AMB_AI {
        #region Members
        [Foldout("MB_SpiderAI", true)]
        [SerializeField] private protected RangedFloat m_DashRange;
        [SerializeField] private protected RangedFloat m_RestRange;

        [SerializeField] private protected RangedInt m_DashesBetweenSpiderWebsRange;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_Spider m_Spider;

        [ReadOnly][SerializeField] private protected int m_NextSpiderWeb;

        [ReadOnly][SerializeField] private protected Vector2 m_MovementDirection;
        #endregion

        #region Getters / Setters
        private RangedFloat DashRange { get => this.m_DashRange; }
        private RangedFloat RestRange { get => this.m_RestRange; }

        private RangedInt DashesBetweenSpiderWebsRange { get => this.m_DashesBetweenSpiderWebsRange; }

        private MB_Spider Spider { get => this.m_Spider; set => this.m_Spider = value; }

        private int NextSpiderWeb { get => this.m_NextSpiderWeb; set => this.m_NextSpiderWeb = value; }

        private Vector2 MovementDirection { get => this.m_MovementDirection; set => this.m_MovementDirection = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.Spider = this.GetComponent<MB_Spider>();
            this.NextSpiderWeb = Random.Range(0, this.DashesBetweenSpiderWebsRange.Max);
        }

        protected override void OnEnable() {
            base.OnEnable();

            this.StartCoroutine(this.MoveCoroutine());
        }

        protected void OnDisable() {
            this.StopAllCoroutines();
        }
        #endregion

        protected override void UpdateBehaviour() { }
        protected override Vector2 GetMovementDirection() => this.MovementDirection;
        protected override Vector2 GetAimDirection() => this.MovementDirection;

        private IEnumerator MoveCoroutine() {
            this.MovementDirection = Vector2.zero;
            yield return new WaitForSeconds(Random.Range(this.RestRange.Min, this.RestRange.Max));

            this.MovementDirection = this.GetDirectionToPlayer(0, 1, 2);
            yield return new WaitForSeconds(Random.Range(this.DashRange.Min, this.DashRange.Max));

            if (this.NextSpiderWeb == 0) {
                this.Spider.UseSpell();
                this.NextSpiderWeb = Random.Range(this.DashesBetweenSpiderWebsRange.Min, this.DashesBetweenSpiderWebsRange.Max);
            }

            this.NextSpiderWeb--;

            this.StartCoroutine(this.MoveCoroutine());
        }
    }
}
