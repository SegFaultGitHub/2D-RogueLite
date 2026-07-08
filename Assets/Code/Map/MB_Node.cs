using System;
using MyBox;
using UnityEngine;

namespace Code.Map {
    public class MB_Node : MonoBehaviour {
        #region Members
        [Foldout("MB_Node", true)]
        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected MB_Cell m_Cell;
        [ReadOnly][SerializeField] private protected Vector3 m_WorldPosition;
        #endregion

        #region Getters / Setters
        public MB_Cell Cell { get => this.m_Cell; private set => this.m_Cell = value; }
        public Vector3 WorldPosition { get => this.m_WorldPosition; private set => this.m_WorldPosition = value; }
        #endregion

        #region Static / Readonly / Const
        #endregion

        #region Unity methods
        private void Awake() {
            this.Cell = this.GetComponentInParent<MB_Cell>();
            this.WorldPosition = this.transform.position;
        }
        #endregion
    }
}
