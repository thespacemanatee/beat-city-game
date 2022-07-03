using System;
using UnityEngine;

namespace MoreMountains.Tools
{
    public enum MMTweenDefinitionTypes
    {
        MMTween,
        AnimationCurve
    }

    [Serializable]
    public class MMTweenType
    {
        public MMTweenDefinitionTypes MMTweenDefinitionType = MMTweenDefinitionTypes.MMTween;
        public MMTween.MMTweenCurve MMTweenCurve = MMTween.MMTweenCurve.EaseInCubic;
        public AnimationCurve Curve = new(new Keyframe(0, 0), new Keyframe(1, 1f));

        public MMTweenType(MMTween.MMTweenCurve newCurve)
        {
            MMTweenCurve = newCurve;
            MMTweenDefinitionType = MMTweenDefinitionTypes.MMTween;
        }

        public MMTweenType(AnimationCurve newCurve)
        {
            Curve = newCurve;
            MMTweenDefinitionType = MMTweenDefinitionTypes.AnimationCurve;
        }
    }
}