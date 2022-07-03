using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Object = UnityEngine.Object;

namespace MoreMountains.Tools
{
    internal static class MMMonoBehaviourFieldInfo
    {
        public static Dictionary<int, List<FieldInfo>> FieldInfoList = new();

        public static int GetFieldInfo(Object target, out List<FieldInfo> fieldInfoList)
        {
            var targetType = target.GetType();
            var targetTypeHashCode = targetType.GetHashCode();

            if (!FieldInfoList.TryGetValue(targetTypeHashCode, out fieldInfoList))
            {
                var typeTree = targetType.GetBaseTypes();
                fieldInfoList = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static |
                                                           BindingFlags.Public | BindingFlags.NonPublic |
                                                           BindingFlags.NonPublic)
                    .OrderByDescending(x => typeTree.IndexOf(x.DeclaringType))
                    .ToList();
                FieldInfoList.Add(targetTypeHashCode, fieldInfoList);
            }

            return fieldInfoList.Count;
        }

        public static IList<Type> GetBaseTypes(this Type t)
        {
            var types = new List<Type>();
            while (t.BaseType != null)
            {
                types.Add(t);
                t = t.BaseType;
            }

            return types;
        }
    }
}