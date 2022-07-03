using System.Reflection;
using UnityEngine;

namespace MoreMountains.Tools
{
    public class MMPlotterGenerator : MonoBehaviour
    {
        public MMPlotter PlotterPrefab;

        public Vector2 Spacing;
        public float VerticalOddSpacing;
        public int RowLength;

        [Header("Materials")] public Material LinearMaterial;

        public Material QuadraticMaterial;
        public Material CubicMaterial;
        public Material QuarticMaterial;
        public Material QuinticMaterial;
        public Material SinusoidalMaterial;
        public Material BounceMaterial;
        public Material OverheadMaterial;
        public Material ExponentialMaterial;
        public Material ElasticMaterial;
        public Material CircularMaterial;

        [MMInspectorButton("GeneratePlotters")]
        public bool GeneratePlottersButton;

        protected Vector2 _position;

        protected virtual void Awake()
        {
            Time.timeScale = 0f;

            GeneratePlotters();
        }

        protected virtual void GeneratePlotters()
        {
            transform.MMDestroyAllChildren();

            var flags = BindingFlags.Public | BindingFlags.Static;
            var methods = typeof(MMTweenDefinitions).GetMethods(flags);

            var row = 0;
            var column = 0;
            float yCoordinate = 0;

            for (var i = 0; i < methods.Length; i++)
            {
                _position.x = column * Spacing.x;


                _position.y = yCoordinate;

                var plotter = Instantiate(PlotterPrefab);
                plotter.transform.SetParent(transform);
                plotter.transform.localPosition = _position;
                plotter.TweenMethodIndex = i;
                var tweenName = plotter.TweenName(plotter.TweenMethodIndex);
                plotter.gameObject.name = tweenName;

                var newMaterial = LinearMaterial;
                if (tweenName.Contains("Linear")) newMaterial = LinearMaterial;
                if (tweenName.Contains("Quadratic")) newMaterial = QuadraticMaterial;
                if (tweenName.Contains("Cubic")) newMaterial = CubicMaterial;
                if (tweenName.Contains("Quartic")) newMaterial = QuarticMaterial;
                if (tweenName.Contains("Quintic")) newMaterial = QuinticMaterial;
                if (tweenName.Contains("Sinusoidal")) newMaterial = SinusoidalMaterial;
                if (tweenName.Contains("Bounce")) newMaterial = BounceMaterial;
                if (tweenName.Contains("Overhead")) newMaterial = OverheadMaterial;
                if (tweenName.Contains("Exponential")) newMaterial = ExponentialMaterial;
                if (tweenName.Contains("Elastic")) newMaterial = ElasticMaterial;
                if (tweenName.Contains("Circular")) newMaterial = CircularMaterial;

                plotter.SetMaterial(newMaterial);
                plotter.GetMethodsList();
                plotter.DrawGraph();

                if (column >= RowLength - 1)
                {
                    column = 0;
                    row++;
                    if (row % 2 == 0)
                        yCoordinate += Spacing.y + VerticalOddSpacing;
                    else
                        yCoordinate += Spacing.y;
                }
                else
                {
                    column++;
                }
            }
        }
    }
}