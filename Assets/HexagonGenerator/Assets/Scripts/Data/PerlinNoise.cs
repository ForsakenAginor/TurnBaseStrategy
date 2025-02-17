using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Polygen.HexagonGenerator
{

	[CreateAssetMenu(fileName = nameof(PerlinNoise), menuName = "POLYGEN/Settings/" + nameof(PerlinNoise))]

	public class PerlinNoise : NoiseFilter
	{
		[Range(1, 10)]
		public int octaves = 6;
		[Range(0, 1)]
		public float persistance = .6f;
		[Range(1, 10)]
		public float lacunarity = 2;

		public override float Evaluate(int seed, float x, float y, Vector2 sampleCenter, float baseScale)
		{
			seed = Mathf.Clamp(seed * 1000, int.MinValue, int.MaxValue);

			float noiseHeight = 0;

			float maxPossibleHeight = 0;
			float amplitude = 1;
			float frequency;

			float offsetX = seed + sampleCenter.x;
			float offsetY = seed + sampleCenter.y;


			for (int i = 0; i < octaves; i++)
			{
				maxPossibleHeight += amplitude;
				amplitude *= persistance;
			}

			amplitude = 1;
			frequency = 1;

			for (int i = 0; i < octaves; i++)
			{
				float sampleX = (x + offsetX) / scale * baseScale * frequency;
				float sampleY = (y * HexMetrics.innerRadius + offsetY) / scale * baseScale * frequency;

				float value = Mathf.Clamp01(noise.cnoise(new float2(sampleX, sampleY)));

				noiseHeight += value * amplitude;

				amplitude *= persistance;
				frequency *= lacunarity;
			}

			return Mathf.InverseLerp(0, 1, noiseHeight);
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			octaves = Mathf.Max(octaves, 1);
			lacunarity = Mathf.Max(lacunarity, 1);
			persistance = Mathf.Clamp01(persistance);
			base.OnValidate();
		}
#endif

	}
}
