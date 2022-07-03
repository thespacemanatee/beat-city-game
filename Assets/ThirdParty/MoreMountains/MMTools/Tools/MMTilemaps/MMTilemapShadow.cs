﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     A class to put on a tilemap so it acts as a shadow/copy of another reference tilemap.
    ///     Useful for wall shadows for example.
    ///     Offsetting the tilemap and changing its sorting order etc is done via the regular components
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("More Mountains/Tools/Tilemaps/MMTilemapShadow")]
    [RequireComponent(typeof(Tilemap))]
    public class MMTilemapShadow : MonoBehaviour
    {
        /// the tilemap to copy
        public Tilemap ReferenceTilemap;

        [MMInspectorButton("UpdateShadows")] public bool UpdateShadowButton;

        protected Tilemap _tilemap;

        /// <summary>
        ///     This method will copy the reference tilemap into the one on this gameobject
        /// </summary>
        public virtual void UpdateShadows()
        {
            if (ReferenceTilemap == null) return;

            _tilemap = gameObject.GetComponent<Tilemap>();

            Copy(ReferenceTilemap, _tilemap);
        }

        /// <summary>
        ///     Copies the source tilemap on the destination tilemap
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void Copy(Tilemap source, Tilemap destination)
        {
            source.RefreshAllTiles();
            destination.RefreshAllTiles();

            var referenceTilemapPositions = new List<Vector3Int>();

            // we grab all filled positions from the ref tilemap
            foreach (var pos in source.cellBounds.allPositionsWithin)
            {
                var localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (source.HasTile(localPlace)) referenceTilemapPositions.Add(localPlace);
            }

            // we turn our list into an array
            var positions = new Vector3Int[referenceTilemapPositions.Count];
            var allTiles = new TileBase[referenceTilemapPositions.Count];
            var i = 0;
            foreach (var tilePosition in referenceTilemapPositions)
            {
                positions[i] = tilePosition;
                allTiles[i] = source.GetTile(tilePosition);
                i++;
            }

            // we clear our tilemap and resize it
            destination.ClearAllTiles();
            destination.RefreshAllTiles();
            destination.size = source.size;
            destination.origin = source.origin;
            destination.ResizeBounds();

            // we feed it our positions
            destination.SetTiles(positions, allTiles);
        }
    }
}