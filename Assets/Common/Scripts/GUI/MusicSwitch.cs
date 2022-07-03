using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    [AddComponentMenu("TopDown Engine/GUI/MusicSwitch")]
    public class MusicSwitch : MonoBehaviour
    {
        public virtual void On()
        {
            MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.UnmuteTrack,
                MMSoundManager.MMSoundManagerTracks.Music);
        }

        public virtual void Off()
        {
            MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.MuteTrack,
                MMSoundManager.MMSoundManagerTracks.Music);
        }
    }
}