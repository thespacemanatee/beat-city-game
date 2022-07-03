using System;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     A class defining the contents of a MMLootTable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MMLoot<T>
    {
        /// the chance percentage to display for this object to be looted. ChancePercentages are meant to be computed by the MMLootTable class
        [MMReadOnly] public float ChancePercentage;

        /// the object to return
        public T Loot;

        /// the weight attributed to this specific object in the table
        public float Weight = 1f;

        /// the computed low bound of this object's range
        public float RangeFrom { get; set; }

        /// the computed high bound of this object's range
        public float RangeTo { get; set; }
    }


    /// <summary>
    ///     a MMLoot implementation for gameobjects
    /// </summary>
    [Serializable]
    public class MMLootGameObject : MMLoot<GameObject>
    {
    }

    /// <summary>
    ///     a MMLoot implementation for strings
    /// </summary>
    [Serializable]
    public class MMLootString : MMLoot<string>
    {
    }

    /// <summary>
    ///     a MMLoot implementation for floats
    /// </summary>
    [Serializable]
    public class MMLootFloat : MMLoot<float>
    {
    }
}