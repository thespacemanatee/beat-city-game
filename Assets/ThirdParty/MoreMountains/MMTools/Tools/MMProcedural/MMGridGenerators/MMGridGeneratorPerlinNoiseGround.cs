﻿using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     Generates a grid with a ground floor
    /// </summary>
    public class MMGridGeneratorPerlinNoiseGround : MMGridGenerator
    {
        /// <summary>
        ///     Generates a grid with a ground floor
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static int[,] Generate(int width, int height, float seed)
        {
            var grid = PrepareGrid(ref width, ref height);

            for (var i = 0; i < width; i++)
            {
                var groundHeight = Mathf.FloorToInt((Mathf.PerlinNoise(i, seed) - 0.5f) * height) + height / 2;
                for (var j = groundHeight; j >= 0; j--) SetGridCoordinate(grid, i, j, 1);
            }

            return grid;
        }
    }
}