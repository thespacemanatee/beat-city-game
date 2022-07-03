using System;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace MoreMountains.Tools
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class MMConditionAttribute : PropertyAttribute
    {
        public string ConditionBoolean = "";
        public bool Hidden;

        public MMConditionAttribute(string conditionBoolean)
        {
            ConditionBoolean = conditionBoolean;
            Hidden = false;
        }

        public MMConditionAttribute(string conditionBoolean, bool hideInInspector)
        {
            ConditionBoolean = conditionBoolean;
            Hidden = hideInInspector;
        }
    }
}