using System;
using System.Collections.Generic;
using Code.Characters;
using Code.Map;
using Code.Utils;
using DG.Tweening;
using MyBox;
using UnityEngine;

namespace Code.Managers {
    public class MB_RoomManager : MonoBehaviour {
        #region Members
        [Foldout("MB_RoomManager", true)]
        [SerializeField] private protected List<MB_Room> m_Rooms;
        [SerializeField] private protected Transform m_RoomParent;
        [SerializeField] private protected MB_PlayerSpawner m_PlayerSpawner;

        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_ObjectsManager m_ObjectsManager;
        [ReadOnly][SerializeField] private protected MB_Room m_Room;
        #endregion

        #region Getters / Setters
        private List<MB_Room> Rooms { get => this.m_Rooms; }
        private Transform RoomParent { get => this.m_RoomParent; }
        private MB_PlayerSpawner PlayerSpawner { get => this.m_PlayerSpawner; }

        public MB_ObjectsManager ObjectsManager { get => this.m_ObjectsManager; set => this.m_ObjectsManager = value; }
        public MB_Room Room { get => this.m_Room; private set => this.m_Room = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        #endregion

        public void Initialize() { }

        public void PostInitialize() {
            this.NextRoom();
        }

        [ButtonMethod]
        public void NextRoom() {
            this.ObjectsManager.MainCamera.SetPixelSize(32, 2f, MB_TransitionManager.HIDE_EASE);
            this.ObjectsManager.TransitionManager.Run(
                realTime: false,
                waitTime: 1f,
                duration: 1f,
                action2: () => {
                    if (this.Room != null) Destroy(this.Room.gameObject);
                    this.ObjectsManager.DamageCanvas.DestroyAll();
                    for (int i = this.ObjectsManager.SpellsTransform.transform.childCount - 1; i >= 0; i--) {
                        Destroy(this.ObjectsManager.SpellsTransform.transform.GetChild(i).gameObject);
                    }
                    this.Room = Instantiate(this.Rooms[0], this.RoomParent);
                    this.Rooms.RemoveAt(0);
                    // this.Room = Instantiate(SC_Utils.Sample(this.Rooms), this.RoomParent);
                    this.ObjectsManager.Player.transform.position = this.Room.SpawnCell.transform.position;
                    this.ObjectsManager.Player.Hide();
                },
                action3: () => {
                    this.ObjectsManager.MainCamera.SetPixelSize(8, 1f, MB_TransitionManager.SHOW_EASE);
                    this.Room.ShowMap();
                },
                action4: () => {
                    MB_PlayerSpawner playerSpawner = Instantiate(this.PlayerSpawner);
                    playerSpawner.transform.position = this.Room.SpawnCell.transform.position;
                }
            );
        }
    }
}
