using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Utils {
    [Serializable]
    public class C_WeightedObject<T> {
        #region Members
        [SerializeField] private protected float m_Weight;
        [SerializeField] private protected T m_Obj;
        #endregion

        #region Getters / Setters
        public float Weight { get => this.m_Weight; set => this.m_Weight = value; }
        public T Obj { get => this.m_Obj; set => this.m_Obj = value; }
        #endregion
    }
}
