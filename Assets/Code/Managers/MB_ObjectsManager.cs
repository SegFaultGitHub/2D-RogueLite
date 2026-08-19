using Code.Cameras;
using Code.Characters.Controllers;
using Code.Characters.Players;
using Code.UI.Damage;
using Code.UI.EnhancementList;
using Code.UI.HUD;
using Code.UI.Notifications;
using MyBox;
using UnityEngine;

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
        [SerializeField] private protected MB_StatsManager m_StatsManager;
        [SerializeField] private protected MB_EnhancementsManager m_EnhancementsManager;
        [SerializeField] private protected MB_TransitionManager m_TransitionManager;
        [SerializeField] private protected MB_EnemyIndicatorsManager m_EnemyIndicatorsManager;
        [SerializeField] private protected MB_DissolveManager m_DissolveManager;
        #endregion

        #region UI
        [Separator("UI")]
        [SerializeField] private protected MB_MainCamera m_MainCamera;
        [SerializeField] private protected MB_DamageCanvas m_DamageCanvas;
        [SerializeField] private protected MB_PlayerHUD m_PlayerHUD;
        [SerializeField] private protected MB_BossLifeBar m_BossLifeBar;
        [SerializeField] private protected MB_NotificationsContainer m_NotificationsContainer;
        [SerializeField] private protected MB_EnhancementList m_EnhancementList;
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
        public MB_StatsManager StatsManager { get => this.m_StatsManager; }
        public MB_EnhancementsManager EnhancementsManager { get => this.m_EnhancementsManager; }
        public MB_TransitionManager TransitionManager { get => this.m_TransitionManager; }
        public MB_EnemyIndicatorsManager EnemyIndicatorsManager { get => this.m_EnemyIndicatorsManager; }
        public MB_DissolveManager DissolveManager { get => this.m_DissolveManager; }
        #endregion

        #region UI
        public MB_MainCamera MainCamera { get => this.m_MainCamera; }
        public MB_DamageCanvas DamageCanvas { get => this.m_DamageCanvas; }
        public MB_PlayerHUD PlayerHUD { get => this.m_PlayerHUD; }
        public MB_BossLifeBar BossLifeBar { get => this.m_BossLifeBar; }
        public MB_NotificationsContainer NotificationsContainer { get => this.m_NotificationsContainer; }
        public MB_EnhancementList EnhancementList { get => this.m_EnhancementList; }
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
            this.StatsManager.ObjectsManager = this;
            this.EnhancementsManager.ObjectsManager = this;
            this.TransitionManager.ObjectsManager = this;
            this.EnemyIndicatorsManager.ObjectsManager = this;
            this.DissolveManager.ObjectsManager = this;

            this.EnhancementList.ObjectsManager = this;

            this.MainCamera.ObjectsManager = this;
            this.PlayerHUD.ObjectsManager = this;

            /*----------------------------------------------------*/

            this.Player.Initialize();

            this.AudioManager.Initialize();
            this.PauseManager.Initialize();
            this.ShockWavesManager.Initialize();
            this.FOVManager.Initialize();
            this.RoomManager.Initialize();
            this.ScreenshotManager.Initialize();
            this.StatsManager.Initialize();
            this.EnhancementsManager.Initialize();
            this.TransitionManager.Initialize();
            this.EnemyIndicatorsManager.Initialize();

            this.EnhancementList.Initialize();

            this.MainCamera.Initialize();
            this.PlayerHUD.Initialize();
            this.NotificationsContainer.Initialize();

            /*----------------------------------------------------*/

            this.Player.PostInitialize();

            this.AudioManager.PostInitialize();
            this.PauseManager.PostInitialize();
            this.ShockWavesManager.PostInitialize();
            this.FOVManager.PostInitialize();
            this.RoomManager.PostInitialize();
            this.ScreenshotManager.PostInitialize();
            this.StatsManager.PostInitialize();
            this.EnhancementsManager.PostInitialize();
            this.TransitionManager.PostInitialize();
            this.EnemyIndicatorsManager.PostInitialize();

            this.EnhancementList.PostInitialize();

            this.MainCamera.PostInitialize();
            this.PlayerHUD.PostInitialize();
            this.NotificationsContainer.PostInitialize();
        }
    }
}
