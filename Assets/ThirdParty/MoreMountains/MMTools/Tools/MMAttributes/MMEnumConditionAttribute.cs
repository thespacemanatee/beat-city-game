using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace MoreMountains.Tools
{
    /// <summary>
    ///     An attribute to conditionnally hide fields based on the current selection in an enum.
    ///     Usage :  [MMEnumCondition("rotationMode", (int)RotationMode.LookAtTarget, (int)RotationMode.RotateToAngles)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class MMEnumConditionAttribute : PropertyAttribute
    {
        private readonly BitArray bitArray = new(32);
        public string ConditionEnum = "";
        public bool Hidden;

        public MMEnumConditionAttribute(string conditionBoolean, params int[] enumValues)
        {
            ConditionEnum = conditionBoolean;
            Hidden = true;

            for (var i = 0; i < enumValues.Length; i++) bitArray.Set(enumValues[i], true);
        }

        public bool ContainsBitFlag(int enumValue)
        {
            return bitArray.Get(enumValue);
        }
    }
}