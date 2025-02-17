using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Polygen.HexagonGenerator
{
    public class Tile : MonoBehaviour
    {

        public float height;

        //TODO: Add neighbour tiles array

        BiomeData biomeData;
        public BiomeData BiomeData
        {
            get => biomeData;
        }

        Color color;
        public Color Color
        {
            get => color;
        }
        

        Vector2Int coordinatesGrid;
        public Vector2Int CoordinatesGrid
        {
            get => coordinatesGrid;
        }

        HexVectorCubic coordinatesCubic;
        public HexVectorCubic CoordinatesCubic
        {
            get => coordinatesCubic;
        }

        public void Initialize(BiomeData biomeData, Vector2Int coordinatesGrid, HexVectorCubic coordinatesCubic, float height, Color color)
        {
            this.biomeData = biomeData;
            this.coordinatesGrid = coordinatesGrid;
            this.coordinatesCubic = coordinatesCubic;

            this.height = height;
            this.color = color;

            if (DebugManager.debugNotActivated)
                return;

            TMPro.TextMeshPro textMeshPro = Instantiate(DebugManager.debugText, transform).GetComponent<TMPro.TextMeshPro>();

            switch (DebugManager.debugMode)
            {
                case DebugManager.DebugMode.Default:
                    textMeshPro.text = biomeData.biomeName;
                    break;
                case DebugManager.DebugMode.Cubic:
                    textMeshPro.text = coordinatesCubic.ToString();
                    textMeshPro.rectTransform.sizeDelta = new Vector2(1.6f, textMeshPro.rectTransform.sizeDelta.y);
                    break;
                case DebugManager.DebugMode.Grid:
                    textMeshPro.text = coordinatesGrid.ToString();
                    break;
                case DebugManager.DebugMode.Height:
                    textMeshPro.text = height.ToString("#.##");
                    break;
                default:
                    break;
            }
            //textMeshPro.transform.localScale = new Vector3(1, 1 / transform.localScale.y, 1);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            //TODO: Respond height changes and detect correct biome based on height value.
        }
#endif
    }
}
