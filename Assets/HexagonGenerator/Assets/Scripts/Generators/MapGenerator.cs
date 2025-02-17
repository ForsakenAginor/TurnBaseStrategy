using Polygen.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Polygen.HexagonGenerator
{
	public class MapGenerator
    {
        
        //Generate topografia only
        public static void GenerateTopografia(GeneratorData data, Transform parent)
        {
            if (!data.topografiaSettings.seaTiles)
            {
                GenerateSea(data, parent);
            }

            for (int z = -data.topografiaSettings.numTilePerEdge; z <= data.topografiaSettings.numTilePerEdge; z++)
            {
                for (int x = data.topografiaSettings.numTilePerEdge; x >= -data.topografiaSettings.numTilePerEdge; x--)
                {
                    if ((z + x < -data.topografiaSettings.numTilePerEdge) || (z + x > data.topografiaSettings.numTilePerEdge))
                    {
                        continue;
                    }

                    Vector3 position = CreatePositionVector(data, parent, z, x);

                    bool isSeaTile = position.y < data.topografiaSettings.seaLevel;

                    if (isSeaTile && !data.topografiaSettings.seaTiles)
                    {
                        continue;
                    }

                    //Instantiate tile gameobject at given position
                    GameObject tileObj = InstantiateTile(data, parent, z, x, position, isSeaTile);

                    //Create tile coordinates and set tile name
                    HexVectorCubic coordinatesCubic = new HexVectorCubic(x, z);
                    tileObj.name = "Tile_ " + coordinatesCubic.x + "_" + coordinatesCubic.y + "_" + coordinatesCubic.z;
                }
            }
        }

        //Generate topografia with details
        public static void GenerateMap(GeneratorData data, Transform parent)
        {
            Initialize(data, parent, out Vector2 heightBounds, out GameObject detailParent);

            if (!data.topografiaSettings.seaTiles)
            {
                GenerateSea(data, parent);
            }

            for (int z = -data.topografiaSettings.numTilePerEdge; z <= data.topografiaSettings.numTilePerEdge; z++)
            {
                for (int x = data.topografiaSettings.numTilePerEdge; x >= -data.topografiaSettings.numTilePerEdge; x--)
                {
                    //To create hexagon map shape
                    if ((z + x < -data.topografiaSettings.numTilePerEdge) || (z + x > data.topografiaSettings.numTilePerEdge))
                    {
                        continue;
                    }

                    Vector3 position = CreatePositionVector(data, parent, z, x);

                    bool isSeaTile = position.y < data.topografiaSettings.seaLevel;

                    if (isSeaTile && !data.topografiaSettings.seaTiles)
                    {
                        continue;
                    }

                    //Instantiate tile gameobject at given position
                    GameObject tileObj = InstantiateTile(data, parent, z, x, position, isSeaTile);

                    //Create tile coordinates and set tile name
                    HexVectorCubic coordinatesCubic = new HexVectorCubic(x, z);
                    tileObj.name = "Tile_ " + coordinatesCubic.x + "_" + coordinatesCubic.y + "_" + coordinatesCubic.z;

                    //Get biome data for tile
                    BiomeData biomeData = GetBiomeData(data, heightBounds, position, isSeaTile);

                    //Generate details for tile
                    InstantiateDetails(data, detailParent.transform, z, x, position, biomeData, isSeaTile);
                }
            }
        }

       

        //Generate map with details and scripts
        public static void GenerateMap(GeneratorData data, Transform parent, Dictionary<Vector2Int,Tile> gridDictionary, Dictionary<HexVectorCubic,Tile> cubicDictionary)
        {
            Initialize(data, parent, out Vector2 heightBounds, out GameObject detailParent);

            //Generate Sea if selected
            GenerateSea(data, parent);

            for (int z = -data.topografiaSettings.numTilePerEdge; z <= data.topografiaSettings.numTilePerEdge; z++)
            {
                for (int x = data.topografiaSettings.numTilePerEdge; x >= -data.topografiaSettings.numTilePerEdge; x--)
                {
                    //To create hexagon map shape
                    if ((z + x < -data.topografiaSettings.numTilePerEdge) || (z + x > data.topografiaSettings.numTilePerEdge))
                    {
                        continue;
                    }

                    Vector3 position = CreatePositionVector(data, parent, z, x);

                    bool isSeaTile = position.y < data.topografiaSettings.seaLevel;

                    if (isSeaTile && !data.topografiaSettings.seaTiles)
                    {
                        continue;
                    }

                    //Instantiate tile gameobject at given position
                    GameObject tileObj = InstantiateTile(data, parent, z, x, position, isSeaTile);

                    //Create tile coordinates and set tile name
                    HexVectorCubic coordinatesCubic = new HexVectorCubic(x, z);
                    Vector2Int coordinatesGrid = new Vector2Int(x, z);
                    tileObj.name = "Tile_ " + coordinatesCubic.x + "_" + coordinatesCubic.y + "_" + coordinatesCubic.z;

                    //Get biome data for tile
                    BiomeData biomeData = GetBiomeData(data, heightBounds, position, isSeaTile, out float tileHeight);

                    //Generate details for tile
                    InstantiateDetails(data, detailParent.transform, z, x, position, biomeData, isSeaTile);

                    //Assign and init HexagonTile monobehaviour component
                    HexagonTile tile = GenerateTileScript(position, isSeaTile, tileObj, coordinatesCubic, coordinatesGrid, biomeData, tileHeight);

                    //Register to tile dictionaries
                    gridDictionary.Add(coordinatesGrid, tile);
                    cubicDictionary.Add(coordinatesCubic, tile);
                }
            }

            HexagonTile GenerateTileScript(Vector3 position, bool isSeaTile, GameObject tileObj, HexVectorCubic coordinatesCubic, Vector2Int coordinatesGrid, BiomeData biomeData, float tileHeight)
            {
                HexagonTile tile = tileObj.AddComponent<HexagonTile>();
                tile.Initialize(biomeData, coordinatesGrid, coordinatesCubic, position.y, GetTileColor());
                return tile;

                Color GetTileColor()
                {
                    return isSeaTile ? data.graphicsSettings.waterBaseColor : data.GetColorByHeight(tileHeight);
                }
            }
        }

        #region Helpers

        private static void Initialize(GeneratorData data, Transform parent, out Vector2 heightBounds, out GameObject detailParent)
        {
            heightBounds = new Vector2(data.topografiaSettings.seaLevel, data.topografiaSettings.heightScale);
            detailParent = new GameObject("Detail Parent");
            detailParent.transform.SetParent(parent);
            detailParent.transform.localPosition = Vector3.zero;
        }
        private static void GenerateSea(GeneratorData data, Transform parent)
        {
            if (!data.topografiaSettings.seaTiles)
            {
                GameObject sea = Object.Instantiate(data.topografiaSettings.seaPrefab, parent);
                sea.GetComponent<Renderer>().sharedMaterial = data.graphicsSettings.waterMaterial;

                sea.name = "Sea";
                sea.transform.localScale += (Vector3.right + Vector3.forward) * (data.topografiaSettings.numTilePerEdge - .5f) * (HexMetrics.innerRadius * 2) * data.topografiaSettings.seaScale;
                sea.transform.localScale += Vector3.up * data.topografiaSettings.seaLevel;
                sea.transform.localPosition += Vector3.up * data.topografiaSettings.seaLevel;
                sea.transform.eulerAngles += Vector3.up * 30;
            }
        }
        private static Vector3 CreatePositionVector(GeneratorData data, Transform parent, int z, int x)
        {
            Vector3 position = parent.localPosition;
            position.x += (x + z * 0.5f) * (HexMetrics.innerRadius * 2f);
            position.z += z * (HexMetrics.outerRadius * 1.5f);
            float height = data.noiseSettings.Evaluate(data.seed, x, z, data.topografiaSettings.offset, data.topografiaSettings.baseNoiseScale);
            position.y += height * data.topografiaSettings.heightScale;
            return position;
        }
        private static BiomeData GetBiomeData(GeneratorData data, Vector2 heightBounds, Vector3 position, bool isSeaTile, out float tileHeight)
        {
            tileHeight = Mathf.InverseLerp(heightBounds.x, heightBounds.y, position.y);
            BiomeData biomeData = isSeaTile ? data.graphicsSettings.seaBiomeData : data.graphicsSettings.GetBiomeData(tileHeight);
            return biomeData;
        }
        private static BiomeData GetBiomeData(GeneratorData data, Vector2 heightBounds, Vector3 position, bool isSeaTile)
        {
            float tileHeight = Mathf.InverseLerp(heightBounds.x, heightBounds.y, position.y);
            BiomeData biomeData = isSeaTile ? data.graphicsSettings.seaBiomeData : data.graphicsSettings.GetBiomeData(tileHeight);
            return biomeData;
        }
        private static GameObject InstantiateTile(GeneratorData data, Transform parent, int z, int x, Vector3 position, bool isSeaTile)
        {
            //Create local variable for tile gameobject
            GameObject tileObj;

            //Instantiate tile gameobject and assign parent transform and material
            if (isSeaTile)
            {
                position.y = data.topografiaSettings.seaLevel;
                tileObj = Object.Instantiate(data.topografiaSettings.seaPrefab, parent);
                tileObj.GetComponentInChildren<Renderer>().sharedMaterial = data.graphicsSettings.waterMaterial;
            }
            else
            {
                tileObj = Object.Instantiate(data.topografiaSettings.tilePrefab, parent);
                tileObj.GetComponentInChildren<Renderer>().sharedMaterial = data.graphicsSettings.debugMaterial ? data.graphicsSettings.debugMaterial : data.graphicsSettings.terrainMaterial;
            }

            //Set height and position for tile
            tileObj.transform.localScale += Vector3.up * position.y;
            tileObj.transform.localPosition = position;

            //Add collider if selected
            if (data.generateTileColliders)
            {
                tileObj.GetComponentInChildren<MeshFilter>().gameObject.AddComponent<MeshCollider>();
            }

            tileObj.layer = data.tileLayer;
            tileObj.isStatic = data.staticTiles;

            return tileObj;
        }
        private static void InstantiateDetails(GeneratorData data, Transform parent, int z, int x, Vector3 position, BiomeData biomeData, bool isSeaTile)
        {
            if (biomeData != null && biomeData.details != null)
            {
                BiomeData.DetailData detailData = data.graphicsSettings.fixedDetails ? biomeData.details.GetRandomWeightedPrefab(x, z, data.seed, biomeData.detailDensity * data.graphicsSettings.baseDetailDensity * 2) : biomeData.details.GetRandomWeightedPrefab(biomeData.detailDensity * data.graphicsSettings.baseDetailDensity * 2);
                if (detailData != null && detailData.detailPrefab != null)
                {
                    if (data.noiseSettings.Evaluate(data.graphicsSettings.baseDetailSeed + data.seed, x, z, data.topografiaSettings.offset + data.graphicsSettings.detailOffset, data.graphicsSettings.baseDetailNoiseScale * biomeData.biomeDetailNoiseScale) < detailData.density * biomeData.detailDensity * data.graphicsSettings.baseDetailDensity)
                    {
                        int variantCount = Random.Range(0, detailData.maxVariantCount);
                        for (int i = 0; i <= variantCount; i++)
                        {
                            GameObject detailObj = Object.Instantiate(detailData.detailPrefab, parent);
                            detailObj.transform.localScale = Vector3.one * data.graphicsSettings.baseDetailScale * biomeData.biomeDetailScale * detailData.detailScale;
                            detailObj.transform.localScale += new Vector3(detailData.variantOffset, detailData.variantOffset, detailData.variantOffset) * Random.Range(-.1f, .1f);
                            detailObj.transform.localEulerAngles += new Vector3(detailData.variantOffset, detailData.variantOffset, detailData.variantOffset) * Random.Range(-5, 5);
                            
                            if(isSeaTile)
                                detailObj.transform.localPosition = new Vector3(position.x, data.topografiaSettings.seaLevel, position.z) + new Vector3(detailData.variantOffset * Random.Range(-.5f, .5f), detailData.verticalOffset, detailData.variantOffset * Random.Range(-.5f, .5f));
                            else
                                detailObj.transform.localPosition = position + new Vector3(detailData.variantOffset * Random.Range(-.5f, .5f), detailData.verticalOffset, detailData.variantOffset * Random.Range(-.5f, .5f));

                            if (detailData.randomizeRotation)
                                detailObj.transform.localEulerAngles += Vector3.up * Random.Range(0, 360);

                            if (data.generateDetailColliders)
                                detailObj.AddComponent<MeshCollider>();

                            detailObj.layer = data.detailLayer;
                            detailObj.isStatic = data.staticDetails;
                        }
                    }
                }
            }
        }

        #endregion
    }
}