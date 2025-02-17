
using UnityEngine;

namespace Polygen.HexagonGenerator
{
	[System.Serializable]
	public static class DebugManager
	{
		[SerializeField] public static GameObject debugText;
		[SerializeField] public static bool debugNotActivated = true;
		[SerializeField] public static DebugMode debugMode;

		public enum DebugMode
		{
			Default,
			Cubic,
			Grid,
			Height,
			//Biome
		}

	}
}
