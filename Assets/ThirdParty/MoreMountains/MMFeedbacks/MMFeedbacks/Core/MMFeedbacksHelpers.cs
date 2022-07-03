using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    public class MMFeedbacksHelpers : MonoBehaviour
    {
        /// <summary>
        ///     Remaps a value x in interval [A,B], to the proportional value in interval [C,D]
        /// </summary>
        /// <param name="x">The value to remap.</param>
        /// <param name="A">the minimum bound of interval [A,B] that contains the x value</param>
        /// <param name="B">the maximum bound of interval [A,B] that contains the x value</param>
        /// <param name="C">the minimum bound of target interval [C,D]</param>
        /// <param name="D">the maximum bound of target interval [C,D]</param>
        public static float Remap(float x, float A, float B, float C, float D)
        {
            var remappedValue = C + (x - A) / (B - A) * (D - C);
            return remappedValue;
        }
    }

    public class MMFReadOnlyAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class MMFInspectorButtonAttribute : PropertyAttribute
    {
        public readonly string MethodName;

        public MMFInspectorButtonAttribute(string MethodName)
        {
            this.MethodName = MethodName;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class MMFEnumConditionAttribute : PropertyAttribute
    {
        private readonly BitArray bitArray = new(32);
        public string ConditionEnum = "";
        public bool Hidden;

        public MMFEnumConditionAttribute(string conditionBoolean, params int[] enumValues)
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

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MMFInspectorButtonAttribute))]
    public class MMFInspectorButtonPropertyDrawer : PropertyDrawer
    {
        private MethodInfo _eventMethodInfo;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var inspectorButtonAttribute = (MMFInspectorButtonAttribute)attribute;

            var buttonLength = position.width;
            var buttonRect = new Rect(position.x + (position.width - buttonLength) * 0.5f, position.y, buttonLength,
                position.height);

            if (GUI.Button(buttonRect, inspectorButtonAttribute.MethodName))
            {
                var eventOwnerType = prop.serializedObject.targetObject.GetType();
                var eventName = inspectorButtonAttribute.MethodName;

                if (_eventMethodInfo == null)
                    _eventMethodInfo = eventOwnerType.GetMethod(eventName,
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (_eventMethodInfo != null)
                    _eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
                else
                    Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName,
                        eventOwnerType));
            }
        }
    }
