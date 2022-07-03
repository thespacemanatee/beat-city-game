using System;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     An attribute to add to static methods to they can be called via the MMDebugMenu's command line
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MMDebugLogCommandAttribute : Attribute
    {
    }
}