using System.Collections.Generic;
using Code.Characters.Enemies;
using Code.Managers;
using Code.Map;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Characters {
    public class MB_EnemySpawner : MB_DestroyOnAnimationEvent {
        #region Members
        [Foldout("MB_EnemySpawner", true)]
        [SerializeField] private protected bool m_RandomDelay;
        [SerializeField][ConditionalField(nameof(m_RandomDelay))] private protected RangedFloat m_DelayRange;
        [SerializeField][ConditionalField(nameof(m_RandomDelay), false, false)] private protected float m_Delay;
        [SerializeField] private protected List<C_WeightedObject<AMB_Enemy>> m_Enemies;
        [SerializeField] private protected int m_Wave;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected Animator m_Animator;
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        [ReadOnly][SerializeField] private protected MB_Room m_Room;
        #endregion

        #region Getters / Setters
        private bool RandomDelay { get => this.m_RandomDelay; }
        private RangedFloat DelayRange { get => this.m_DelayRange; }
        private float Delay { get => this.m_Delay; set => this.m_Delay = value; }
        public List<C_WeightedObject<AMB_Enemy>> Enemies { get => this.m_Enemies; set => this.m_Enemies = value; }
        public int Wave { get => this.m_Wave; }

        private Animator Animator { get => this.m_Animator; set => this.m_Animator = value; }
        private MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        private MB_Room Room { get => this.m_Room; set => this.m_Room = value; }
        #endregion

        #region Static / Readonly / Const
        private static readonly int RUN = Animator.StringToHash("Run");
        #endregion

        #region Unity methods
        private void Awake() {
            this.Animator = this.GetComponent<Animator>();
            this.ObjectsManager = FindFirstObjectByType<MB_ObjectsManager>(FindObjectsInactive.Include);

            this.Room = this.GetComponentInParent<MB_Room>();
            this.Room.Register(this);
        }

        private void Start() {
            this.Until(
                () => this.Room.Ready,
                () => {
                    this.Until(
                        () => this.Room.CurrentWave == this.Wave,
                        () => {
                            float delay = this.RandomDelay
                                ? Random.Range(this.DelayRange.Min, this.DelayRange.Max)
                                : this.Delay;
                            this.InSeconds(delay, () => this.Animator.SetTrigger(RUN));
                        }
                    );
                }
            );
        }

        private void OnDestroy() {
            this.Room.Unregister(this);
        }
        #endregion

        public virtual AMB_Enemy SpawnEnemy() {
            AMB_Enemy enemy = Instantiate(SC_Utils.Sample(this.Enemies).Obj, this.transform.parent, true);
            enemy.Wave = this.Wave;
            enemy.transform.position = this.transform.position;
            return enemy;
        }
    }
}
