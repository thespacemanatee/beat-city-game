using System;
#if UNITY_EDITOR
#endif

namespace MoreMountains.Tools
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MMHiddenPropertiesAttribute : Attribute
    {
        public string[] PropertiesNames;

        public MMHiddenPropertiesAttribute(params string[] propertiesNames)
        {
            PropertiesNames = propertiesNames;
        }
    }
}