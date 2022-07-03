using UnityEngine;

namespace MoreMountains.Tools
{
    public class MMUIFollowMouse : MonoBehaviour
    {
        protected Vector2 _newPosition;
        public Canvas TargetCanvas { get; set; }

        protected virtual void LateUpdate()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(TargetCanvas.transform as RectTransform,
                Input.mousePosition, TargetCanvas.worldCamera, out _newPosition);
            transform.position = TargetCanvas.transform.TransformPoint(_newPosition);
        }
    }
}