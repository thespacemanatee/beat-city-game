using UnityEngine;
using UnityEngine.Tilemaps;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     A collection of helper methods for interacting with Tilemaps
    /// </summary>
    public class MMTilemap : MonoBehaviour
    {
        /// <summary>
        ///     Returns a random world position on the specified tilemap/grid combo, filled or not based on the shouldBeFilled flag
        /// </summary>
        /// <param name="targetTilemap"></param>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="shouldBeFilled"></param>
        /// <param name="maxIterations"></param>
        /// <returns></returns>
        public static Vector2 GetRandomPosition(Tilemap targetTilemap, Grid grid, int width, int height,
            bool shouldBeFilled = true, int maxIterations = 1000)
        {
            var iterationsCount = 0;
            var randomCoordinate = Vector3Int.zero;

            while (iterationsCount < maxIterations)
            {
                randomCoordinate.x = Random.Range(0, width);
                randomCoordinate.y = Random.Range(0, height);
                randomCoordinate += MMTilemapGridRenderer.ComputeOffset(width - 1, height - 1);

                var hasTile = targetTilemap.HasTile(randomCoordinate);
                if (hasTile == shouldBeFilled) return targetTilemap.CellToWorld(randomCoordinate) + grid.cellSize / 2;

                iterationsCount++;
            }

            return Vector2.zero;
        }

        /// <summary>
        ///     Returns a random position on the ground floor of the grid
        /// </summary>
        /// <param name="targetTilemap"></param>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="startingHeight"></param>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        /// <param name="shouldBeFilled"></param>
        /// <param name="maxIterations"></param>
        /// <returns></returns>
        public static Vector2 GetRandomPositionOnGround(Tilemap targetTilemap, Grid grid, int width, int height,
            int startingHeight, int xMin, int xMax, bool shouldBeFilled = true, int maxIterations = 1000)
        {
            var iterationsCount = 0;
            var randomCoordinate = Vector3Int.zero;

            while (iterationsCount < maxIterations)
            {
                randomCoordinate.x = Random.Range(xMin, xMax);
                randomCoordinate.y = startingHeight;
                randomCoordinate += MMTilemapGridRenderer.ComputeOffset(width - 1, height - 1);

                var counter = height;

                while (counter > 0)
                {
                    var hasTile = targetTilemap.HasTile(randomCoordinate);
                    if (hasTile == shouldBeFilled)
                    {
                        randomCoordinate.y++;
                        return targetTilemap.CellToWorld(randomCoordinate) + grid.cellSize / 2;
                    }

                    randomCoordinate.y--;
                    counter--;
                }

                iterationsCount++;
            }

            return Vector2.zero;
        }
    }
}