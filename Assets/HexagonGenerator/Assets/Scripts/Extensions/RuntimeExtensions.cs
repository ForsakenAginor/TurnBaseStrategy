using Polygen.HexagonGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Polygen.Extensions
{
	public static class RuntimeExtensions
	{
        /// <summary> Converts given bitmask to layer number </summary>
        /// <returns> layer number </returns>
        public static int ToLayer(this LayerMask bitmask)
        {
            int result = bitmask > 0 ? 0 : 31;
            while (bitmask > 1)
            {
                bitmask = bitmask >> 1;
                result++;
            }
            return result;
        }

        public static int CustomIndexOf(this string source, char toFind, int position)
		{
			int index = -1;
			for (int i = 0; i < position; i++)
			{
				index = source.IndexOf(toFind, index + 1);

				if (index == -1)
					break;
			}

			return index;
		}


        public static T GetRandomFromArray<T>(this T[] array) where T : class
		{
            if (array.Length < 1)
                return null;
            return array[UnityEngine.Random.Range(0, array.Length)];
		}

        public static BiomeData.DetailData GetRandomWeightedPrefab(this BiomeData.DetailData[] details, float density)
        {
            if (details == null || details.Length == 0) return null;

            int i = 0;
            float[] weights = new float[details.Length];

            for (i = 0; i < weights.Length; i++)
			{
                weights[i] = details[i].density;
			}

            float w;
            float t = 0;
            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];

                if (float.IsPositiveInfinity(w))
                {
                    return details[i];
                }
                else if (w >= 0f && !float.IsNaN(w))
                {
                    t += weights[i];
                }
            }

            float r = UnityEngine.Random.value;
            float s = 0f;

            if (r > density)
                return null;

            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];
                if (float.IsNaN(w) || w <= 0f) continue;

                s += w / t;
                if (s >= r) return details[i];
            }

            return null;
        }

        public static BiomeData.DetailData GetRandomWeightedPrefab(this BiomeData.DetailData[] details, float x, float y, int seed, float density, float scale = 1.1f)
        {

            if (details == null || details.Length == 0) return null;

            int i = 0;
            float[] weights = new float[details.Length];

            for (i = 0; i < weights.Length; i++)
            {
                weights[i] = details[i].density;
            }

            float w;
            float t = 0;
            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];

                if (float.IsPositiveInfinity(w))
                {
                    return details[i];
                }
                else if (w >= 0f && !float.IsNaN(w))
                {
                    t += weights[i];
                }
            }

            float r = Mathf.PerlinNoise((x + seed) / scale, (y + seed) / scale);
            float s = 0f;

            if (r > density)
                return null;

            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];
                if (float.IsNaN(w) || w <= 0f) continue;

                s += w / t;
                if (s >= r) return details[i];
            }

            return null;
        }

        public static int GetRandomWeightedVariant(int maxVariantCount, float variation, float x, float y, int seed, float scale = 1.1f)
        {
            int i = 0;

            float w;
            float t = 0;
            for (i = 0; i < maxVariantCount; i++)
            {
                w = i;

                if (float.IsPositiveInfinity(w))
                {
                    return i;
                }
                else if (w >= 0f && !float.IsNaN(w))
                {
                    t += i;
                }
            }

            float r = Mathf.PerlinNoise((x + seed) / scale, (y + seed) / scale);
            float s = 0f;

            for (i = 0; i < maxVariantCount; i++)
            {
                w = i;
                if (float.IsNaN(w) || w <= 0f) continue;

                s += w / t;
                if (s >= r) return i;
            }

            return 0;
        }
    }
}
