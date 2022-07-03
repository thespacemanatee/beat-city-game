using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     A class used to broadcast a level to MMRadioReceiver(s), either directly or via events
    ///     It can read from pretty much any value on any class
    /// </summary>
    [MMRequiresConstantRepaint]
    public class MMRadioBroadcaster : MMMonoBehaviour
    {
        /// a delegate to handle value changes
        public delegate void OnValueChangeDelegate();

        [Header("Source")]
        /// the emitter to read the level on
        public MMPropertyEmitter Emitter;

        [Header("Destinations")]
        /// a list of receivers hardwired to this broadcaster, that will receive the level at runtime
        public MMRadioReceiver[] Receivers;

        [Header("Channel Broadcasting")]
        /// whether or not this broadcaster should use events to broadcast its level on the specified channel
        public bool BroadcastOnChannel = true;

        /// the channel to broadcast on, has to match the Channel on the target receivers
        [MMCondition("BroadcastOnChannel", true)]
        public int Channel;

        /// whether to broadcast all the time, or only when the value changes (lighter on performance, but won't "lock" the value)
        [MMCondition("BroadcastOnChannel", true)]
        public bool OnlyBroadcastOnValueChange = true;

        protected float _levelLastFrame;

        /// what to do on value change
        public OnValueChangeDelegate OnValueChange;

        /// <summary>
        ///     On Awake we initialize our emitter
        /// </summary>
        protected virtual void Awake()
        {
            Emitter.Initialization(gameObject);
        }

        /// <summary>
        ///     On Update we process our broadcast
        /// </summary>
        protected virtual void Update()
        {
            ProcessBroadcast();
        }

        /// <summary>
        ///     Broadcasts the value if needed
        /// </summary>
        protected virtual void ProcessBroadcast()
        {
            if (Emitter == null) return;

            var level = Emitter.GetLevel();

            if (level != _levelLastFrame)
            {
                // we trigger a value change event
                OnValueChange?.Invoke();

                // for each of our receivers, we set the level manually
                foreach (var receiver in Receivers) receiver?.SetLevel(level);

                // we broadcast an event
                if (BroadcastOnChannel) MMRadioLevelEvent.Trigger(Channel, level);
            }

            _levelLastFrame = level;
        }
    }

    /// <summary>
    ///     A struct event used to broadcast the level to channels
    /// </summary>
    public struct MMRadioLevelEvent
    {
        public delegate void Delegate(int channel, float level);

        private static event Delegate OnEvent;

        public static void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        public static void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        public static void Trigger(int channel, float level)
        {
            OnEvent?.Invoke(channel, level);
        }
    }
}