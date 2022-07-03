﻿using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     Add this class to a line renderer and it'll add control points that let you turn your line into a bezier curve
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    [AddComponentMenu("More Mountains/Tools/Sprites/MMBezierLineRenderer")]
    public class MMBezierLineRenderer : MonoBehaviour
    {
        /// a list of handles to control your line. Usually 4, but you can have more.
        public Transform[] AdjustmentHandles;

        /// the amount of segments of the line renderer (more segments, less visible straight lines)
        public int NumberOfSegments = 50;

        /// the sorting layer for this line renderer
        public string SortingLayerName = "Default";

        /// the amount of curves we're working with
        [MMReadOnly] public int NumberOfCurves;

        protected bool _initialized;
        protected LineRenderer _lineRenderer;
        protected Vector3 _p;
        protected Vector3 _point;

        protected int _sortingLayerID;

        /// <summary>
        ///     On Awake we initialize our line renderer
        /// </summary>
        protected virtual void Awake()
        {
            Initialization();
        }

        /// <summary>
        ///     On Update we draw our curve
        /// </summary>
        protected virtual void LateUpdate()
        {
            DrawCurve();
        }

        /// <summary>
        ///     Grabs the sorting layer, computes the amount of curves
        /// </summary>
        protected virtual void Initialization()
        {
            if (_initialized) return;

            _sortingLayerID = SortingLayer.NameToID(SortingLayerName);

            NumberOfCurves = AdjustmentHandles.Length / 3;

            _lineRenderer = GetComponent<LineRenderer>();
            if (_lineRenderer != null) _lineRenderer.sortingLayerID = _sortingLayerID;
            _initialized = true;
        }

        /// <summary>
        ///     For each point, determines the bezier position and feeds it to the line renderer
        /// </summary>
        protected virtual void DrawCurve()
        {
            for (var i = 0; i < NumberOfCurves; i++)
            for (var j = 1; j <= NumberOfSegments; j++)
            {
                var t = (j - 1) / (float)(NumberOfSegments - 1);
                var pointIndex = i * 3;
                _point = BezierPoint(t, AdjustmentHandles[pointIndex].position,
                    AdjustmentHandles[pointIndex + 1].position, AdjustmentHandles[pointIndex + 2].position,
                    AdjustmentHandles[pointIndex + 3].position);
                _lineRenderer.positionCount = i * NumberOfSegments + j;
                _lineRenderer.SetPosition(i * NumberOfSegments + (j - 1), _point);
            }
        }

        /// <summary>
        ///     Computes the coordinates of a point on the bezier curve controlled by p0, p1, p2 and p3
        /// </summary>
        /// <param name="t"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        protected virtual Vector3 BezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var u = 1 - t;
            var tt = t * t;
            var uu = u * u;
            var uuu = uu * u;
            var ttt = tt * t;

            _p = uuu * p0;
            _p += 3 * uu * t * p1;
            _p += 3 * u * tt * p2;
            _p += ttt * p3;

            return _p;
        }
    }
}