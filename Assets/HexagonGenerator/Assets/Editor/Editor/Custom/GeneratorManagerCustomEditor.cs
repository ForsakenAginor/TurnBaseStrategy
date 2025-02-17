using Polygen.HexagonGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PolygenEditor
{
	[CustomEditor(typeof(GeneratorManager))]
	public class GeneratorManagerCustomEditor : Editor
	{

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Generate"))
				((GeneratorManager)target).Generate();

			base.OnInspectorGUI();
		}
	}
}

