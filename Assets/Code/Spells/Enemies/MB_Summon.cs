using System.Collections.Generic;
using Code.Characters;
using Code.Characters.Enemies;
using Code.Utils;
using MyBox;
using UnityEngine;

namespace Code.Spells.Enemies {
    public class MB_Summon : AMB_PositionalSpell {
        #region Members
        [Foldout("MB_Summon", true)]
        [SerializeField] private protected List<C_WeightedObject<AMB_Enemy>> m_Enemies;
        [SerializeField] private protected MB_SummonSpawner m_Spawner;
        #endregion

        #region Getters / Setters
        private List<C_WeightedObject<AMB_Enemy>> Enemies { get => this.m_Enemies; }
        private MB_SummonSpawner Spawner { get => this.m_Spawner; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public override void SetPositions(Vector2 from, Vector2 to) {
            base.SetPositions(from, to);
            Instantiate(this.Spawner, this.ObjectsManager.RoomManager.Room.EnemiesParent);
            this.Spawner.transform.position = to;
            this.Spawner.Enemies = this.Enemies;
            this.Spawner.Summoner = this.Character as AMB_Enemy;
            this.Collide(null, true);
            this.Destroyer.Destroy();
        }

        public override void TriggerEnter(AMB_Character character) { }
        public override void TriggerStay(AMB_Character character) { }
        public override void TriggerExit(AMB_Character character) { }
    }
}
