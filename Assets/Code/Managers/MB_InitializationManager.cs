using System;
using Code.Characters.Players;
using Code.Map;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Managers {
    public class MB_InitializationManager : MonoBehaviour {
        #region Members
        [Foldout("MB_PlayerManager", true)]
        [SerializeField] private protected E_Biome m_Biome;

        [SerializeField] private protected AMB_Player m_Player;
        [SerializeField] private protected MB_ObjectsManager m_ObjectsManager;

        [SerializeField] private protected Material m_CRTMaterial;
        #endregion

        #region Getters / Setters
        private E_Biome Biome { get => this.m_Biome; }

        private AMB_Player Player { get => this.m_Player; }
        private MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; }

        private Material CRTMaterial { get => this.m_CRTMaterial; }
        #endregion

        #region Static / Readonly / Const
        public static E_Biome BIOME = E_Biome.None;
        private static readonly int UNSCALED_TIME = Shader.PropertyToID("_UnscaledTime");
        #endregion

        #region Unity methods
        private void Awake() {
            DOTween.Init(true, true, LogBehaviour.Default).SetCapacity(2000, 1250);

            BIOME = BIOME != E_Biome.None
                ? BIOME
                : this.Biome;

            this.ObjectsManager.Player = this.Player;
            this.ObjectsManager.Initialize();
            this.ObjectsManager.PlayerHUD.SetClass(E_Class.Knight);
        }

        private void Update() {
            this.CRTMaterial.SetFloat(UNSCALED_TIME, Time.unscaledTime);
        }
        #endregion
    }
}
