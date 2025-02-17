using System;
using UnityEngine;

namespace Polygen.HexagonGenerator
{

	public static class HexMetrics
	{
		public const float outerRadius = 1f;

		public const float innerRadius = outerRadius * 0.866f;

		public static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
		};

		public static HexVectorCubic FromGridCoordinates(int x, int y)
		{
			return new HexVectorCubic(x - y / 2, y);
		}

	}

	/// <summary>
	/// Represents cubic coordinates in hexagon environment as they are three-dimensional and the topology resembles a cube.
	/// As these X and Y dimensions mirror each other, adding their coordinates together will always produces the same result, if you keep Z constant. In fact, if you add all three coordinates together you will always get zero. 
	/// If you increment one coordinate, you have to decrement another. Indeed, this produces six possible directions of movement.
	/// </summary>
	[Serializable]
	public struct HexVectorCubic
	{
		[SerializeField]
		private int gridX;
		public int x
        {
			get => gridX - z / 2;

		}

		public int z;

		public int y
        {
			get => -x - z;
        }

		public HexVectorCubic(int gridX, int gridZ)
        {
            this.gridX = gridX;
            this.z = gridZ;
        }

		public override string ToString()
		{
			return "x:" + x.ToString() + ", y:" + y.ToString() + ", z:" + z.ToString();
		}
	}


}