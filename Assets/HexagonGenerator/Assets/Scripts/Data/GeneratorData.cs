using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polygen.HexagonGenerator
{
	[System.Serializable, ExecuteInEditMode]
	public class GeneratorData : UpdatableData
	{
		[Delayed]
		public string generatorName;

		[Header("Settings Assets")]
		//TODO: Add tooltip to settings and their children properties.
		[ForceDrawInEditor]
		public TopografiaSettings topografiaSettings;
		[ForceDrawInEditor]
		public NoiseFilter noiseSettings;
		[ForceDrawInEditor]
		public GraphicsSettings graphicsSettings;

		[Delayed, Tooltip("Base seed for generator")]
		public int seed;

		[Header("Generator Options")]
		[Tooltip("Select whether generating monobehaviour components on tiles")]
		public bool generateScripts = false;
		[Tooltip("Select whether generating mesh collider components on tiles"), Space(26)]
		public bool generateTileColliders = false;
		[Tooltip("Select whether generating mesh collider components on detail gameobjects"), Space(26)]
		public bool generateDetailColliders = false;
		[Tooltip("Select whether generated tile gameobjects marked as static"), Space(26)]
		public bool staticTiles = true;
		[Tooltip("Select whether generated detail gameobjects marked as static"), Space(26)]
		public bool staticDetails = true;
		
		[HideInInspector] public int tileLayer;
		[HideInInspector] public int detailLayer;

		[HideInInspector] public string generatorFolderPath;

		public void Initialize(GeneratorSettings generatorSettings, string generatorName, string generatorFolder)
		{
			name = generatorName;
			this.generatorName = name;
			generatorFolderPath = generatorFolder;
			tileLayer = LayerMask.GetMask("Default");
			detailLayer = LayerMask.GetMask("Default");

			topografiaSettings = CreateInstance<TopografiaSettings>();
			topografiaSettings.Initialize(generatorSettings.tilePrefab, generatorSettings.seaPrefab);

			noiseSettings = CreateInstance<PerlinNoise>();

			graphicsSettings = CreateInstance<GraphicsSettings>();
			graphicsSettings.terrainMaterial = new Material(generatorSettings.terrainMaterial);
			graphicsSettings.waterMaterial = new Material(generatorSettings.waterMaterial);
			graphicsSettings.texture2D = new Texture2D(TextureGenerator.textureRes, 1);
			graphicsSettings.texture2D.filterMode = FilterMode.Point;
		}

		public void SetRandomSeed()
		{
			seed = Random.Range(-10000, 10000);
		}

		public GameObject CreateGeneratorGameobject()
		{
			GameObject generatedObj = new GameObject(generatorName);
			GeneratorObject generatorObject = generatedObj.AddComponent<GeneratorObject>();
			generatorObject.Initialize(this);
			return generatedObj;
		}

		public Color GetColorByHeight(float tileHeight)
        {
			return graphicsSettings.gradient.Evaluate(tileHeight);
		}

#if UNITY_EDITOR
		private void OnValidate()
        {
			seed = Mathf.Clamp(seed, -10000, 10000);
        }
#endif
	}
}