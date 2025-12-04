using UnityEngine;

#nullable enable

namespace HellTiles.Player
{
    /// <summary>
    /// Applies the equipped skin sprite to the player.
    /// </summary>
    public class SkinApplier : MonoBehaviour
    {
        [System.Serializable]
        private class SkinEntry
        {
            public string skinId = "Default";
            public Sprite? sprite;
        }

        [SerializeField] private SpriteRenderer? targetRenderer;
        [SerializeField] private string defaultSkinId = "Default";
        [SerializeField] private SkinEntry[] skins = System.Array.Empty<SkinEntry>();

        private void Awake()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            ApplyEquippedSkin();
        }

        public void ApplyEquippedSkin()
        {
            if (targetRenderer == null)
            {
                return;
            }

            var equippedId = PlayerPrefs.GetString("hellTiles_equippedSkin", defaultSkinId);
            var sprite = FindSprite(equippedId) ?? FindSprite(defaultSkinId);
            if (sprite != null)
            {
                targetRenderer.sprite = sprite;
            }
        }

        private Sprite? FindSprite(string id)
        {
            if (skins == null)
            {
                return null;
            }

            foreach (var entry in skins)
            {
                if (entry != null && entry.skinId == id)
                {
                    return entry.sprite;
                }
            }

            return null;
        }
    }
}
