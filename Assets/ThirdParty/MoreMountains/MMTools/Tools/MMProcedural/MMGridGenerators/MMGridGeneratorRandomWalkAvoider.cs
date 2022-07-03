using UnityEngine;
using Random = System.Random;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     Generates a grid with a path carved by a drunkard walk algorithm that will avoid another grid's walls
    /// </summary>
    public class MMGridGeneratorRandomWalkAvoider : MMGridGenerator
    {
        /// <summary>
        ///     Generates a grid with a path carved by a drunkard walk algorithm that will avoid another grid's walls
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="seed"></param>
        /// <param name="fillPercentage"></param>
        /// <returns></returns>
        public static int[,] Generate(int width, int height, int seed, int fillPercentage, Vector2Int startingPoint,
            int[,] obstacles, int obstacleDistance, int maxIterations)
        {
            var grid = PrepareGrid(ref width, ref height);
            grid = MMGridGeneratorFull.Generate(width, height, true);
            var random = new Random(seed);

            var requiredFillQuantity = width * height * fillPercentage / 100;
            var fillCounter = 0;

            var currentX = startingPoint.x;
            var currentY = startingPoint.y;
            grid[currentX, currentY] = 0;
            fillCounter++;

            var iterationsCount = 0;

            while (fillCounter < requiredFillQuantity && iterationsCount < maxIterations)
            {
                var direction = random.Next(4);

                switch (direction)
                {
                    case 0: // up
                        if (currentY + 1 <= height && !ObstacleAt(obstacles, currentX, currentY + obstacleDistance))
                        {
                            currentY++;
                            grid = Carve(grid, currentX, currentY, ref fillCounter);
                        }

                        break;
                    case 1: // down 
                        if (currentY - 1 > 1 && !ObstacleAt(obstacles, currentX, currentY - obstacleDistance))
                        {
                            currentY--;
                            grid = Carve(grid, currentX, currentY, ref fillCounter);
                        }

                        break;
                    case 2: // left
                        if (currentX - 1 > 1 && !ObstacleAt(obstacles, currentX - obstacleDistance, currentY))
                        {
                            currentX--;
                            grid = Carve(grid, currentX, currentY, ref fillCounter);
                        }

                        break;
                    case 3: // right
                        if (currentX + 1 <= width && !ObstacleAt(obstacles, currentX + obstacleDistance, currentY))
                        {
                            currentX++;
                            grid = Carve(grid, currentX, currentY, ref fillCounter);
                        }

                        break;
                }

                iterationsCount++;
            }

            return grid;
        }

        /// <summary>
        ///     Returns true if an obstacle is found at the specified coordinates, false otherwise
        /// </summary>
        /// <param name="obstacles"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static bool ObstacleAt(int[,] obstacles, int x, int y)
        {
            return GetValueAtGridCoordinate(obstacles, x, y, 1) == 1;
        }

        /// <summary>
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="fillCounter"></param>
        /// <returns></returns>
        private static int[,] Carve(int[,] grid, int x, int y, ref int fillCounter)
        {
            if (GetValueAtGridCoordinate(grid, x, y, 0) == 1)
            {
                SetGridCoordinate(grid, x, y, 0);
                fillCounter++;
            }

            return grid;
        }
    }
}