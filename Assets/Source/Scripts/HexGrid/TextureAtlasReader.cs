using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HexGrid
{
    [RequireComponent(typeof(MeshRenderer))]
    public class TextureAtlasReader : MonoBehaviour
    {
        [SerializeField] private SpriteUV1[] _spritesUV;

        private Dictionary<CellSprite, UVCoordinates> _spriteUVDictionary;

        internal IReadOnlyDictionary<CellSprite, UVCoordinates> GetUVDictionary()
        {
            if (_spriteUVDictionary == null)
                SetUVDictionary();

            return _spriteUVDictionary;
        }

        private void SetUVDictionary()
        {
            _spriteUVDictionary = new Dictionary<CellSprite, UVCoordinates>();
            Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
            float width = texture.width;
            float height = texture.height;

            foreach (SpriteUV1 sprite in _spritesUV)
            {
                UVCoordinates uv = new ()
                {
                    UV00 = new Vector2(sprite.CoordinatesUV00.x / width, sprite.CoordinatesUV00.y / height),
                    UV11 = new Vector2(sprite.CoordinatesUV11.x / width, sprite.CoordinatesUV11.y / height),
                };
                _spriteUVDictionary.Add(sprite.Sprite, uv);
            }
        }

        [Serializable]
        private struct SpriteUV1
        {
            public CellSprite Sprite;
            public Vector2 CoordinatesUV00;
            public Vector2 CoordinatesUV11;
        }
    }
}