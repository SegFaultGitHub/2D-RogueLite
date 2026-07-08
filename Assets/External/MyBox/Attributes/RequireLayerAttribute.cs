using System;
using JetBrains.Annotations;
using UnityEngine;

namespace MyBox {
    [AttributeUsage(AttributeTargets.Class), PublicAPI]
    public class RequireLayerAttribute : Attribute {
        public readonly string LayerName;
        public readonly int LayerIndex = -1;

        public int RequiredLayerIndex =>
            this.LayerName != null
                ? LayerMask.NameToLayer(this.LayerName)
                : this.LayerIndex;

        public RequireLayerAttribute(string layer) => this.LayerName = layer;
        public RequireLayerAttribute(int layer) => this.LayerIndex = layer;
    }
}
