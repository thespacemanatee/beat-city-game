namespace MoreMountains.Tools
{
    /// <summary>
    ///     An event used to broadcast slider events from a MMDebugMenu
    /// </summary>
    public struct MMDebugMenuSliderEvent
    {
        public enum EventModes
        {
            FromSlider,
            SetSlider
        }

        public delegate void Delegate(string sliderEventName, float value,
            EventModes eventMode = EventModes.FromSlider);

        private static event Delegate OnEvent;

        public static void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        public static void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        public static void Trigger(string sliderEventName, float value, EventModes eventMode = EventModes.FromSlider)
        {
            OnEvent?.Invoke(sliderEventName, value, eventMode);
        }
    }
}