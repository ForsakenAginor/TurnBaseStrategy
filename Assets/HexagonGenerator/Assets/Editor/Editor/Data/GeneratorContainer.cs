using Polygen.HexagonGenerator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PolygenEditor
{
	[ExecuteInEditMode, InitializeOnLoad]
	public class GeneratorContainer : ScriptableObject
	{
		public GeneratorDataSet generatorSet;
		public GeneratorSettings generatorSettings;

		[HideInInspector]public bool notAutoGenerate;

		public static GeneratorContainer instance;
        private void OnEnable()
        {
            instance = this;
        }


        public void Initialize()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			if (!ResourceManager.TryGetGeneratorsFolder(out string folder))
				return;

			var generatorGuids = AssetDatabase.FindAssets("t:GeneratorData", new[] { folder });

			if(instance.generatorSet == null)
            {
				ResourceManager.SetGeneratorDataSet();
            }

			foreach (var generatorGuid in generatorGuids)
			{
				string path = AssetDatabase.GUIDToAssetPath(generatorGuid);

				//Debug.Log(path);
				GeneratorData generatorData = AssetDatabase.LoadAssetAtPath<GeneratorData>(path);

				if(!instance.generatorSet.generators.Contains(generatorData))
					instance.generatorSet.generators.Add(generatorData);
			}
		}		

		public void Add(GeneratorData data) 
		{
			if(!generatorSet.generators.Contains(data))
				generatorSet.generators.Add(data);
			data.CreateGeneratorGameobject();
		}


		public void Remove(int index)
		{

			if (generatorSet.generators.Count < index)
				return;

			GeneratorData generatorData = generatorSet.generators[index];
			generatorSet.generators.Remove(generatorData);
			ResourceManager.RemoveGenerator(generatorData);

			AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(generatorData));

			DestroyImmediate(generatorData);
		}

		public GeneratorData Get(int index)
		{
			if (generatorSet.generators.Count < index)
			{
				Debug.LogError("Index is greater than array size!");
				return null;
			}

			if (0 > index)
			{
				Debug.LogError("Index is smaller than 0 and not valid!");
				return null;
			}

			return generatorSet.generators[index];
		}

	}
}
