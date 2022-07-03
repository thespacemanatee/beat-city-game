using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     this feedback will let you animate the position of
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback will animate the target object's position over time, for the specified duration, from the chosen initial position to the chosen destination. These can either be relative Vector3 offsets from the Feedback's position, or Transforms. If you specify transforms, the Vector3 values will be ignored.")]
    [FeedbackPath("Transform/Position")]
    public class MMF_Position : MMF_Feedback
    {
        public enum Modes
        {
            AtoB,
            AlongCurve,
            ToDestination
        }

        public enum Spaces
        {
            World,
            Local,
            RectTransform
        }

        public enum TimeScales
        {
            Scaled,
            Unscaled
        }

        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        protected Coroutine _coroutine;
        protected Vector3 _currentPosition;
        protected Vector3 _destinationPosition;
        protected Vector3 _initialPosition;

        protected Vector3 _newPosition;
        protected RectTransform _rectTransform;
        protected Vector3 _workInitialPosition;

        /// if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over
        [Tooltip(
            "if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
        public bool AllowAdditivePlays = false;

        /// the acceleration of the movement
        [Tooltip("the acceleration of the movement")]
        [MMFEnumCondition("Mode", (int)Modes.AtoB, (int)Modes.ToDestination)]
        public AnimationCurve AnimatePositionCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));

        /// the acceleration of the movement
        [Tooltip("the acceleration of the movement")] [MMFCondition("AnimateX", true)]
        public AnimationCurve AnimatePositionCurveX = new(new Keyframe(0, 0), new Keyframe(0.3f, 1f),
            new Keyframe(0.6f, -1f), new Keyframe(1, 0f));

        /// the acceleration of the movement
        [Tooltip("the acceleration of the movement")] [MMFCondition("AnimateY", true)]
        public AnimationCurve AnimatePositionCurveY = new(new Keyframe(0, 0), new Keyframe(0.3f, 1f),
            new Keyframe(0.6f, -1f), new Keyframe(1, 0f));

        /// the acceleration of the movement
        [Tooltip("the acceleration of the movement")] [MMFCondition("AnimateZ", true)]
        public AnimationCurve AnimatePositionCurveZ = new(new Keyframe(0, 0), new Keyframe(0.3f, 1f),
            new Keyframe(0.6f, -1f), new Keyframe(1, 0f));

        /// the duration of the animation on play
        [Tooltip("the duration of the animation on play")]
        public float AnimatePositionDuration = 0.2f;

        [MMFInspectorGroup("Position Target", true, 61, true)]
        /// the object this feedback will animate the position for
        [Tooltip("the object this feedback will animate the position for")]
        public GameObject AnimatePositionTarget;

        /// if this is true, the x position will be animated
        [Tooltip("if this is true, the x position will be animated")] [MMFEnumCondition("Mode", (int)Modes.AlongCurve)]
        public bool AnimateX;

        /// if this is true, the y position will be animated
        [Tooltip("if this is true, the y position will be animated")] [MMFEnumCondition("Mode", (int)Modes.AlongCurve)]
        public bool AnimateY;

        /// if this is true, the z position will be animated
        [Tooltip("if this is true, the z position will be animated")] [MMFEnumCondition("Mode", (int)Modes.AlongCurve)]
        public bool AnimateZ;

        /// the destination position
        [Tooltip("the destination position")] [MMFEnumCondition("Mode", (int)Modes.AtoB, (int)Modes.ToDestination)]
        public Vector3 DestinationPosition = Vector3.one;

        /// the destination transform - if set, takes precedence over the Vector3 above
        [Tooltip("the destination transform - if set, takes precedence over the Vector3 above")]
        [MMFEnumCondition("Mode", (int)Modes.AtoB, (int)Modes.ToDestination)]
        public Transform DestinationPositionTransform;

        /// if this is true, initial and destination positions will be recomputed on every play
        [Tooltip("if this is true, initial and destination positions will be recomputed on every play")]
        public bool DeterminePositionsOnPlay = false;

        /// the initial position
        [Tooltip("the initial position")] [MMFEnumCondition("Mode", (int)Modes.AtoB, (int)Modes.AlongCurve)]
        public Vector3 InitialPosition = Vector3.zero;

        /// the initial transform - if set, takes precedence over the Vector3 above
        [Tooltip("the initial transform - if set, takes precedence over the Vector3 above")]
        [MMFEnumCondition("Mode", (int)Modes.AtoB, (int)Modes.AlongCurve)]
        public Transform InitialPositionTransform;

        [MMFInspectorGroup("Transition", true, 63)]
        /// the mode this animation should follow (either going from A to B, or moving along a curve)
        [Tooltip("the mode this animation should follow (either going from A to B, or moving along a curve)")]
        public Modes Mode = Modes.AtoB;

        [MMFInspectorGroup("Positions", true, 64)]
        /// if this is true, the initial position won't be added to init and destination
        [Tooltip("if this is true, the initial position won't be added to init and destination")]
        public bool RelativePosition = true;

        /// the value to remap the curve's 1 value to
        [Tooltip("the value to remap the curve's 1 value to")]
        [MMFEnumCondition("Mode", (int)Modes.AlongCurve)]
        [FormerlySerializedAs("CurveMultiplier")]
        public float RemapCurveOne = 1f;

        /// the value to remap the curve's 0 value to
        [MMFEnumCondition("Mode", (int)Modes.AlongCurve)] [Tooltip("the value to remap the curve's 0 value to")]
        public float RemapCurveZero = 0f;

        /// the space in which to move the position in
        [Tooltip("the space in which to move the position in")]
        public Spaces Space = Spaces.World;

        /// the duration of this feedback is the duration of its animation
        public override float FeedbackDuration
        {
            get => ApplyTimeMultiplier(AnimatePositionDuration);
            set => AnimatePositionDuration = value;
        }

        /// <summary>
        ///     On init, we set our initial and destination positions (transform will take precedence over vector3s)
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);
            if (Active)
            {
                if (AnimatePositionTarget == null)
                {
                    Debug.LogWarning("The animate position target for " + this +
                                     " is null, you have to define it in the inspector");
                    return;
                }

                if (Space == Spaces.RectTransform) _rectTransform = AnimatePositionTarget.GetComponent<RectTransform>();

                if (!DeterminePositionsOnPlay) DeterminePositions();
            }
        }

        protected virtual void DeterminePositions()
        {
            if (DeterminePositionsOnPlay && RelativePosition && InitialPosition != Vector3.zero) return;

            if (Mode != Modes.ToDestination)
            {
                if (InitialPositionTransform != null)
                    _workInitialPosition = GetPosition(InitialPositionTransform);
                else
                    _workInitialPosition = RelativePosition
                        ? GetPosition(AnimatePositionTarget.transform) + InitialPosition
                        : GetPosition(AnimatePositionTarget.transform);
                if (DestinationPositionTransform != null)
                    DestinationPosition = GetPosition(DestinationPositionTransform);
                else
                    DestinationPosition = RelativePosition
                        ? GetPosition(AnimatePositionTarget.transform) + DestinationPosition
                        : DestinationPosition;
            }
        }

        /// <summary>
        ///     On Play, we move our object from A to B
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized || AnimatePositionTarget == null) return;

            if (Active || Owner.AutoPlayOnEnable)
            {
                if (DeterminePositionsOnPlay && NormalPlayDirection) DeterminePositions();

                switch (Mode)
                {
                    case Modes.ToDestination:
                        _initialPosition = GetPosition(AnimatePositionTarget.transform);
                        _destinationPosition =
                            RelativePosition ? _initialPosition + DestinationPosition : DestinationPosition;
                        if (DestinationPositionTransform != null)
                            _destinationPosition = GetPosition(DestinationPositionTransform);
                        _coroutine = Owner.StartCoroutine(MoveFromTo(AnimatePositionTarget, _initialPosition,
                            _destinationPosition, FeedbackDuration, AnimatePositionCurve));
                        break;
                    case Modes.AtoB:
                        if (!AllowAdditivePlays && _coroutine != null) return;
                        _coroutine = Owner.StartCoroutine(MoveFromTo(AnimatePositionTarget, _workInitialPosition,
                            DestinationPosition, FeedbackDuration, AnimatePositionCurve));
                        break;
                    case Modes.AlongCurve:
                        if (!AllowAdditivePlays && _coroutine != null) return;
                        var intensityMultiplier = Timing.ConstantIntensity ? 1f : feedbacksIntensity;
                        _coroutine = Owner.StartCoroutine(MoveAlongCurve(AnimatePositionTarget, _workInitialPosition,
                            FeedbackDuration, intensityMultiplier));
                        break;
                }
            }
        }

        /// <summary>
        ///     Moves the object along a curve
        /// </summary>
        /// <param name="movingObject"></param>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <param name="duration"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        protected virtual IEnumerator MoveAlongCurve(GameObject movingObject, Vector3 initialPosition, float duration,
            float intensityMultiplier)
        {
            IsPlaying = true;
            var journey = NormalPlayDirection ? 0f : duration;
            while (journey >= 0 && journey <= duration && duration > 0)
            {
                var percent = Mathf.Clamp01(journey / duration);

                ComputeNewCurvePosition(movingObject, initialPosition, percent, intensityMultiplier);

                journey += NormalPlayDirection ? FeedbackDeltaTime : -FeedbackDeltaTime;
                yield return null;
            }

            ComputeNewCurvePosition(movingObject, initialPosition, FinalNormalizedTime, intensityMultiplier);
            _coroutine = null;
            IsPlaying = false;
        }

        /// <summary>
        ///     Evaluates the position curves and computes the new position
        /// </summary>
        /// <param name="movingObject"></param>
        /// <param name="initialPosition"></param>
        /// <param name="percent"></param>
        protected virtual void ComputeNewCurvePosition(GameObject movingObject, Vector3 initialPosition, float percent,
            float intensityMultiplier)
        {
            var newValueX = AnimatePositionCurveX.Evaluate(percent);
            var newValueY = AnimatePositionCurveY.Evaluate(percent);
            var newValueZ = AnimatePositionCurveZ.Evaluate(percent);

            newValueX = MMFeedbacksHelpers.Remap(newValueX, 0f, 1f, RemapCurveZero * intensityMultiplier,
                RemapCurveOne * intensityMultiplier);
            newValueY = MMFeedbacksHelpers.Remap(newValueY, 0f, 1f, RemapCurveZero * intensityMultiplier,
                RemapCurveOne * intensityMultiplier);
            newValueZ = MMFeedbacksHelpers.Remap(newValueZ, 0f, 1f, RemapCurveZero * intensityMultiplier,
                RemapCurveOne * intensityMultiplier);

            _newPosition = initialPosition;
            _currentPosition = GetPosition(movingObject.transform);

            if (RelativePosition)
            {
                _newPosition.x = AnimateX ? initialPosition.x + newValueX : _currentPosition.x;
                _newPosition.y = AnimateY ? initialPosition.y + newValueY : _currentPosition.y;
                _newPosition.z = AnimateZ ? initialPosition.z + newValueZ : _currentPosition.z;
            }
            else
            {
                _newPosition.x = AnimateX ? newValueX : _currentPosition.x;
                _newPosition.y = AnimateY ? newValueY : _currentPosition.y;
                _newPosition.z = AnimateZ ? newValueZ : _currentPosition.z;
            }

            SetPosition(movingObject.transform, _newPosition);
        }

        /// <summary>
        ///     Moves an object from point A to point B in a given time
        /// </summary>
        /// <param name="movingObject">Moving object.</param>
        /// <param name="pointA">Point a.</param>
        /// <param name="pointB">Point b.</param>
        /// <param name="duration">Time.</param>
        protected virtual IEnumerator MoveFromTo(GameObject movingObject, Vector3 pointA, Vector3 pointB,
            float duration, AnimationCurve curve = null)
        {
            IsPlaying = true;
            var journey = NormalPlayDirection ? 0f : duration;
            while (journey >= 0 && journey <= duration && duration > 0)
            {
                var percent = Mathf.Clamp01(journey / duration);

                _newPosition = Vector3.LerpUnclamped(pointA, pointB, curve.Evaluate(percent));

                SetPosition(movingObject.transform, _newPosition);

                journey += NormalPlayDirection ? FeedbackDeltaTime : -FeedbackDeltaTime;
                yield return null;
            }

            // set final position
            if (NormalPlayDirection)
                SetPosition(movingObject.transform, pointB);
            else
                SetPosition(movingObject.transform, pointA);
            _coroutine = null;
            IsPlaying = false;
        }

        /// <summary>
        ///     Gets the world, local or anchored position
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected virtual Vector3 GetPosition(Transform target)
        {
            switch (Space)
            {
                case Spaces.World:
                    return target.position;
                case Spaces.Local:
                    return target.localPosition;
                case Spaces.RectTransform:
                    return target.gameObject.GetComponent<RectTransform>().anchoredPosition;
            }

            return Vector3.zero;
        }

        /// <summary>
        ///     Sets the position, localposition or anchoredposition of the target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="newPosition"></param>
        protected virtual void SetPosition(Transform target, Vector3 newPosition)
        {
            switch (Space)
            {
                case Spaces.World:
                    target.position = newPosition;
                    break;
                case Spaces.Local:
                    target.localPosition = newPosition;
                    break;
                case Spaces.RectTransform:
                    _rectTransform.anchoredPosition = newPosition;
                    break;
            }
        }

        /// <summary>
        ///     On stop, we interrupt movement if it was active
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized || _coroutine == null) return;
            IsPlaying = false;
            Owner.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        /// <summary>
        ///     On disable we reset our coroutine
        /// </summary>
        protected virtual void OnDisable()
        {
            _coroutine = null;
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.TransformColor; }
        }

        public override bool EvaluateRequiresSetup()
        {
            return AnimatePositionTarget == null;
        }

        public override string RequiredTargetText => AnimatePositionTarget != null ? AnimatePositionTarget.name : "";
        public override string RequiresSetupText =>
            "This feedback requires that a AnimatePositionTarget and a Destination be set to be able to work properly. You can set one below.";
#endif
    }
}