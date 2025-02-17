using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Polygen.HexagonGenerator {

	[ExecuteInEditMode]
	public class GeneratorObject : MonoBehaviour
	{
		public Transform terrainObj;

        //[ForceDrawInEditor]
        public GeneratorData generatorData;

        Dictionary<Vector2Int, Tile> gridCoordinateTiles = new Dictionary<Vector2Int, Tile>();
        Dictionary<HexVectorCubic, Tile> cubicCoordinateTiles = new Dictionary<HexVectorCubic, Tile>();

		void Awake()
        {
			enabled = false;
        }

		void OnDestroy()
		{
			if (generatorData != null)
			{
				generatorData.UnregisterUpdatedValues(OnValuesUpdated);
			}
		}

		public void Initialize(GeneratorData generatorData)
		{
			this.generatorData = generatorData;
            Generate(false);
            Initialize();
        }

		public void Initialize()
        {
            RegisterEvents();
        }

        public Tile GetTileWithGridCoordinates(Vector2Int coordinatesGrid)
        {
           return gridCoordinateTiles[coordinatesGrid];
        }

        public Tile GetTileWithCubicCoordinates(HexVectorCubic coordinatesCubic)
        {
            return cubicCoordinateTiles[coordinatesCubic];
        }

        private void RegisterEvents()
        {
            generatorData.UnregisterUpdatedValues(OnValuesUpdated);
            generatorData.RegisterUpdatedValues(OnValuesUpdated);
        }
        public void OnValuesUpdated()
        {
            Generate(true);
        }

        public void Generate(bool setDirty)
        {
            if (!ValidateGeneratorData()) return;

#if UNITY_EDITOR
            Stopwatch stopwatch = EditorBeginGenerate();
#endif

            ResetTerrain();

            if (generatorData.generateScripts || !DebugManager.debugNotActivated)
            {
                MapGenerator.GenerateMap(generatorData, terrainObj, gridCoordinateTiles, cubicCoordinateTiles);
            }
            else
            {
                MapGenerator.GenerateMap(generatorData, terrainObj);
            }

            TextureGenerator.UpdateColours(generatorData.graphicsSettings);
            UpdateMaterial();
            EditParentObject(generatorData.topografiaSettings.baseScale);

#if UNITY_EDITOR
            EditorEndGenerate(); 
#endif

            #region LocalFuncs
            bool ValidateGeneratorData()
            {
                if (generatorData == null)
                {
                    //TODO: Global editor name.
                    Debug.LogWarning("Generator could not found. Please check editor window for details.");
                    return false;
                }

                if (generatorData.topografiaSettings == null)
                {
                    Debug.LogWarning(nameof(TopografiaSettings) + " could not found. Please check editor window for details.");
                    return false;
                }

                if (generatorData.noiseSettings == null)
                {
                    Debug.LogWarning(nameof(NoiseFilter) + "could not found. Please check editor window for details.");
                    return false;
                }

                if (generatorData.graphicsSettings == null)
                {
                    Debug.LogWarning(nameof(GraphicsSettings) + "could not found. Please check editor window for details.");
                    return false;
                }

                return true;
            }

            void ResetTerrain()
            {
                gridCoordinateTiles = new Dictionary<Vector2Int, Tile>();
                cubicCoordinateTiles = new Dictionary<HexVectorCubic, Tile>();

                if (terrainObj != null)
                    DestroyImmediate(terrainObj.gameObject);

                terrainObj = new GameObject("Terrain").transform;

                terrainObj.SetParent(transform);
                terrainObj.localPosition = Vector3.zero;
            }

            void UpdateMaterial()
            {
                //TODO: Add shader to visualize coordinate system option(cubic&grid)
                //TODO: Selected tile highlighter
                //TODO: Coordinate axis highlighter
                generatorData.graphicsSettings.terrainMaterial.SetVector("_heightBounds", new Vector2(generatorData.topografiaSettings.seaLevel + 1, generatorData.topografiaSettings.heightScale + 1));
                generatorData.graphicsSettings.terrainMaterial.SetFloat("_mainSeaLevel", generatorData.topografiaSettings.seaLevel);
                generatorData.graphicsSettings.terrainMaterial.SetInt("_seaTiles", generatorData.topografiaSettings.seaTiles ? 1 : 0);
                generatorData.graphicsSettings.terrainMaterial.SetFloat("_baseScale", generatorData.topografiaSettings.baseScale);
            }

            void EditParentObject(float scale)
            {
                terrainObj.transform.localScale = Vector3.one * scale;
                terrainObj.transform.localEulerAngles = generatorData.topografiaSettings.rotation;
            }

#if UNITY_EDITOR
            Stopwatch EditorBeginGenerate()
            {
                Debug.Log("Generating object: " + name, gameObject);
                int maxProgress = (int)Mathf.Pow((generatorData.topografiaSettings.numTilePerEdge * 2) + 1, 2);
                EditorUtility.DisplayProgressBar("Generating map", "Generating tiles: " + maxProgress, .5f);
                return Stopwatch.StartNew();
            }

            void EditorEndGenerate()
            {
                if (setDirty)
                    EditorUtility.SetDirty(this);

                EditorUtility.ClearProgressBar();

                Debug.Log("Generation finished: <color=blue>" + (stopwatch.ElapsedMilliseconds) + " ms</color>", gameObject);
            }
#endif
            #endregion
        }
	}
}