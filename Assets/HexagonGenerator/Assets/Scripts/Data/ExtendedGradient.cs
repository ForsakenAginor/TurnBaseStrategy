using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polygen.HexagonGenerator
{
    [System.Serializable]
    public class ExtendedGradient
    {
      
        public enum BlendMode { Linear, Fixed };
        public BlendMode blendMode;
        public bool randomizeColor = true;

        [Range(.1f,5f)]
        public float biomeBlendWeight = 1f;

        [SerializeField]
        List<BiomeKey> keys = new List<BiomeKey>();

        public ExtendedGradient()
        {
            AddKey(Color.white, 0);
            AddKey(Color.red, 0.5f);
            AddKey(Color.black, 1);
        }


        public Color Evaluate(float time)
        {
            BiomeKey keyLeft = keys[0];
            BiomeKey keyRight = keys[keys.Count - 1];

            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Time < time)
                {
                    keyLeft = keys[i];
                }
                if (keys[i].Time > time)
                {
                    keyRight = keys[i];
                    break;
                }
            }

            if (blendMode == BlendMode.Linear)
            {
                float blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
                return Color.Lerp(keyLeft.Color, keyRight.Color, blendTime * biomeBlendWeight);
            }

            return keyRight.Color;
        }

        //TODO: Add biome data weight to tiles and blending weight multiplier inbtw biomes to control blending weight. 
        public BiomeKey EvaluateKey(float time)
        {
            BiomeKey keyLeft = keys[0];
            BiomeKey keyRight = keys[keys.Count - 1];

            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Time < time)
                {
                    keyLeft = keys[i];
                }
                if (keys[i].Time > time)
                {
                    keyRight = keys[i];
                    break;
                }
            }

            if (blendMode == BlendMode.Linear)
            {
                float blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time) * biomeBlendWeight; 
                return Mathf.Abs(keyLeft.Time - blendTime) < Mathf.Abs(keyRight.Time - blendTime) ? keyLeft : keyRight;
            }

            return keyRight;
        }


        public int AddKey(BiomeData data, float time)
        {
            BiomeKey newKey = new BiomeKey(data, time, data.biomeBaseColor);
            for (int i = 0; i < keys.Count; i++)
            {
                if (newKey.Time < keys[i].Time)
                {
                    keys.Insert(i, newKey);
                    return i;
                }
            }

            keys.Add(newKey);
            return keys.Count - 1;
        }

        public int AddKey(Color color, float time)
        {
            BiomeKey newKey = new BiomeKey(null, time, color);
            for (int i = 0; i < keys.Count; i++)
            {
                if (newKey.Time < keys[i].Time)
                {
                    keys.Insert(i, newKey);
                    return i;
                }
            }

            keys.Add(newKey);
            return keys.Count - 1;
        }

        public void RemoveKey(int index)
        {
            if (keys.Count >= 2)
            {
                keys.RemoveAt(index);
            }
        }

        public int UpdateKeyTime(int index, float time)
        {
            BiomeKey key = keys[index];
            RemoveKey(index);
            return key.biomeData ? AddKey(key.biomeData, time) : AddKey(key.Color,time);
        }

        public void UpdateKeyColour(BiomeData biomeData, int index)
        {
            keys[index] = new BiomeKey(biomeData, keys[index].Time, Color.black);
        }

        public int NumKeys
        {
            get
            {
                return keys.Count;
            }
        }

        public BiomeKey GetKey(int i)
        {
            return keys[i];
        }

        public void SetKeyData(int i, BiomeData biomeData)
        {
            BiomeKey biomeKey = keys[i];
            biomeKey.biomeData = biomeData;
            keys[i] = biomeKey;
        }

        public void SetKeyColor(int i, Color color)
        {
            BiomeKey biomeKey = keys[i];
            biomeKey.Color = color;
            keys[i] = biomeKey;
        }


        public Texture2D GetTexture(int width, bool horizontal = true)
        {
            if (horizontal)
            {
                Texture2D texture = new Texture2D(width, 1);
                Color[] colours = new Color[width];
                for (int i = 0; i < width; i++)
                {
                    colours[i] = Evaluate((float)i / (width - 1));
                }
                texture.SetPixels(colours);
                texture.Apply();
                return texture;
            }
            else
            {
                Texture2D texture = new Texture2D(1, width);
                Color[] colours = new Color[width];
                for (int i = 0; i < width; i++)
                {
                    colours[i] = Evaluate((float)i / (width - 1));
                }
                texture.SetPixels(colours);
                texture.Apply();
                return texture;
            }

        }

        public void Reset()
		{
            keys = new List<BiomeKey>();
            AddKey(Color.white, 0);
            AddKey(Color.red, 0.5f);
            AddKey(Color.black, 1);
        }

       
        [System.Serializable]
        public struct BiomeKey
        {
            [SerializeField]
            public BiomeData biomeData;
            [SerializeField]
            Color color;
            [SerializeField]
            float time;

            public BiomeKey(BiomeData biomeData, float time, Color color)
            {
                this.biomeData = biomeData;
                this.color = color;
                this.time = time;
            }

            public Color Color
            {
                get
                {
                    return biomeData ? biomeData.biomeBaseColor : color;
                }

                set
				{
                    color = value;
				}
            }

            public float Time
            {
                get
                {
                    return time;
                }
            }
        }

    }
}
