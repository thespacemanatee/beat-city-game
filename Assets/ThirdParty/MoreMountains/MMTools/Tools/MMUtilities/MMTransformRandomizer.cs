using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     Add this component to an object to randomize its position/rotation/scale on demand or automatically
    /// </summary>
#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    public class MMTransformRandomizer : MonoBehaviour
    {
        /// the possible ways to automatically randomize
        public enum AutoExecutionModes
        {
            Never,
            OnAwake,
            OnStart,
            OnEnable
        }

        [Header("Position")]
        /// whether or not to randomize position
        public bool RandomizePosition = true;

        /// the minimum position to apply when randomizing
        [MMCondition("RandomizePosition", true)]
        public Vector3 MinRandomPosition;

        /// the maximum position to apply when randomizing
        [MMCondition("RandomizePosition", true)]
        public Vector3 MaxRandomPosition;

        [Header("Rotation")]
        /// whether or not to randomize rotation
        public bool RandomizeRotation = true;

        /// the minimum rotation to apply when randomizing (in degrees)
        [MMCondition("RandomizeRotation", true)]
        public Vector3 MinRandomRotation;

        /// the maximum rotation to apply when randomizing (in degrees)
        [MMCondition("RandomizeRotation", true)]
        public Vector3 MaxRandomRotation;

        [Header("Scale")]
        /// whether or not to randomize scale
        public bool RandomizeScale = true;

        /// the minimum scale to apply when randomizing
        [MMCondition("RandomizeScale", true)] public Vector3 MinRandomScale;

        /// the maximum scale to apply when randomizing
        [MMCondition("RandomizeScale", true)] public Vector3 MaxRandomScale;

        [Header("Settings")]
        /// whether or not to remove this component after randomizing its attributes
        public bool AutoRemoveAfterRandomize;

        /// whether or not to remove all colliders attached to this object
        public bool RemoveAllColliders;

        /// the selected auto execution mode
        public AutoExecutionModes AutoExecutionMode = AutoExecutionModes.Never;

        /// <summary>
        ///     On Awake we randomize if needed
        /// </summary>
        protected virtual void Awake()
        {
            if (Application.isPlaying && AutoExecutionMode == AutoExecutionModes.OnAwake) Randomize();
        }

        /// <summary>
        ///     On Start we randomize if needed
        /// </summary>
        protected virtual void Start()
        {
            if (Application.isPlaying && AutoExecutionMode == AutoExecutionModes.OnStart) Randomize();
        }

        /// <summary>
        ///     On Enable we randomize if needed
        /// </summary>
        protected virtual void OnEnable()
        {
            if (Application.isPlaying && AutoExecutionMode == AutoExecutionModes.OnEnable) Randomize();
        }

        /// <summary>
        ///     Randomizes position, rotation, scale, and cleanups if necessary
        /// </summary>
        public virtual void Randomize()
        {
            ProcessRandomizePosition();
            ProcessRandomizeRotation();
            ProcessRandomizeScale();
            RemoveColliders();
            Cleanup();
        }

        /// <summary>
        ///     Randomizes the position
        /// </summary>
        protected virtual void ProcessRandomizePosition()
        {
            if (!RandomizePosition) return;
            var randomPosition = MMMaths.RandomVector3(MinRandomPosition, MaxRandomPosition);
            transform.localPosition += randomPosition;
        }

        /// <summary>
        ///     Randomizes the rotation
        /// </summary>
        protected virtual void ProcessRandomizeRotation()
        {
            if (!RandomizeRotation) return;
            var randomRotation = MMMaths.RandomVector3(MinRandomRotation, MaxRandomRotation);
            transform.localRotation = Quaternion.Euler(randomRotation);
        }

        /// <summary>
        ///     Randomizes the scale
        /// </summary>
        protected virtual void ProcessRandomizeScale()
        {
            if (!RandomizeScale) return;
            var randomScale = MMMaths.RandomVector3(MinRandomScale, MaxRandomScale);
            transform.localScale = randomScale;
        }

        /// <summary>
        ///     Removes all colliders attached to this object or its children
        /// </summary>
        protected virtual void RemoveColliders()
        {
            if (RemoveAllColliders)
            {
#if UNITY_EDITOR
                var colliders = gameObject.GetComponentsInChildren<Collider>();
                foreach (var collider in colliders) DestroyImmediate(collider);
                var colliders2D = gameObject.GetComponentsInChildren<Collider2D>();
                foreach (var collider2D in colliders2D) DestroyImmediate(collider2D);
#endif
            }
        }

        /// <summary>
        ///     Destroys this component
        /// </summary>
        protected virtual void Cleanup()
        {
            if (AutoRemoveAfterRandomize)
            {
#if UNITY_EDITOR
                DestroyImmediate(this);
#endif
            }
        }
    }
}