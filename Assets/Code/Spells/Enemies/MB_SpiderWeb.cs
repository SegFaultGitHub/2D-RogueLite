using Code.Characters;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Spells.Enemies {
    public class MB_SpiderWeb : AMB_PositionalSpell {
        #region Members
        [Foldout("MB_SpiderWeb", true)]
        [SerializeField] private protected float m_SpeedRatio;
        [SerializeField] private protected float m_Duration;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected AMB_Character.C_SpeedRatioRef m_SpeedRatioRef;
        #endregion

        #region Getters / Setters
        private float SpeedRatio { get => this.m_SpeedRatio; }
        private float Duration { get => this.m_Duration; }

        private AMB_Character.C_SpeedRatioRef SpeedRatioRef { get => this.m_SpeedRatioRef; set => this.m_SpeedRatioRef = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();
            this.SpeedRatioRef = new AMB_Character.C_SpeedRatioRef { Ratio = this.SpeedRatio };
            this.transform.localScale *= 0;
            DOTween.Sequence()
                .Append(this.transform.DOScale(1, .5f))
                .AppendInterval(this.Duration)
                .Append(this.transform.DOScale(0, .5f))
                .OnComplete(() => this.Destroyer.Destroy());
        }

        private void OnDestroy() {
            foreach (AMB_Character character in this.AffectedCharacters.Value) {
                character.SpeedRatio.Remove(this.SpeedRatioRef);
            }
        }
        #endregion

        public override void SetPositions(Vector2 from, Vector2 to) {
            base.SetPositions(from, to);

            this.transform.position = to;
        }

        public override void TriggerEnter(AMB_Character character) {
            if (this.AffectedCharacters.Contains(character)) return;
            this.AffectedCharacters.Add(character);
            character.SpeedRatio.Add(this.SpeedRatioRef);
        }

        public override void TriggerStay(AMB_Character character) { }

        public override void TriggerExit(AMB_Character character) {
            this.AffectedCharacters.Remove(character);
            character.SpeedRatio.Remove(this.SpeedRatioRef);
        }
    }
}
