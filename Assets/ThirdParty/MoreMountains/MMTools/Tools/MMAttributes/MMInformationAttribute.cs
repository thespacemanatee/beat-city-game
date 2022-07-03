using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoreMountains.Tools
{
    public class MMInformationAttribute : PropertyAttribute
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

        public MMInformationAttribute(string message, InformationType type, bool messageAfterProperty)
        {
            Message = message;
            if (type == InformationType.Error) Type = MessageType.Error;
            if (type == InformationType.Info) Type = MessageType.Info;
            if (type == InformationType.Warning) Type = MessageType.Warning;
            if (type == InformationType.None) Type = MessageType.None;
            MessageAfterProperty = messageAfterProperty;
        }
#else
		public MMInformationAttribute(string message, InformationType type, bool messageAfterProperty)
		{

		}
#endif
    }
}