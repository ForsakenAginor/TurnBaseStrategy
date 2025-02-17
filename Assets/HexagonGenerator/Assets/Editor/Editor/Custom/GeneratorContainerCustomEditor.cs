using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Polygen.HexagonGenerator;

namespace PolygenEditor
{
	public class AssetHandler
	{
		[OnOpenAsset()]
		public static bool OpenEditor(int id, int line)
		{
			GeneratorContainer obj = EditorUtility.InstanceIDToObject(id) as GeneratorContainer;

			if (obj != null)
			{
				HexagonGeneratorEditorWindow.Open();
				return true;
			}

			return false;
		}
	}

	[CustomEditor(typeof(GeneratorContainer))]
	public class GeneratorContainerCustomEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Editor Window"))
				HexagonGeneratorEditorWindow.Open();

			base.OnInspectorGUI();
		}
	}
	
}