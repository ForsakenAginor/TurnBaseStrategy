using Polygen.HexagonGenerator;
using UnityEditor;
using UnityEngine;


namespace PolygenEditor
{
	[CustomEditor(typeof(GeneratorObject))]
	public class GeneratorObjectCustomEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Generate"))
				((GeneratorObject)target).Generate(true);

			if (GUILayout.Button("Open Editor Window"))
				HexagonGeneratorEditorWindow.Open();

			base.OnInspectorGUI();
		}
	}
}