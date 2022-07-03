using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MoreMountains.Tools
{
    public class MMTilemapGridRenderer
    {
        /// <summary>
        ///     Renders the specified grid on the specified tilemap, with optional slow mode (only works at runtime)
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="tilemap"></param>
        /// <param name="tile"></param>
        /// <param name="slowRender"></param>
        /// <param name="slowRenderDuration"></param>
        /// <param name="slowRenderTweenType"></param>
        /// <param name="slowRenderSupport"></param>
        public static void RenderGrid(int[,] grid, MMTilemapGeneratorLayer layer, bool slowRender = false,
            float slowRenderDuration = 1f,
            MMTweenType slowRenderTweenType = null, MonoBehaviour slowRenderSupport = null)
        {
            if (layer.FusionMode == MMTilemapGeneratorLayer.FusionModes.Normal) ClearTilemap(layer.TargetTilemap);
            var tile = layer.Tile;
            if (layer.FusionMode == MMTilemapGeneratorLayer.FusionModes.Combine)
            {
                grid = MMGridGenerator.InvertGrid(grid);
                tile = null;
            }

            if (layer.FusionMode == MMTilemapGeneratorLayer.FusionModes.Subtract)
                grid = MMGridGenerator.InvertGrid(grid);

            if (!slowRender || !Application.isPlaying)
                DrawGrid(grid, layer.TargetTilemap, tile, 0, TotalFilledBlocks(grid));
            else
                slowRenderSupport.StartCoroutine(SlowRenderGrid(grid, layer.TargetTilemap, tile, slowRenderDuration,
                    slowRenderTweenType, 60));

            if (!Application.isPlaying && slowRender)
                Debug.LogWarning("Rendering maps in SlowRender mode is only supported at runtime.");
        }

        /// <summary>
        ///     Renders a grid chunk by chunk - runtime only
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="tilemap"></param>
        /// <param name="tile"></param>
        /// <param name="slowRenderDuration"></param>
        /// <param name="slowRenderTweenType"></param>
        /// <param name="frameRate"></param>
        /// <returns></returns>
        public static IEnumerator SlowRenderGrid(int[,] grid, Tilemap tilemap, TileBase tile, float slowRenderDuration,
            MMTweenType slowRenderTweenType, int frameRate)
        {
            var totalBlocks = TotalFilledBlocks(grid);
            totalBlocks = totalBlocks == 0 ? 1 : totalBlocks;
            frameRate = frameRate == 0 ? 1 : frameRate;
            var refreshFrequency = 1f / frameRate;
            var startedAt = Time.unscaledTime;
            var lastWaitAt = startedAt;
            var drawnBlocks = 0;
            var lastIndex = 0;

            while (Time.unscaledTime - startedAt < slowRenderDuration)
            {
                while (Time.unscaledTime - lastWaitAt < refreshFrequency) yield return null;

                var remainingBlocks = totalBlocks - drawnBlocks;
                var elapsedTime = Time.unscaledTime - startedAt;
                var remainingTime = slowRenderDuration - elapsedTime;
                var normalizedProgress = MMMaths.Remap(elapsedTime, 0f, slowRenderDuration, 0f, 1f);
                var curveProgress = MMTween.Tween(normalizedProgress, 0f, 1f, 0f, 1f, slowRenderTweenType);
                var ratio = 1 - (normalizedProgress - curveProgress);

                var blocksToDraw = Mathf.RoundToInt(remainingBlocks / remainingTime * refreshFrequency * ratio);

                lastIndex = DrawGrid(grid, tilemap, tile, lastIndex, blocksToDraw);
                drawnBlocks += blocksToDraw;
                lastWaitAt = Time.unscaledTime;
            }

            DrawGrid(grid, tilemap, tile, lastIndex, totalBlocks - lastIndex);
        }

        /// <summary>
        ///     Returns the total amount of filled blocks in a grid
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static int TotalFilledBlocks(int[,] grid)
        {
            var width = grid.GetUpperBound(0);
            var height = grid.GetUpperBound(1);

            var totalBlocks = 0;
            for (var i = 0; i <= width; i++)
            for (var j = 0; j <= height; j++)
                if (grid[i, j] == 1)
                    totalBlocks++;
            return totalBlocks;
        }

        /// <summary>
        ///     Draws the specified section of a grid on a target tilemap
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="tilemap"></param>
        /// <param name="tile"></param>
        /// <param name="startIndex"></param>
        /// <param name="numberOfTilesToDraw"></param>
        /// <returns></returns>
        private static int DrawGrid(int[,] grid, Tilemap tilemap, TileBase tile, int startIndex,
            int numberOfTilesToDraw)
        {
            var width = grid.GetUpperBound(0);
            var height = grid.GetUpperBound(1);

            tilemap.RefreshAllTiles();

            var counter = 0;
            var drawCount = 0;

            for (var i = 0; i <= width; i++)
            for (var j = 0; j <= height; j++)
                if (grid[i, j] == 1)
                {
                    if (counter >= startIndex)
                    {
                        var tilePosition = new Vector3Int(i, j, 0);
                        tilePosition += ComputeOffset(width, height);
                        tilemap.SetTile(tilePosition, tile);
                        drawCount++;
                    }

                    if (drawCount > numberOfTilesToDraw) return counter;
                    counter++;
                }

            return counter;
        }

        /// <summary>
        ///     Determines the offset to apply to a grid to have it centered
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Vector3Int ComputeOffset(int width, int height)
        {
            var offset = new Vector3Int(width + 2, height + 2, 0);
            offset = offset - offset / 2;
            return -offset;
        }

        /// <summary>
        ///     Clears and refreshes an entire tilemap
        /// </summary>
        /// <param name="tilemap"></param>
        public static void ClearTilemap(Tilemap tilemap)
        {
            tilemap.ClearAllTiles();
            tilemap.RefreshAllTiles();
        }
    }
}