using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    ///     This class will set the depth of field to focus on the set of targets specified in its inspector.
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMAutoFocus")]
    [RequireComponent(typeof(PostProcessVolume))]
    public class MMAutoFocus : MonoBehaviour
    {
        [Header("Bindings")]
        /// the position of the camera
        [Tooltip("the position of the camera")]
        public Transform CameraTransform;

        /// a list of all possible targets
        [Tooltip("a list of all possible targets")]
        public Transform[] FocusTargets;

        /// an offset to apply to the focus target
        [Tooltip("an offset to apply to the focus target")]
        public Vector3 Offset;

        [Header("Setup")]
        /// the current target of this auto focus
        [Tooltip("the current target of this auto focus")]
        public float FocusTargetID;

        [Header("Desired Aperture")]
        /// the aperture to work with
        [Tooltip("the aperture to work with")]
        [Range(0.1f, 20f)]
        public float Aperture = 0.1f;

        protected DepthOfField _depthOfField;
        protected PostProcessProfile _profile;

        protected PostProcessVolume _volume;

        /// <summary>
        ///     On start we grab our volume and profile
        /// </summary>
        private void Start()
        {
            _volume = GetComponent<PostProcessVolume>();
            _profile = _volume.profile;
            _profile.TryGetSettings(out _depthOfField);
        }

        /// <summary>
        ///     Adapts DoF to target
        /// </summary>
        private void Update()
        {
            var focusTargetID = Mathf.FloorToInt(FocusTargetID);
            if (focusTargetID < FocusTargets.Length)
            {
                var distance = Vector3.Distance(CameraTransform.position,
                    FocusTargets[focusTargetID].position + Offset);
                _depthOfField.focusDistance.Override(distance);
                _depthOfField.aperture.Override(Aperture);
            }
        }
    }
}