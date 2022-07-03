using MoreMountains.FeedbacksForThirdParty;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    public class AutoBindAutoFocus : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        /// the AutoFocus component on the camera
        public MMAutoFocus AutoFocus { get; set; }

        protected virtual void Start()
        {
            AutoFocus = FindObjectOfType<MMAutoFocus>();
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening();
        }

        protected virtual void OnDisable()
        {
            this.MMEventStopListening();
        }

        public virtual void OnMMEvent(MMCameraEvent cameraEvent)
        {
            switch (cameraEvent.EventType)
            {
                case MMCameraEventTypes.StartFollowing:
                    if (AutoFocus == null) AutoFocus = FindObjectOfType<MMAutoFocus>();
                    if (AutoFocus != null)
                    {
                        AutoFocus.FocusTargets = new Transform[1];
                        AutoFocus.FocusTargets[0] = LevelManager.Instance.Players[0].transform;
                    }

                    break;
            }
        }
    }
}