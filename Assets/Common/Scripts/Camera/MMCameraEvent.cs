using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    public enum MMCameraEventTypes
    {
        SetTargetCharacter,
        SetConfiner,
        StartFollowing,
        StopFollowing,
        RefreshPosition,
        ResetPriorities
    }

    public struct MMCameraEvent
    {
        public MMCameraEventTypes EventType;
        public Character TargetCharacter;
        public Collider Bounds;

        public MMCameraEvent(MMCameraEventTypes eventType, Character targetCharacter = null, Collider bounds = null)
        {
            EventType = eventType;
            TargetCharacter = targetCharacter;
            Bounds = bounds;
        }

        private static MMCameraEvent e;

        public static void Trigger(MMCameraEventTypes eventType, Character targetCharacter = null,
            Collider bounds = null)
        {
            e.EventType = eventType;
            e.Bounds = bounds;
            e.TargetCharacter = targetCharacter;
            MMEventManager.TriggerEvent(e);
        }
    }
}