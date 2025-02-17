using Polygen.HexagonGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PolygenEditor
{
	[CustomEditor(typeof(Tile), true)]
	public class TileCustomEditor : Editor
	{
        GUIStyle style;
        Tile targetTile;

        private void OnEnable()
        {
            style = new GUIStyle { richText = true };
            targetTile = (serializedObject.targetObject as Tile);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("<b>Height: </b>" + targetTile.height, style);
            EditorGUILayout.LabelField("<b>Biome: </b>" + targetTile.BiomeData.biomeName, style);
            EditorGUILayout.LabelField("<b>Cubic Coordinates: </b>" + targetTile.CoordinatesCubic.ToString(), style);
            EditorGUILayout.LabelField("<b>Grid Coordinates: </b>" + targetTile.CoordinatesGrid.ToString(), style);
            EditorGUILayout.ColorField("Tile Color", targetTile.Color);
        }

    }
}
