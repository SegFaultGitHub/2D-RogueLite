using Code.Characters.Enemies;
using MyBox;
using UnityEngine;

namespace Code.Characters {
    public class MB_SummonSpawner : MB_EnemySpawner {
        #region Members
        [Foldout("MB_SummonSpawner", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected AMB_Enemy m_Summoner;
        #endregion

        #region Getters / Setters
        public AMB_Enemy Summoner { get => this.m_Summoner; set => this.m_Summoner = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override AMB_Enemy SpawnEnemy() {
            AMB_Enemy enemy = base.SpawnEnemy();
            enemy.IsSummon = true;
            enemy.Summoner = this.Summoner;
            enemy.Wave = -1;
            return enemy;
        }
    }
}
