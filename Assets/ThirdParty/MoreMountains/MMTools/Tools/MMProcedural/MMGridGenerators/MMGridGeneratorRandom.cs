using System;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     Generates a simple grid filled with random points
    /// </summary>
    public class MMGridGeneratorRandom : MMGridGenerator
    {
        /// <summary>
        ///     Generates a simple grid filled with random points
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="seed"></param>
        /// <param name="fillPercentage"></param>
        /// <returns></returns>
        public static int[,] Generate(int width, int height, int seed, int fillPercentage)
        {
            var grid = PrepareGrid(ref width, ref height);

            grid = MMGridGeneratorFull.Generate(width, height, true);
            var random = new Random(seed);

            for (var i = 0; i <= width; i++)
            for (var j = 0; j <= height; j++)
            {
                var value = random.Next(0, 100) < fillPercentage ? 1 : 0;
                SetGridCoordinate(grid, i, j, value);
            }

            return grid;
        }
    }
}