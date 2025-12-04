using System.Collections.Generic;
using UnityEngine;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Hazards
{
    public class PushTileSpawner : MonoBehaviour
    {
        [SerializeField] private TileGridController gridController = default!;
        [SerializeField] private PushTileHazard pushPrefab = default!;
        [SerializeField, Tooltip("Assign sprites in order: Up, Down, Left, Right.")] private Sprite[] directionSprites = System.Array.Empty<Sprite>();
        [SerializeField, Tooltip("Seconds between spawn attempts.")] private float spawnInterval = 10f;
        [SerializeField, Tooltip("Seconds before first spawn.")] private float initialDelay = 4f;
        [SerializeField, Tooltip("Max push pads alive.")] private int maxSimultaneous = 2;

        private readonly Dictionary<Vector3Int, PushTileHazard> activePads = new();
        private readonly List<Vector3Int> walkableCells = new();
        private float timer;

        private void Awake()
        {
            if (gridController == null)
            {
                gridController = FindFirstObjectByType<TileGridController>();
            }

            timer = Mathf.Max(0f, initialDelay);
        }

        private void Start()
        {
            CacheWalkable();
        }

        private void Update()
        {
            if (pushPrefab == null || gridController == null)
            {
                return;
            }

            if (activePads.Count >= maxSimultaneous)
            {
                return;
            }

            timer -= Time.deltaTime;
            if (timer > 0f)
            {
                return;
            }

            Spawn();
            timer = Mathf.Max(0.01f, spawnInterval);
        }

        private void CacheWalkable()
        {
            walkableCells.Clear();
            foreach (var cell in gridController.EnumerateWalkableCells())
            {
                walkableCells.Add(cell);
            }
        }

        private void Spawn()
        {
            if (walkableCells.Count == 0)
            {
                CacheWalkable();
                if (walkableCells.Count == 0)
                {
                    return;
                }
            }

            var attempts = walkableCells.Count;
            while (attempts-- > 0)
            {
                var cell = walkableCells[Random.Range(0, walkableCells.Count)];
                if (activePads.ContainsKey(cell))
                {
                    continue;
                }

                if (!gridController.WalkableTilemap.HasTile(cell))
                {
                    continue;
                }

                var pad = Instantiate(pushPrefab, gridController.CellToWorldCenter(cell), Quaternion.identity, transform);
                pad.Initialise(gridController, cell);

                // Pick direction randomly and set sprite if provided.
                var dirIndex = Random.Range(0, 4);
                SetDirectionAndSprite(pad, dirIndex);

                activePads[cell] = pad;
                return;
            }
        }

        private void SetDirectionAndSprite(PushTileHazard pad, int dirIndex)
        {
            var direction = (PushTileHazard.PushDirection)dirIndex;

            // Assign sprite if available
            if (directionSprites != null && directionSprites.Length > dirIndex)
            {
                var renderer = pad.GetComponentInChildren<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sprite = directionSprites[dirIndex];
                }
            }

            // Adjust rotation? sprites supplied per direction, so none needed.

            // store direction on pad
            SetPadDirection(pad, direction);
        }

        private void SetPadDirection(PushTileHazard pad, PushTileHazard.PushDirection direction)
        {
            var setter = pad as IPushPadConfigurable;
            setter?.SetDirection(direction);
        }
    }

    public interface IPushPadConfigurable
    {
        void SetDirection(PushTileHazard.PushDirection direction);
    }
}
