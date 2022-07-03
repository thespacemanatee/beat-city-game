﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     A class used to display a reoderable list of MMTilemapGeneratorLayers
    /// </summary>
    [Serializable]
    public class MMTilemapGeneratorLayerList : MMReorderableArray<MMTilemapGeneratorLayer>
    {
    }

    /// <summary>
    ///     A class used to store and edit the data of MMTilemapGenerator layers, which you can use and combine
    ///     to generate unique and random grids
    /// </summary>
    [Serializable]
    public class MMTilemapGeneratorLayer
    {
        /// the various modes of fusion you can use on this layer.
        /// Fusion modes will be applied on layers from top to bottom (the last to speak wins)
        /// Normal : just generates a grid, default mode
        /// NormalNoClear : generates a grid, but doesn't clear it first
        /// Intersect : when painting on a target grid that already has content, will only keep the resulting intersection
        /// Combine : adds the result of this grid to the existing target
        /// Subtract : removes the result of this grid from the existing target
        public enum FusionModes
        {
            Normal,
            NormalNoClear,
            Intersect,
            Combine,
            Subtract
        }

        /// the name of this layer, doesn't do anything, just used to organize things
        [Tooltip("the name of this layer, doesn't do anything, just used to organize things")]
        public string Name = "Layer";

        /// whether this layer should be taken into account when generating the final grid
        [Tooltip("whether this layer should be taken into account when generating the final grid")]
        public bool Active = true;

        [Header("Tilemaps")]
        /// the tilemap on which to paint tiles
        [Tooltip("the tilemap on which to paint tiles")]
        public Tilemap TargetTilemap;

        /// the tile to use to paint on the tilemap
        [Tooltip("the tile to use to paint on the tilemap")]
        public TileBase Tile;

        [Header("Grid")]
        /// whether or not this layer should paint a grid of a different size than the global one
        [Tooltip("whether or not this layer should paint a grid of a different size than the global one")]
        public bool OverrideGridSize;

        /// the new value of the grid width
        [Tooltip("the new value of the grid width")] [MMCondition("OverrideGridSize", true)]
        public int GridWidth = 50;

        /// the new value of the grid height
        [Tooltip("the new value of the grid height")] [MMCondition("OverrideGridSize", true)]
        public int GridHeight = 50;

        [Header("Method")]
        /// the algorithm to use to generate this layer's grid
        [Tooltip("the algorithm to use to generate this layer's grid :\n" +
                 "Full : will fill or empty the grid\n" +
                 "Perlin : uses perlin noise to randomly fill the grid\n" +
                 "Perling Ground : uses perlin noise to generate a ground surface\n" +
                 "Random Walk : starts at point A then moves randomly, carving a path\n" +
                 "Random Walk Avoider : same, but avoids obstacles\n" +
                 "Path : starts at Point A, and carves a path in the selected direction\n" +
                 "Copy : copies another tilemap to generate a grid")]
        public MMTilemapGenerator.GenerateMethods GenerateMethod = MMTilemapGenerator.GenerateMethods.Perlin;

        /// if this is true, global seed won't be used for this layer
        [Tooltip("if this is true, global seed won't be used for this layer")]
        public bool DoNotUseGlobalSeed;

        /// whether or not to randomize this layer's seed when pressing Generate
        [Tooltip("whether or not to randomize this layer's seed when pressing Generate")]
        [MMCondition("DoNotUseGlobalSeed", true)]
        public bool RandomizeSeed = true;

        /// the dedicated seed of this layer, when not using the global one
        [Tooltip("the dedicated seed of this layer, when not using the global one")]
        [MMCondition("DoNotUseGlobalSeed", true)]
        public int Seed = 1;

        [Header("PostProcessing")]
        /// whether or not to smoothen the resulting grid, gets rid of spikes/isolated points
        [Tooltip("whether or not to smoothen the resulting grid, gets rid of spikes/isolated points")]
        public bool Smooth;

        /// whether or not to invert the grid to get the opposite result (filled becomes empty, empty becomes filled)
        [Tooltip(
            "whether or not to invert the grid to get the opposite result (filled becomes empty, empty becomes filled)")]
        public bool InvertGrid;

        /// The selected fusion mode
        [Tooltip("the various modes of fusion you can use on this layer.\n" +
                 "Fusion modes will be applied on layers from top to bottom (the last to speak wins)\n" +
                 "Normal : just generates a grid, default mode\n" +
                 "NormalNoClear : generates a grid, but doesn't clear it first\n" +
                 "Intersect : when painting on a target grid that already has content, will only keep the resulting intersection\n" +
                 "Combine : adds the result of this grid to the existing target\n" +
                 "Subtract : removes the result of this grid from the existing target")]
        public FusionModes FusionMode = FusionModes.Normal;

        [Header("Settings")]

        // full
        /// in full mode, whether the grid should be full or empty
        [Tooltip("in full mode, whether the grid should be full or empty")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.Full)]
        public bool FullGenerationFilled = true;

        // random
        /// in random mode, the percentage of the grid to fill
        [Tooltip("in random mode, the percentage of the grid to fill")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.Random)]
        public int RandomFillPercentage = 50;

        // random walk ground 
        /// in random walk ground mode,the minimum height difference between two steps
        [Tooltip("in random walk ground mode,the minimum height difference between two steps")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalkGround)]
        public int RandomWalkGroundMinHeightDifference = 1;

        /// in random walk ground mode,the maximum height difference between two steps
        [Tooltip("in random walk ground mode,the maximum height difference between two steps")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalkGround)]
        public int RandomWalkGroundMaxHeightDifference = 3;

        /// in random walk ground mode, the minimum distance that should remain flat
        [Tooltip("in random walk ground mode, the minimum distance that should remain flat")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalkGround)]
        public int RandomWalkGroundMinFlatDistance = 1;

        /// in random walk ground mode, the maximum distance that should remain flat
        [Tooltip("in random walk ground mode, the maximum distance that should remain flat")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalkGround)]
        public int RandomWalkGroundMaxFlatDistance = 3;

        /// in random walk ground mode, the maximum height of the tallest platfrom, from the bottom of the grid
        [Tooltip("in random walk ground mode, the maximum height of the tallest platfrom, from the bottom of the grid")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalkGround)]
        public int RandomWalkGroundMaxHeight = 3;

        // random walk
        /// in random walk mode, the percentage of the map the walker should try filling
        [Tooltip("in random walk mode, the percentage of the map the walker should try filling")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalk)]
        public int RandomWalkPercent = 50;

        /// in random walk mode,the point at which the walker starts, in grid coordinates
        [Tooltip("in random walk mode,the point at which the walker starts, in grid coordinates")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalk)]
        public Vector2Int RandomWalkStartingPoint = Vector2Int.zero;

        /// in random walk mode, the max amount of iterations to run the random on
        [Tooltip("in random walk mode, the max amount of iterations to run the random on")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalk)]
        public int RandomWalkMaxIterations = 1500;

        // random walk avoider
        /// in random walk avoider mode, the percentage of the grid the walker should try filling
        [Tooltip("in random walk avoider mode, the percentage of the grid the walker should try filling")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalkAvoider)]
        public int RandomWalkAvoiderPercent = 50;

        /// in random walk avoider mode, the point in grid units at which the walker starts
        [Tooltip("in random walk avoider mode, the point in grid units at which the walker starts")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalkAvoider)]
        public Vector2Int RandomWalkAvoiderStartingPoint = Vector2Int.zero;

        /// in random walk avoider mode, the tilemap containing the data the walker will try to avoid
        [Tooltip("in random walk avoider mode, the tilemap containing the data the walker will try to avoid")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalkAvoider)]
        public Tilemap RandomWalkAvoiderObstaclesTilemap;

        /// in random walk avoider mode,the distance at which the walker should try to stay away from obstacles
        [Tooltip("in random walk avoider mode,the distance at which the walker should try to stay away from obstacles")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalkAvoider)]
        public int RandomWalkAvoiderObstaclesDistance = 1;

        /// in random walk avoider mode,the max amount of iterations this algorithm will iterate on
        [Tooltip("in random walk avoider mode,the max amount of iterations this algorithm will iterate on")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.RandomWalkAvoider)]
        public int RandomWalkAvoiderMaxIterations = 100;

        // path
        /// in path mode, the start position of the path
        [Tooltip("in path mode, the start position of the path")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.Path)]
        public Vector2Int PathStartPosition = Vector2Int.zero;

        /// in path mode, the direction the path should follow
        [Tooltip("in path mode, the direction the path should follow")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.Path)]
        public MMGridGeneratorPath.Directions PathDirection = MMGridGeneratorPath.Directions.BottomToTop;

        /// in path mode, the minimum width of the path
        [Tooltip("in path mode, the minimum width of the path")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.Path)]
        public int PathMinWidth = 2;

        /// in path mode, the maximum width of the path
        [Tooltip("in path mode, the maximum width of the path")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.Path)]
        public int PathMaxWidth = 4;

        /// in path mode, the maximum number of units the path can change direction
        [Tooltip("in path mode, the maximum number of units the path can change direction")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.Path)]
        public int PathDirectionChangeDistance = 2;

        /// in path mode, the chance (in percent) for the path to change width at every step
        [Tooltip("in path mode, the chance (in percent) for the path to change width at every step")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.Path)]
        public int PathWidthChangePercentage = 50;

        /// in path mode, the chance percentage that the path will take a new direction
        [Tooltip("in path mode, the chance percentage that the path will take a new direction")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.Path)]
        public int PathDirectionChangePercentage = 50;

        // copy
        /// in copy mode, the tilemap to copy
        [Tooltip("in copy mode, the tilemap to copy")]
        [MMEnumCondition("GenerateMethod", (int)MMTilemapGenerator.GenerateMethods.Copy)]
        public Tilemap CopyTilemap;

        [Header("Bounds")]
        /// whether or not to force a wall on the grid's top
        [Tooltip("whether or not to force a wall on the grid's top")]
        public bool BoundsTop;

        /// whether or not to force a wall on the grid's bottom
        [Tooltip("whether or not to force a wall on the grid's bottom")]
        public bool BoundsBottom;

        /// whether or not to force a wall on the grid's left
        [Tooltip("whether or not to force a wall on the grid's left")]
        public bool BoundsLeft;

        /// whether or not to force a wall on the grid's right
        [Tooltip("whether or not to force a wall on the grid's right")]
        public bool BoundsRight;

        [Header("Safe Spots")]
        /// a list of "safe spots" : defined by their start and end coordinates, these areas will be left empty
        [Tooltip("a list of 'safe spots' : defined by their start and end coordinates, these areas will be left empty")]
        public List<MMTilemapGeneratorLayerSafeSpot> SafeSpots;

        [HideInInspector]
        /// this is only used to initialize the default values in the inspector
        public bool Initialized;

        /// the grid generated by this layer
        public int[,] Grid { get; set; }

        /// <summary>
        ///     This method will set default values, because Unity.
        /// </summary>
        public virtual void SetDefaults()
        {
            if (!Initialized)
            {
                GridWidth = 50;
                GridHeight = 50;
                GenerateMethod = MMTilemapGenerator.GenerateMethods.Perlin;
                RandomizeSeed = true;
                DoNotUseGlobalSeed = false;
                FusionMode = FusionModes.Normal;
                Seed = 123456789;
                Smooth = false;
                InvertGrid = false;
                FullGenerationFilled = true;
                RandomFillPercentage = 50;
                RandomWalkGroundMinHeightDifference = 1;
                RandomWalkGroundMaxHeightDifference = 3;
                RandomWalkGroundMinFlatDistance = 1;
                RandomWalkGroundMaxFlatDistance = 3;
                RandomWalkGroundMaxHeight = 8;
                RandomWalkPercent = 50;
                RandomWalkStartingPoint = Vector2Int.zero;
                RandomWalkMaxIterations = 1500;
                PathMinWidth = 2;
                PathMaxWidth = 4;
                PathDirectionChangeDistance = 2;
                PathWidthChangePercentage = 50;
                PathDirectionChangePercentage = 50;
                RandomWalkAvoiderPercent = 50;
                RandomWalkAvoiderStartingPoint = Vector2Int.zero;
                RandomWalkAvoiderObstaclesTilemap = null;
                RandomWalkAvoiderObstaclesDistance = 1;
                RandomWalkAvoiderMaxIterations = 100;
                BoundsTop = false;
                BoundsBottom = false;
                BoundsLeft = false;
                BoundsRight = false;
                PathStartPosition = Vector2Int.zero;
                PathDirection = MMGridGeneratorPath.Directions.BottomToTop;
                Initialized = true;
            }
        }

        /// <summary>
        ///     A struct used to store safe spots dimensions
        /// </summary>
        [Serializable]
        public struct MMTilemapGeneratorLayerSafeSpot
        {
            public Vector2Int Start;
            public Vector2Int End;
        }
    }
}