using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Tools
{
    public class MMAudioEvents
    {
    }

    /// <summary>
    ///     A struct used to trigger sounds
    /// </summary>
    public struct MMSfxEvent
    {
        public delegate void Delegate(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f,
            float pitch = 1f);

        private static event Delegate OnEvent;

        public static void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        public static void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        public static void Trigger(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f,
            float pitch = 1f)
        {
            OnEvent?.Invoke(clipToPlay, audioGroup, volume, pitch);
        }
    }
}