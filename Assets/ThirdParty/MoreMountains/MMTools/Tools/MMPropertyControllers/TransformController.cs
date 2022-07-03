using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     A simple class used to control properties on a transform
    /// </summary>
    [AddComponentMenu("More Mountains/Tools/Property Controllers/TransformController")]
    public class TransformController : MonoBehaviour
    {
        [Header("Position")] public bool ControlPositionX;

        [MMCondition("ControlPositionX", true)]
        public float PositionX;

        public bool ControlPositionY;

        [MMCondition("ControlPositionY", true)]
        public float PositionY;

        public bool ControlPositionZ;

        [MMCondition("ControlPositionZ", true)]
        public float PositionZ;

        [Header("Local Position")] public bool ControlLocalPositionX;

        [MMCondition("ControlLocalPositionX", true)]
        public float LocalPositionX;

        public bool ControlLocalPositionY;

        [MMCondition("ControlLocalPositionY", true)]
        public float LocalPositionY;

        public bool ControlLocalPositionZ;

        [MMCondition("ControlLocalPositionZ", true)]
        public float LocalPositionZ;

        [Header("Rotation")] public bool ControlRotationX;

        [MMCondition("ControlRotationX", true)]
        public float RotationX;

        public bool ControlRotationY;

        [MMCondition("ControlRotationY", true)]
        public float RotationY;

        public bool ControlRotationZ;

        [MMCondition("ControlRotationZ", true)]
        public float RotationZ;

        [Header("Local Rotation")] public bool ControlLocalRotationX;

        [MMCondition("ControlLocalRotationX", true)]
        public float LocalRotationX;

        public bool ControlLocalRotationY;

        [MMCondition("ControlLocalRotationY", true)]
        public float LocalRotationY;

        public bool ControlLocalRotationZ;

        [MMCondition("ControlLocalRotationZ", true)]
        public float LocalRotationZ;

        [Header("Scale")] public bool ControlScaleX;

        [MMCondition("ControlScaleX", true)] public float ScaleX;

        public bool ControlScaleY;

        [MMCondition("ControlScaleY", true)] public float ScaleY;

        public bool ControlScaleZ;

        [MMCondition("ControlScaleZ", true)] public float ScaleZ;

        protected Vector3 _localPosition;
        protected Vector3 _localRotation;

        protected Vector3 _position;
        protected Vector3 _rotation;
        protected Vector3 _scale;

        /// <summary>
        ///     At update, modifies the requested properties
        /// </summary>
        protected virtual void Update()
        {
            _position = transform.position;
            _localPosition = transform.localPosition;
            _rotation = transform.eulerAngles;
            _localRotation = transform.localEulerAngles;
            _scale = transform.localScale;

            if (ControlPositionX)
            {
                _position.x = PositionX;
                transform.position = _position;
            }

            if (ControlPositionY)
            {
                _position.y = PositionY;
                transform.position = _position;
            }

            if (ControlPositionZ)
            {
                _position.z = PositionZ;
                transform.position = _position;
            }

            if (ControlLocalPositionX)
            {
                _localPosition.x = LocalPositionX;
                transform.localPosition = _localPosition;
            }

            if (ControlLocalPositionY)
            {
                _localPosition.y = LocalPositionY;
                transform.localPosition = _localPosition;
            }

            if (ControlLocalPositionZ)
            {
                _localPosition.z = LocalPositionZ;
                transform.localPosition = _localPosition;
            }

            if (ControlRotationX)
            {
                _rotation.x = RotationX;
                transform.eulerAngles = _rotation;
            }

            if (ControlRotationY)
            {
                _rotation.y = RotationY;
                transform.eulerAngles = _rotation;
            }

            if (ControlRotationZ)
            {
                _rotation.z = RotationZ;
                transform.eulerAngles = _rotation;
            }

            if (ControlLocalRotationX)
            {
                _localRotation.x = LocalRotationX;
                transform.localEulerAngles = _localRotation;
            }

            if (ControlLocalRotationY)
            {
                _localRotation.y = LocalRotationY;
                transform.localEulerAngles = _localRotation;
            }

            if (ControlLocalRotationZ)
            {
                _localRotation.z = LocalRotationZ;
                transform.localEulerAngles = _localRotation;
            }

            if (ControlScaleX)
            {
                _scale.x = ScaleX;
                transform.localScale = _scale;
            }

            if (ControlScaleY)
            {
                _scale.y = ScaleY;
                transform.localScale = _scale;
            }

            if (ControlScaleZ)
            {
                _scale.z = ScaleZ;
                transform.localScale = _scale;
            }
        }
    }
}