#endif

    public class MMFInformationAttribute : PropertyAttribute
    {
        public enum InformationType
        {
            Error,
            Info,
            None,
            Warning
        }

#if UNITY_EDITOR
        public string Message;
        public MessageType Type;
        public bool MessageAfterProperty;

        public MMFInformationAttribute(string message, InformationType type, bool messageAfterProperty)
        {
            Message = message;
            if (type == InformationType.Error) Type = MessageType.Error;
            if (type == InformationType.Info) Type = MessageType.Info;
            if (type == InformationType.Warning) Type = MessageType.Warning;
            if (type == InformationType.None) Type = MessageType.None;
            MessageAfterProperty = messageAfterProperty;
        }
#else
		public MMFInformationAttribute(string message, InformationType type, bool messageAfterProperty)
		{

		}
#endif
    }

    public class MMFHiddenAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class MMFConditionAttribute : PropertyAttribute
    {
        public string ConditionBoolean = "";
        public bool Hidden;

        public MMFConditionAttribute(string conditionBoolean)
        {
            ConditionBoolean = conditionBoolean;
            Hidden = false;
        }

        public MMFConditionAttribute(string conditionBoolean, bool hideInInspector)
        {
            ConditionBoolean = conditionBoolean;
            Hidden = hideInInspector;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MMFHiddenPropertiesAttribute : Attribute
    {
        public string[] PropertiesNames;

        public MMFHiddenPropertiesAttribute(params string[] propertiesNames)
        {
            PropertiesNames = propertiesNames;
        }
    }

    /// <summary>
    ///     An attribute used to group inspector fields under common dropdowns
    ///     Implementation inspired by Rodrigo Prinheiro's work, available at
    ///     https://github.com/RodrigoPrinheiro/unityFoldoutAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class MMFInspectorGroupAttribute : PropertyAttribute
    {
        public bool ClosedByDefault;
        public bool GroupAllFieldsUntilNextGroupAttribute;
        public int GroupColorIndex;
        public string GroupName;
        public bool RequiresSetup;

        public MMFInspectorGroupAttribute(string groupName, bool groupAllFieldsUntilNextGroupAttribute = false,
            int groupColorIndex = 24, bool requiresSetup = false, bool closedByDefault = false)
        {
            if (groupColorIndex > 139) groupColorIndex = 139;

            GroupName = groupName;
            GroupAllFieldsUntilNextGroupAttribute = groupAllFieldsUntilNextGroupAttribute;
            GroupColorIndex = groupColorIndex;
            RequiresSetup = requiresSetup;
            ClosedByDefault = closedByDefault;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TmpAttribute : PropertyAttribute
    {
        /// <summary>
        ///     <para>The header text.</para>
        /// </summary>
        /// <footer>
        ///     <a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=HeaderAttribute.header">
        ///         `HeaderAttribute.header`
        ///         on docs.unity3d.com
        ///     </a>
        /// </footer>
        public readonly string header;

        /// <summary>
        ///     <para>Add a header above some fields in the Inspector.</para>
        /// </summary>
        /// <param name="header">The header text.</param>
        /// <footer>
        ///     <a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=HeaderAttribute">
        ///         `HeaderAttribute`
        ///         on docs.unity3d.com
        ///     </a>
        /// </footer>
        public TmpAttribute(string header)
        {
            this.header = header;
        }
    }

    public static class MMFeedbackStaticMethods
    {
        private static readonly List<Component> m_ComponentCache = new();

        /// <summary>
        ///     Grabs a component without allocating memory uselessly
        /// </summary>
        /// <param name="this"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static Component GetComponentNoAlloc(this GameObject @this, Type componentType)
        {
            @this.GetComponents(componentType, m_ComponentCache);
            var component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
            m_ComponentCache.Clear();
            return component;
        }

        public static Type MMFGetTypeByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetTypes())
                if (type.Name == name)
                    return type;

            return null;
        }

        /// <summary>
        ///     Grabs a component without allocating memory uselessly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T MMFGetComponentNoAlloc<T>(this GameObject @this) where T : Component
        {
            @this.GetComponents(typeof(T), m_ComponentCache);
            var component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
            m_ComponentCache.Clear();
            return component as T;
        }

#if UNITY_EDITOR
        /// <summary>
        ///     Returns the object value of a target serialized property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object MMFGetObjectValue(this SerializedProperty property)
        {
            if (property == null) return null;

            var propertyPath = property.propertyPath.Replace(".Array.data[", "[");
            object targetObject = property.serializedObject.targetObject;
            var elements = propertyPath.Split('.');
            foreach (var element in elements)
                if (!element.Contains("["))
                {
                    targetObject = MMFGetPropertyValue(targetObject, element);
                }
                else
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var elementIndex =
                        Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    targetObject = MMFGetPropertyValue(targetObject, elementName, elementIndex);
                }

            return targetObject;
        }

        private static object MMFGetPropertyValue(object source, string propertyName)
        {
            if (source == null) return null;

            var propertyType = source.GetType();

            while (propertyType != null)
            {
                var fieldInfo = propertyType.GetField(propertyName,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (fieldInfo != null) return fieldInfo.GetValue(source);
                var propertyInfo = propertyType.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (propertyInfo != null) return propertyInfo.GetValue(source, null);
                propertyType = propertyType.BaseType;
            }

            return null;
        }

        private static object MMFGetPropertyValue(object source, string propertyName, int index)
        {
            var enumerable = MMFGetPropertyValue(source, propertyName) as IEnumerable;
            if (enumerable == null) return null;
            var enumerator = enumerable.GetEnumerator();
            for (var i = 0; i <= index; i++)
                if (!enumerator.MoveNext())
                    return null;
            return enumerator.Current;
        }
#endif
    }

    /// <summary>
    ///     Atttribute used to mark feedback class.
    ///     The provided path is used to sort the feedback list displayed in the feedback manager dropdown
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FeedbackPathAttribute : Attribute
    {
        public string Name;
        public string Path;

        public FeedbackPathAttribute(string path)
        {
            Path = path;
            Name = path.Split('/').Last();
        }

        public static string GetFeedbackDefaultName(Type type)
        {
            var attribute = type.GetCustomAttributes(false).OfType<FeedbackPathAttribute>().FirstOrDefault();
            return attribute != null ? attribute.Name : type.Name;
        }

        public static string GetFeedbackDefaultPath(Type type)
        {
            var attribute = type.GetCustomAttributes(false).OfType<FeedbackPathAttribute>().FirstOrDefault();
            return attribute != null ? attribute.Path : type.Name;
        }
    }

    /// <summary>
    ///     Atttribute used to mark feedback class.
    ///     The contents allow you to specify a help text for each feedback
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FeedbackHelpAttribute : Attribute
    {
        public string HelpText;

        public FeedbackHelpAttribute(string helpText)
        {
            HelpText = helpText;
        }

        public static string GetFeedbackHelpText(Type type)
        {
            var attribute = type.GetCustomAttributes(false).OfType<FeedbackHelpAttribute>().FirstOrDefault();
            return attribute != null ? attribute.HelpText : "";
        }
    }

    public static class MMF_FieldInfo
    {
        public static Dictionary<int, List<FieldInfo>> FieldInfoList = new();


        public static int GetFieldInfo(MMF_Feedback target, out List<FieldInfo> fieldInfoList)
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