using Code.Managers;
using MyBox;
using UnityEngine;

namespace Code.Characters {
    public class MB_PlayerSpawner : MonoBehaviour {
        #region Members
        [Foldout("MB_PlayerSpawner", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        #endregion

        #region Getters / Setters
        private MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        private void Awake() {
            this.ObjectsManager = FindFirstObjectByType<MB_ObjectsManager>(FindObjectsInactive.Include);
        }
        #endregion

        public void ShowPlayer() => this.ObjectsManager.Player.Show();
    }
}
