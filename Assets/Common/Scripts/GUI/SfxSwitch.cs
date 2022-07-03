using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    [AddComponentMenu("TopDown Engine/GUI/SfxSwitch")]
    public class SfxSwitch : MonoBehaviour
    {
        public virtual void On()
        {
            MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.UnmuteTrack,
                MMSoundManager.MMSoundManagerTracks.Sfx);
        }

        public virtual void Off()
        {
            MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.MuteTrack,
                MMSoundManager.MMSoundManagerTracks.Sfx);
        }
    }
}