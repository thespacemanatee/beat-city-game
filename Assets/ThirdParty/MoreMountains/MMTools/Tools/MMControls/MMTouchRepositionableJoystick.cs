using UnityEngine;
using UnityEngine.EventSystems;

namespace MoreMountains.Tools
{
	/// <summary>
	///     Add this component to a UI rectangle and it'll act as a detection zone for a joystick.
	///     Note that this component extends the MMTouchJoystick class so you don't need to add another joystick to it. It's
	///     both the detection zone and the stick itself.
	/// </summary>
	[AddComponentMenu("More Mountains/Tools/Controls/MMTouchRepositionableJoystick")]
    public class MMTouchRepositionableJoystick : MMTouchJoystick, IPointerDownHandler
    {
        [Header("Dynamic Joystick")] public CanvasGroup KnobCanvasGroup;

        public CanvasGroup BackgroundCanvasGroup;

        protected Vector3 _initialPosition;
        protected CanvasGroup _knobCanvasGroup;
        protected Vector3 _newPosition;

        /// <summary>
        ///     On Start, we instantiate our joystick's image if there's one
        /// </summary>
        protected override void Start()
        {
            base.Start();

            // we store the detection zone's initial position
            _initialPosition = GetComponent<RectTransform>().localPosition;
        }

        /// <summary>
        ///     When the zone is pressed, we move our joystick accordingly
        /// </summary>
        /// <param name="data">Data.</param>
        public virtual void OnPointerDown(PointerEventData data)
        {
            // if we're in "screen space - camera" render mode
            if (ParentCanvasRenderMode == RenderMode.ScreenSpaceCamera)
                _newPosition = TargetCamera.ScreenToWorldPoint(data.position);
            // otherwise
            else
                _newPosition = data.position;
            _newPosition.z = transform.position.z;

            // we define a new neutral position
            BackgroundCanvasGroup.transform.position = _newPosition;
            SetNeutralPosition(_newPosition);
            _knobTransform.position = _newPosition;
        }

        public override void Initialize()
        {
            base.Initialize();
            SetKnobTransform(KnobCanvasGroup.transform);
            _canvasGroup = KnobCanvasGroup;
            _initialOpacity = _canvasGroup.alpha;
        }

        /// <summary>
        ///     When the player lets go of the stick, we restore our stick's position if needed
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
        }
    }
}