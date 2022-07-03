using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     This class forces a transform to stay at a fixed position, rotation and/or scale.
    /// </summary>
    [AddComponentMenu("More Mountains/Tools/Movement/MMStayInPlace")]
    public class MMStayInPlace : MonoBehaviour
    {
        public enum Spaces
        {
            World,
            Local
        }

        public enum UpdateModes
        {
            Update,
            FixedUpdate,
            LateUpdate
        }

        [Header("Modes")] public UpdateModes UpdateMode = UpdateModes.LateUpdate;

        public Spaces Space = Spaces.World;

        [Header("Attributes")] public bool FixedPosition = true;

        public bool FixedRotation = true;
        public bool FixedScale = true;

        [Header("Overrides")] public bool OverridePosition;

        [MMCondition("OverridePosition", true)]
        public Vector3 OverridePositionValue;

        public bool OverrideRotation;

        [MMCondition("OverrideRotation", true)]
        public Vector3 OverrideRotationValue;

        public bool OverrideScale;

        [MMCondition("OverrideScale", true)] public Vector3 OverrideScaleValue;

        protected Vector3 _initialPosition;
        protected Quaternion _initialRotation;
        protected Vector3 _initialScale;

        protected virtual void Awake()
        {
            Initialization();
        }

        protected virtual void Update()
        {
            if (UpdateMode == UpdateModes.Update) StayInPlace();
        }

        protected virtual void FixedUpdate()
        {
            if (UpdateMode == UpdateModes.FixedUpdate) StayInPlace();
        }

        protected virtual void LateUpdate()
        {
            if (UpdateMode == UpdateModes.LateUpdate) StayInPlace();
        }

        protected virtual void Initialization()
        {
            _initialPosition = Space == Spaces.World ? transform.position : transform.localPosition;
            _initialRotation = Space == Spaces.World ? transform.rotation : transform.localRotation;
            _initialScale = Space == Spaces.World ? transform.position : transform.localScale;

            if (OverridePosition) _initialPosition = OverridePositionValue;
            if (OverrideRotation) _initialRotation = Quaternion.Euler(OverrideRotationValue);
            if (OverrideScale) _initialScale = OverrideScaleValue;
        }

        protected virtual void StayInPlace()
        {
            if (Space == Spaces.World)
            {
                if (FixedPosition) transform.position = _initialPosition;
                if (FixedRotation) transform.rotation = _initialRotation;
            }
            else
            {
                if (FixedPosition) transform.localPosition = _initialPosition;
                if (FixedRotation) transform.localRotation = _initialRotation;
            }

            if (FixedScale) transform.localScale = _initialScale;
        }
    }
}