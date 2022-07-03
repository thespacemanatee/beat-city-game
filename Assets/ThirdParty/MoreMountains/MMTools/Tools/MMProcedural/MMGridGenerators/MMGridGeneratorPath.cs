using UnityEngine;
using Random = System.Random;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     Generates a grid with a path in the specified direction
    /// </summary>
    public class MMGridGeneratorPath : MMGridGenerator
    {
        public enum Directions
        {
            TopToBottom,
            BottomToTop,
            LeftToRight,
            RightToLeft
        }

        /// <summary>
        ///     Generates a grid with a path in the specified direction
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="seed"></param>
        /// <param name="direction"></param>
        /// <param name="startPosition"></param>
        /// <param name="pathMinWidth"></param>
        /// <param name="pathMaxWidth"></param>
        /// <param name="directionChangeDistance"></param>
        /// <param name="widthChangePercentage"></param>
        /// <param name="directionChangePercentage"></param>
        /// <returns></returns>
        public static int[,] Generate(int width, int height, int seed, Directions direction, Vector2Int startPosition,
            int pathMinWidth, int pathMaxWidth, int directionChangeDistance, int widthChangePercentage,
            int directionChangePercentage)
        {
            var grid = PrepareGrid(ref width, ref height);
            grid = MMGridGeneratorFull.Generate(width, height, true);
            var random = new Random(seed);
            UnityEngine.Random.InitState(seed);

            var pathWidth = 1;
            var initialX = startPosition.x;
            var initialY = startPosition.y;

            SetGridCoordinate(grid, initialX, initialY, 0);

            switch (direction)
            {
                case Directions.TopToBottom:
                    var x1 = initialX;
                    for (var i = -pathWidth; i <= pathWidth; i++) SetGridCoordinate(grid, x1 + i, initialY, 0);
                    for (var y = initialY; y > 0; y--)
                    {
                        pathWidth = ComputeWidth(random, widthChangePercentage, pathMinWidth, pathMaxWidth, pathWidth);
                        x1 = DetermineNextStep(random, x1, directionChangeDistance, directionChangePercentage,
                            pathMaxWidth, width);
                        for (var i = -pathWidth; i <= pathWidth; i++) SetGridCoordinate(grid, x1 + i, y, 0);
                    }

                    break;
                case Directions.BottomToTop:
                    var x2 = initialX;
                    for (var i = -pathWidth; i <= pathWidth; i++) SetGridCoordinate(grid, x2 + i, initialY, 0);
                    for (var y = initialY; y < height; y++)
                    {
                        pathWidth = ComputeWidth(random, widthChangePercentage, pathMinWidth, pathMaxWidth, pathWidth);
                        x2 = DetermineNextStep(random, x2, directionChangeDistance, directionChangePercentage,
                            pathMaxWidth, width);
                        for (var i = -pathWidth; i <= pathWidth; i++) SetGridCoordinate(grid, x2 + i, y, 0);
                    }

                    break;
                case Directions.LeftToRight:
                    var y1 = initialY;
                    for (var i = -pathWidth; i <= pathWidth; i++) SetGridCoordinate(grid, initialX, y1 + i, 0);
                    for (var x = initialX; x < width; x++)
                    {
                        pathWidth = ComputeWidth(random, widthChangePercentage, pathMinWidth, pathMaxWidth, pathWidth);
                        y1 = DetermineNextStep(random, y1, directionChangeDistance, directionChangePercentage,
                            pathMaxWidth, width);
                        for (var i = -pathWidth; i <= pathWidth; i++) SetGridCoordinate(grid, x, y1 + i, 0);
                    }

                    break;
                case Directions.RightToLeft:
                    var y2 = initialY;
                    for (var i = -pathWidth; i <= pathWidth; i++) SetGridCoordinate(grid, initialX, y2 + i, 0);
                    for (var x = initialX; x > 0; x--)
                    {
                        pathWidth = ComputeWidth(random, widthChangePercentage, pathMinWidth, pathMaxWidth, pathWidth);
                        y2 = DetermineNextStep(random, y2, directionChangeDistance, directionChangePercentage,
                            pathMaxWidth, width);
                        for (var i = -pathWidth; i <= pathWidth; i++) SetGridCoordinate(grid, x, y2 + i, 0);
                    }

                    break;
            }

            return grid;
        }

        /// <summary>
        ///     Determines the new width of the path
        /// </summary>
        /// <param name="random"></param>
        /// <param name="widthChangePercentage"></param>
        /// <param name="pathMinWidth"></param>
        /// <param name="pathMaxWidth"></param>
        /// <param name="pathWidth"></param>
        /// <returns></returns>
        private static int ComputeWidth(Random random, int widthChangePercentage, int pathMinWidth, int pathMaxWidth,
            int pathWidth)
        {
            if (random.Next(0, 100) > widthChangePercentage)
            {
                var widthChange = UnityEngine.Random.Range(-pathMaxWidth, pathMaxWidth);
                pathWidth += widthChange;
                if (pathWidth < pathMinWidth) pathWidth = pathMinWidth;
                if (pathWidth > pathMaxWidth) pathWidth = pathMaxWidth;
            }

            return pathWidth;
        }

        /// <summary>
        ///     Determines in what direction to move the path
        /// </summary>
        /// <param name="random"></param>
        /// <param name="x"></param>
        /// <param name="directionChangeDistance"></param>
        /// <param name="directionChangePercentage"></param>
        /// <param name="pathMaxWidth"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private static int DetermineNextStep(Random random, int x, int directionChangeDistance,
            int directionChangePercentage, int pathMaxWidth, int width)
        {
            if (random.Next(0, 100) > directionChangePercentage)
            {
                var xChange = UnityEngine.Random.Range(-directionChangeDistance, directionChangeDistance);
                x += xChange;
                if (x < pathMaxWidth) x = pathMaxWidth;
                if (x > width - pathMaxWidth) x = width - pathMaxWidth;
            }

            return x;
        }
    }
}