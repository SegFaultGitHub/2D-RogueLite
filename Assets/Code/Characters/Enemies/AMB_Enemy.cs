using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Characters.AI;
using Code.Characters.Controllers.Enemies;
using Code.Managers;
using Code.Map;
using Code.UI.HUD;
using Code.UI.Misc;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Characters.Enemies {
    public abstract class AMB_Enemy : AMB_Character {
        #region Members
        [Foldout("AMB_Enemy", true)]
        [SerializeField] private protected bool m_IsABoss;
        [ConditionalField(nameof(m_IsABoss), false, false)]
        [SerializeField] private protected MB_ProgressBar m_LifeBar;
        [ConditionalField(nameof(m_IsABoss), false, true)]
        [SerializeField] private protected string m_BossName;
        [SerializeField] private protected GameObject m_DisappearAnimation;

        [SerializeField] private protected float m_FOVSize;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_EnemyController m_EnemyController;
        [ReadOnly][SerializeField] private protected AMB_AI m_AI;

        [ReadOnly][SerializeField] private protected CollectionWrapperList<AMB_Enemy> m_Summons;
        [ReadOnly][SerializeField] private protected bool m_IsSummon;
        [ReadOnly][SerializeField][ConditionalField(nameof(m_IsSummon), false, true)] private protected AMB_Enemy m_Summoner;

        [ReadOnly][SerializeField] private protected MB_Room m_Room;
        [ReadOnly][SerializeField] private protected int m_Wave;
        #endregion

        #region Getters / Setters
        private bool IsABoss { get => this.m_IsABoss; }
        private MB_ProgressBar LifeBar { get => this.m_LifeBar; set => this.m_LifeBar = value; }
        private GameObject DisappearAnimation { get => this.m_DisappearAnimation; }
        private string BossName { get => this.m_BossName; }

        private float FOVSize { get => this.m_FOVSize; }

        protected MB_EnemyController EnemyController { get => this.m_EnemyController; private set => this.m_EnemyController = value; }
        protected AMB_AI AI { get => this.m_AI; private set => this.m_AI = value; }

        public CollectionWrapperList<AMB_Enemy> Summons { get => this.m_Summons; }
        public bool IsSummon { get => this.m_IsSummon; set => this.m_IsSummon = value; }
        public AMB_Enemy Summoner { get => this.m_Summoner; set => this.m_Summoner = value; }

        private MB_Room Room { get => this.m_Room; set => this.m_Room = value; }
        public int Wave { get => this.m_Wave; set => this.m_Wave = value; }

        public abstract E_Enemy Enemy { get; }
        public override IEnumerable<I_Effect> AllEffects { get => this.Effects.Value.Select(effect => (I_Effect)effect).ToList(); }
        private Coroutine HideLifeBarCoroutine { get; set; }
        #endregion

        #region Static / Readonly / Const
        private const float LIFE_BAR_VISIBLE_FOR = 2;
        #endregion

        #region Unity methods
        protected override void Awake() {
            base.Awake();

            this.EnemyController = this.GetComponent<MB_EnemyController>();
            this.AI = this.GetComponent<AMB_AI>();

            this.ObjectsManager = FindFirstObjectByType<MB_ObjectsManager>(FindObjectsInactive.Include);

            if (this.IsABoss) {
                this.LifeBar = this.ObjectsManager.BossLifeBar;
                //this.LifeBar.gameObject.SetActive(true);
                this.ObjectsManager.BossLifeBar.ForceSetRatio(1);
                this.ObjectsManager.BossLifeBar.SetBossName(this.BossName);
                this.ObjectsManager.DissolveManager.Show(
                    new List<Transform> { this.ObjectsManager.BossLifeBar.transform },
                    () => this.ObjectsManager.BossLifeBar.gameObject.SetActive(true)
                );
            } else {
                this.LifeBar.ForceSetRatio(1);
            }

            this.LifeBar.gameObject.SetActive(false);

            this.Room = this.GetComponentInParent<MB_Room>();
            this.Room.Register(this);
            this.ObjectsManager.EnemyIndicatorsManager.Register(this);
        }

        protected virtual void OnEnable() {
            if (this.FOVSize > 0) {
                this.ObjectsManager.FOVManager.Register(this.Center, this.FOVSize, .75f);
            }
        }

        private void OnDestroy() {
            this.Room.Unregister(this);
            this.ObjectsManager.EnemyIndicatorsManager.Unregister(this);
        }
        #endregion

        protected override bool Die(AMB_Character killedBy) {
            bool died = base.Die(killedBy);

            if (!died) return false;

            GameObject disappear = Instantiate(this.DisappearAnimation, this.ObjectsManager.SpellsTransform);
            disappear.transform.position = this.Center.position;

            if (this.IsABoss) {
                this.LifeBar.ForceSetRatio(0);
                this.ObjectsManager.DissolveManager.Hide(new List<Transform> { this.ObjectsManager.BossLifeBar.transform }, () => { });
                this.LifeBar.gameObject.SetActive(false);
            }

            if (this.IsSummon && this.Summoner != null) {
                this.Summoner.TakeDamage(
                    becomeInvulnerable: false,
                    freeze: false,
                    value: 10,
                    critical: false,
                    from: killedBy,
                    source: E_DamageSource.SummonDeath
                );
                this.Summoner.Summons.Remove(this);
            }

            Destroy(this.gameObject);

            return true;
        }

        public override int TakeDamage(
            bool becomeInvulnerable,
            bool freeze,
            float value,
            bool critical,
            AMB_Character from,
            E_DamageSource source
        ) {
            int damageTaken = base.TakeDamage(becomeInvulnerable, false, value, critical, from, source);
            if (damageTaken == 0) return 0;

            if (!this.IsABoss) {
                this.LifeBar.gameObject.SetActive(true);
                this.HideLifeBar();
                this.LifeBar.Shake();
            }

            this.LifeBar.SetRatio(this.CharacterStats.HealthRatio);

            return damageTaken;
        }

        private void HideLifeBar() {
            if (this.HideLifeBarCoroutine != null) this.StopCoroutine(this.HideLifeBarCoroutine);

            IEnumerator _Coroutine() {
                yield return new WaitForSeconds(LIFE_BAR_VISIBLE_FOR);
                this.LifeBar.gameObject.SetActive(false);
            }

            this.HideLifeBarCoroutine = this.StartCoroutine(_Coroutine());
        }
    }
}
