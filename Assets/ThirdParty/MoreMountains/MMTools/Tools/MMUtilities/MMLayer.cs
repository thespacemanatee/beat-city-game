using System;
using UnityEngine;

namespace MoreMountains.Tools
{
    [Serializable]
    public class MMLayer
    {
        [SerializeField] protected int _layerIndex;

        public virtual int LayerIndex => _layerIndex;

        public virtual int Mask => 1 << _layerIndex;

        public virtual void Set(int _layerIndex)
        {
            if (_layerIndex > 0 && _layerIndex < 32) this._layerIndex = _layerIndex;
        }
    }
}