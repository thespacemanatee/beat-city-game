namespace MoreMountains.Tools
{
    /// <summary>
    ///     An event used to broadcast button events from a MMDebugMenu
    /// </summary>
    public struct MMDebugMenuButtonEvent
    {
        public enum EventModes
        {
            FromButton,
            SetButton
        }

        public delegate void Delegate(string buttonEventName, bool active = true,
            EventModes eventMode = EventModes.FromButton);

        private static event Delegate OnEvent;

        public static void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        public static void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        public static void Trigger(string buttonEventName, bool active = true,
            EventModes eventMode = EventModes.FromButton)
        {
            OnEvent?.Invoke(buttonEventName, active, eventMode);
        }
    }
}