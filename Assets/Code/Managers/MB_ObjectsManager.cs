using Code.Cameras;
using Code.Characters.Controllers;
using Code.Characters.Players;
using Code.Map;
using Code.UI.Damage;
using Code.UI.HUD;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Managers {
    public class MB_ObjectsManager : MonoBehaviour {
        #region Members
        [Foldout("MB_ObjectsManager", true)]

        #region Gameplay objects
        [Separator("Gameplay objects")]
        [ReadOnly][SerializeField] private protected AMB_Player m_Player;
        [SerializeField] private protected Transform m_SpellsTransform;
        #endregion

        #region Managers
        [Separator("Managers")]
        [SerializeField] private protected MB_AudioManager m_AudioManager;
        [SerializeField] private protected MB_PauseManager m_PauseManager;
        [SerializeField] private protected MB_ShockWavesManager m_ShockWavesManager;
        [SerializeField] private protected MB_FOVManager m_FOVManager;
        [SerializeField] private protected MB_RoomManager m_RoomManager;
        [SerializeField] private protected MB_ScreenshotManager m_ScreenshotManager;
        #endregion

        #region UI
        [Separator("UI")]
        [SerializeField] private protected MB_MainCamera m_MainCamera;
        [SerializeField] private protected MB_TransitionManager m_TransitionManager;
        [SerializeField] private protected MB_DamageCanvas m_DamageCanvas;
        [SerializeField] private protected MB_PlayerHUD m_PlayerHUD;
        [SerializeField] private protected MB_EnemyIndicatorsManager m_EnemyIndicatorsManager;
        #endregion
        #endregion

        #region Getters / Setters
        #region Gameplay objects
        public AMB_Player Player { get => this.m_Player; set => this.m_Player = value; }
        public Transform SpellsTransform { get => this.m_SpellsTransform; }
        #endregion

        #region Managers
        public MB_AudioManager AudioManager { get => this.m_AudioManager; }
        public MB_PauseManager PauseManager { get => this.m_PauseManager; }
        public MB_ShockWavesManager ShockWavesManager { get => this.m_ShockWavesManager; }
        public MB_FOVManager FOVManager { get => this.m_FOVManager; }
        public MB_RoomManager RoomManager { get => this.m_RoomManager; }
        public MB_ScreenshotManager ScreenshotManager { get => this.m_ScreenshotManager; }
        #endregion

        #region UI
        public MB_MainCamera MainCamera { get => this.m_MainCamera; }
        public MB_TransitionManager TransitionManager { get => this.m_TransitionManager; }
        public MB_DamageCanvas DamageCanvas { get => this.m_DamageCanvas; }
        public MB_PlayerHUD PlayerHUD { get => this.m_PlayerHUD; }
        public MB_EnemyIndicatorsManager EnemyIndicatorsManager { get => this.m_EnemyIndicatorsManager; }
        #endregion
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void Initialize() {
            this.Player.ObjectsManager = this;
            this.Player.GetComponent<AMB_BaseController>().ObjectsManager = this;

            this.PauseManager.ObjectsManager = this;
            this.ShockWavesManager.ObjectsManager = this;
            this.FOVManager.ObjectsManager = this;
            this.RoomManager.ObjectsManager = this;
            this.ScreenshotManager.ObjectsManager = this;

            this.MainCamera.ObjectsManager = this;
            this.TransitionManager.ObjectsManager = this;
            this.PlayerHUD.ObjectsManager = this;
            this.EnemyIndicatorsManager.ObjectsManager = this;

            /*----------------------------------------------------*/

            this.Player.Initialize();

            this.AudioManager.Initialize();
            this.PauseManager.Initialize();
            this.ShockWavesManager.Initialize();
            this.FOVManager.Initialize();
            this.RoomManager.Initialize();
            this.ScreenshotManager.Initialize();

            this.MainCamera.Initialize();
            this.TransitionManager.Initialize();
            this.PlayerHUD.Initialize();
            this.EnemyIndicatorsManager.Initialize();

            /*----------------------------------------------------*/

            this.Player.PostInitialize();

            this.AudioManager.PostInitialize();
            this.PauseManager.PostInitialize();
            this.ShockWavesManager.PostInitialize();
            this.FOVManager.PostInitialize();
            this.RoomManager.PostInitialize();
            this.ScreenshotManager.PostInitialize();

            this.MainCamera.PostInitialize();
            this.TransitionManager.PostInitialize();
            this.PlayerHUD.PostInitialize();
            this.EnemyIndicatorsManager.PostInitialize();
        }
    }
}
