using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoreMountains.MMInterface
{
    [RequireComponent(typeof(Image))]
    public class MMScrollviewButton : Button
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (interactable)
            {
                base.OnPointerExit(eventData);
                if (!eventData.dragging) base.OnPointerClick(eventData);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
        }
    }
}