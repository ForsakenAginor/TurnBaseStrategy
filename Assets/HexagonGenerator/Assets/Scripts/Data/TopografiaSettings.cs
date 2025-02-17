using UnityEngine;
using System.Collections;

namespace Polygen.HexagonGenerator
{

	[CreateAssetMenu(fileName = nameof(TopografiaSettings), menuName = "POLYGEN/Settings/" + nameof(TopografiaSettings))]
	public class TopografiaSettings : ScriptableObject
	{
		[Space(5)]
		public Vector2 offset;

		[Header("Tile Settings")]
		public int numTilePerEdge = 6;
		public GameObject tilePrefab;
		public GameObject seaPrefab;

		[Header("Transform Settings")]
		public float baseScale = 1;
		public float heightScale = 10;
		public float baseNoiseScale = 1;
		public Vector3 rotation = Vector3.zero;

		[Header("Sea Settings")]
		public bool seaTiles = false;
		public float seaLevel = 1;
		public float seaScale = 1;


		public void Initialize(GameObject tilePrefab, GameObject seaPrefab)
		{
			this.tilePrefab = tilePrefab;
			this.seaPrefab = seaPrefab;
		}

	}
}



