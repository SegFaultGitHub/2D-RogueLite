using Code.Spells.Enemies;
using MyBox;
using UnityEngine;

namespace Code.Characters.Enemies {
    public class MB_Spider : AMB_Enemy {
        #region Members
        [Foldout("MB_Spider", true)]
        [SerializeField] private protected MB_SpiderWeb m_SpiderWeb;
        #endregion

        #region Getters / Setters
        private MB_SpiderWeb SpiderWeb { get => this.m_SpiderWeb; }

        public override E_Enemy Enemy { get => E_Enemy.Spider; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void UseSpell() => this.UseSpell(this.SpiderWeb, this.transform.position);
    }
}
