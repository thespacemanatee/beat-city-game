﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools
{
    [AddComponentMenu("More Mountains/Tools/GUI/MMPSBToUIConverter")]
    public class MMPSBToUIConverter : MonoBehaviour
    {
        [Header("Target")] public Canvas TargetCanvas;

        public float ScaleFactor = 100f;
        public bool ReplicateNesting;

        [Header("Size")] public float TargetWidth = 2048;

        public float TargetHeight = 1152;

        [Header("Conversion")] [MMInspectorButton("ConvertToCanvas")]
        public bool ConvertToCanvasButton;

        public Vector3 ChildImageOffset = new(-1024f, -576f, 0f);
        protected Dictionary<Transform, int> _sortingOrders;

        protected Transform _topLevel;

        public virtual void ConvertToCanvas()
        {
            Screen.SetResolution((int)TargetWidth, (int)TargetHeight, true);

            _sortingOrders = new Dictionary<Transform, int>();

            // remove existing canvas if found
            foreach (Transform child in TargetCanvas.transform)
                if (child.name == name)
                {
                    child.MMDestroyAllChildren();
                    DestroyImmediate(child.gameObject);
                }

            // force size on canvas scaler
            var canvasScaler = TargetCanvas.GetComponent<CanvasScaler>();
            if (canvasScaler != null) canvasScaler.referenceResolution = new Vector2(TargetWidth, TargetHeight);

            // create a parent in the target canvas
            var newRoot = new GameObject(name, typeof(RectTransform));
            newRoot.transform.SetParent(TargetCanvas.transform);
            var newRootRect = newRoot.GetComponent<RectTransform>();
            SetupForStretch(newRootRect);

            _topLevel = newRoot.transform;
            CreateImageForChildren(transform, newRoot.transform);

            // apply sorting orders
            foreach (var pair in _sortingOrders) pair.Key.SetSiblingIndex(pair.Value);
        }

        /// <summary>
        ///     Recursively goes through the children of the specified "root" Transform, and parents them to the specified "parent"
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        protected virtual void CreateImageForChildren(Transform root, Transform parent)
        {
            foreach (Transform child in root)
            {
                var imageGO = new GameObject(child.name, typeof(RectTransform));
                imageGO.transform.localPosition = ScaleFactor * child.transform.localPosition;
                if (ReplicateNesting)
                {
                    imageGO.transform.SetParent(parent);
                }
                else
                {
                    imageGO.transform.SetParent(_topLevel);
                    var newLocalPosition = imageGO.transform.localPosition;
                    newLocalPosition.x = newLocalPosition.x + TargetWidth / 2f;
                    imageGO.transform.localPosition = newLocalPosition;
                }

                var spriteRenderer = child.gameObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    var image = imageGO.AddComponent<Image>();
                    image.sprite = spriteRenderer.sprite;
                    _sortingOrders.Add(image.transform, spriteRenderer.sortingOrder);
                    image.SetNativeSize();

                    var imageGoRect = imageGO.GetComponent<RectTransform>();
                    var newPosition = imageGoRect.localPosition;
                    newPosition += ChildImageOffset;
                    newPosition.z = 0f;
                    imageGoRect.localPosition = newPosition;
                }
                else
                {
                    imageGO.name += " - NODE";
                    var imageGoRect = imageGO.GetComponent<RectTransform>();
                    imageGoRect.sizeDelta = new Vector2(TargetWidth, TargetHeight);
                    imageGoRect.localPosition = Vector3.zero;
                }

                imageGO.GetComponent<RectTransform>().localScale = Vector3.one;
                CreateImageForChildren(child, imageGO.transform);
            }
        }

        protected virtual void SetupForStretch(RectTransform rect)
        {
            rect.localPosition = Vector3.zero;
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
        }
    }
}