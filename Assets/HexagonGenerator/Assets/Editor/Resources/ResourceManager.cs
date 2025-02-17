using Polygen.HexagonGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PolygenEditor
{
	[InitializeOnLoad]
	public static class ResourceManager
	{
		static ResourceManager()
		{
			//checking if it is initial startup
			if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			AssetDatabase.Refresh();

			Initialize();

			//registering callbacks
			//EditorApplication.playModeStateChanged += PlayModeStateChanged;
			EditorApplication.update += Initialize;
			EditorSceneManager.sceneSaved += EventManager.OnSceneSaved;			
		}


		public static void Initialize()
		{
			EditorApplication.update -= Initialize;

			SetGeneratorContainerInstance();
			SetGeneratorDataSet();
			InitializeGeneratorObjects();
			InitializeDebugManager();

			//registering callbacks
			EventManager.RegisterEditorWindowOpen(GeneratorContainer.instance.Initialize);
			EventManager.RegisterDatabaseRefresh(GeneratorContainer.instance.Initialize);
			EventManager.RegisterInitializeOnLaunch(GeneratorContainer.instance.Initialize);

			//global initialize callback
			EventManager.OnInitializeOnLaunch();

			void InitializeDebugManager()
			{
				if (TryGetDebugFolder(out string folder))
				{
					DebugManager.debugText = AssetDatabase.LoadMainAssetAtPath(folder + "/DebugText.prefab") as GameObject;
				}
				else
				{
					Debug.LogWarning("DebugText prefab could not be loaded! Debug Mode may not work properly!");
				}
			}

			void InitializeGeneratorObjects()
			{
				GeneratorObject[] objs = Object.FindObjectsOfType<GeneratorObject>();

				foreach (var obj in objs)
				{
					obj.Initialize();
				}
			}
		}

		/*
		private static void PlayModeStateChanged(PlayModeStateChange state)
		{
			Debug.Log(state);
		}*/

		public static void SetGeneratorContainerInstance()
		{
			//search for a ScriptObject instance called GeneratorContainer
			string[] guids = AssetDatabase.FindAssets("t:" + nameof(GeneratorContainer));

			//checking for generator container instances
			if (guids.Length == 0)
			{
				Debug.LogWarning("Generator Container instance is not found. Please try to re-open generator window and make sure HexagonGenerator folder includes generator container scriptable object asset called as Hexagon Generator");
				//GeneratorContainer.instance = ScriptableObject.CreateInstance<GeneratorContainer>();
				//GeneratorContainer.instance.name = "Hexagon Generator";
				//CreateAssetAtResources(GeneratorContainer.instance);
			}
            else
			{
				//checking for multiple generator container instances
				if (guids.Length > 1)
				{
					Debug.LogWarning("There should not be multiple generator containers! Please check the assets for multiple instances.");
				}
				//assigning instance
				GeneratorContainer.instance = AssetDatabase.LoadAssetAtPath<GeneratorContainer>(AssetDatabase.GUIDToAssetPath(guids[0]));
			}			
		}

		//TODO: SetGeneratorDataSet metodunu SetGeneratorContainerInstance gibi düzenle
		public static void SetGeneratorDataSet()
		{
			//search for a ScriptObject instance called GeneratorContainer
			string[] guids = AssetDatabase.FindAssets("t:" + nameof(GeneratorDataSet));


			//checking for generator data sets
			if (guids.Length == 0)
			{
				Debug.LogWarning("GeneratorDataSet could not be found! Creating an instance, there should be one instance of data set.");

				GeneratorContainer.instance.generatorSet = ScriptableObject.CreateInstance<GeneratorDataSet>();
				GeneratorContainer.instance.generatorSet.name = "Generator Set";
				CreateAssetAtGeneratorResources(GeneratorContainer.instance.generatorSet);
			}
			//checking for multiple generator data sets
			else if (guids.Length > 1)
			{
				Debug.LogWarning("There should not be multiple generator data sets! Please delete the multiple instances of generator data set scriptable object assets from Assets/HexagonGenerator/Assets/Resources folder.");
				GeneratorContainer.instance.generatorSet = AssetDatabase.LoadAssetAtPath<GeneratorDataSet>(AssetDatabase.GUIDToAssetPath(guids[0]));
			}
			else
			{
				GeneratorContainer.instance.generatorSet = AssetDatabase.LoadAssetAtPath<GeneratorDataSet>(AssetDatabase.GUIDToAssetPath(guids[0]));
			}
		}

		public static bool TryGetGeneratorResourcesFolder(out string folder)
		{
			string path = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:Script GeneratorResources")[0]);
			folder = path.Substring(0, path.LastIndexOf('/'));
			//Debug.Log("Checking folder: " + folder);

			if (!AssetDatabase.IsValidFolder(folder))
			{
				Debug.LogError(folder + " - folder is not exists!");
				return false;
			}

			return true;
		}


		public static bool TryGetResourcesFolder(out string folder)
		{
			string path = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:Script ResourceManager")[0]);
			folder = path.Substring(0, path.LastIndexOf('/'));
			//folder = folder.Substring(0, folder.LastIndexOf('/'));
			//Debug.Log("Checking folder: " + folder);

			if (!AssetDatabase.IsValidFolder(folder))
			{
				Debug.LogError(folder + " - folder is not exists!");
				return false;
			}

			return true;
		}

		public static bool TryGetTexturesFolder(out string folder)
		{
			folder = "";

			if (!TryGetResourcesFolder(out string path))
				return false;

			folder = path + "/Textures";

			//Debug.Log("Checking folder: " + folder);

			if (!AssetDatabase.IsValidFolder(folder))
			{
				Debug.LogError(folder + " - folder is not exists!");
				return false;
			}

			return true;
		}

		public static bool TryGetDebugFolder(out string folder)
		{
			folder = "";

			if (!TryGetGeneratorResourcesFolder(out string path))
				return false;

			folder = path + "/Debug";

			//Debug.Log("Checking folder: " + folder);

			if (!AssetDatabase.IsValidFolder(folder))
			{
				Debug.LogError(folder + " - folder is not exists!");
				return false;
			}

			return true;
		}

		public static bool TryGetTexturesFolder(out string folder, string subFolder)
		{
			folder = "";

			if (!TryGetResourcesFolder(out string path))
				return false;

			folder = path + "/Textures/" + subFolder;

			if (!AssetDatabase.IsValidFolder(folder))
			{
				Debug.LogError(folder + " - folder is not exists!");
				return false;
			}

			return true;
		}

		public static Texture2D LoadTexture(string folder, string name)
		{
			return EditorGUIUtility.Load(folder + "/" + name + ".png") as Texture2D;
		}


		/// <summary>
		/// Creates an asset file for <param name="obj"></param> at main package folder with default extension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="extension"></param>
		public static void CreateAssetAtResources<T>(T obj) where T : Object
		{
			if (TryGetResourcesFolder(out string folder))
			{
				AssetDatabase.CreateAsset(obj, folder + "/" + obj.name + ".asset");
				AssetDatabase.SaveAssets();
			}
		}

		/// <summary>
		/// Creates an asset file for <param name="obj"></param> at main package folder with specified extension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="extension"></param>
		public static void CreateAssetAtResources<T>(T obj, string extension) where T : Object
		{
			if (TryGetResourcesFolder(out string folder))
			{
				AssetDatabase.CreateAsset(obj, folder + "/" + obj.name + extension);
				AssetDatabase.SaveAssets();
			}
		}

		/// <summary>
		/// Creates an asset file for <param name="obj"></param> at specified path.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="path"></param>
		/// <param name="extension"></param>
		public static void CreateAssetAtResources<T>(T obj, string path, string extension) where T : Object
		{
			if (TryGetResourcesFolder(out string folder))
			{
				AssetDatabase.CreateAsset(obj, folder + path + extension);
				AssetDatabase.SaveAssets();
			}

		}



		/// <summary>
		/// Creates an asset file for <param name="obj"></param> at main package folder with default extension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="extension"></param>
		public static void CreateAssetAtGeneratorResources<T>(T obj) where T : Object
		{
			if (TryGetGeneratorResourcesFolder(out string folder))
			{
				AssetDatabase.CreateAsset(obj, folder + "/" + obj.name + ".asset");
				AssetDatabase.SaveAssets();
			}
		}

		/// <summary>
		/// Creates an asset file for <param name="obj"></param> at main package folder with specified extension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="extension"></param>
		public static void CreateAssetAtGeneratorResources<T>(T obj, string extension) where T : Object
		{
			if (TryGetGeneratorResourcesFolder(out string folder))
			{
				AssetDatabase.CreateAsset(obj, folder + "/" + obj.name + extension);
				AssetDatabase.SaveAssets();
			}
		}

		/// <summary>
		/// Creates an asset file for <param name="obj"></param> at specified path.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="path"></param>
		/// <param name="extension"></param>
		public static void CreateAssetAtGeneratorResources<T>(T obj, string path, string extension) where T : Object
		{
			if (TryGetGeneratorResourcesFolder(out string folder))
			{
				AssetDatabase.CreateAsset(obj, folder + path + extension);
				AssetDatabase.SaveAssets();
			}

		}

		public static bool TryGetGeneratorsFolder(out string folder)
		{
			folder = "";

			if (!TryGetGeneratorResourcesFolder(out string path))
				return false;

			folder = path + "/Generators";

			if (!AssetDatabase.IsValidFolder(folder))
			{
				Debug.LogError(folder + " - folder is not exists!");
				return false;
			}

			return true;
		}

		public static GeneratorData CreateGenerator(int currentArraySize, GeneratorSettings generatorSettings)
		{
			if (!TryGetGeneratorsFolder(out string folder))
				return null;

			if (generatorSettings == null)
			{
				Debug.LogError("Generator Settings is null. You probably need to assign the " + nameof(generatorSettings) + " variable of the GeneratorContainer scriptable object in the inspector.");
			}

			string generatorName = "Generator_" + currentArraySize;
			string generatorNameSlashed = "/" + generatorName;
			string generatorFolder = folder + generatorNameSlashed;
			string generatorPath = generatorFolder + ".asset";

			GeneratorData generatorData = AssetDatabase.LoadAssetAtPath<GeneratorData>(generatorPath);

			if (generatorData)
				FindUniquePath();

			generatorData = CreateAndSaveInstances();

			return generatorData;

			void FindUniquePath()
			{
				int j = currentArraySize;

				while (generatorData)
				{
					j++;
					generatorName = "Generator_" + j;
					generatorNameSlashed = "/" + generatorName;
					generatorFolder = folder + generatorNameSlashed;
					generatorPath = generatorFolder + ".asset";
					generatorData = AssetDatabase.LoadAssetAtPath<GeneratorData>(generatorPath);
				}
			}

			GeneratorData CreateAndSaveInstances()
			{
				generatorData = ScriptableObject.CreateInstance<GeneratorData>();
				AssetDatabase.CreateAsset(generatorData, generatorPath);

				generatorData.Initialize(generatorSettings, generatorName, generatorFolder);

				AssetDatabase.CreateFolder(folder, generatorName);

				AssetDatabase.CreateAsset(generatorData.graphicsSettings.terrainMaterial, generatorFolder + generatorNameSlashed + "_terrainMat.mat");
				AssetDatabase.CreateAsset(generatorData.graphicsSettings.waterMaterial, generatorFolder + generatorNameSlashed + "_waterMat.mat");
				//TODO: Consider changing texture asset to png to fix scene texture lost
				AssetDatabase.CreateAsset(generatorData.graphicsSettings.texture2D, generatorFolder + generatorNameSlashed + "_texture.asset");
				AssetDatabase.CreateAsset(generatorData.topografiaSettings, generatorFolder + generatorNameSlashed + "_topoSettings.asset");
				AssetDatabase.CreateAsset(generatorData.noiseSettings, generatorFolder + generatorNameSlashed + "_noiseSettings.asset");
				AssetDatabase.CreateAsset(generatorData.graphicsSettings, generatorFolder + generatorNameSlashed + "_graphicsSettings.asset");

				AssetDatabase.SaveAssets();

				AssetDatabase.Refresh();
				EventManager.OnDatabaseRefresh();

				return generatorData;
			}
		}

		public static void RemoveGenerator(GeneratorData generatorData)
		{
			if (TryGetGeneratorResourcesFolder(out string folder))
				AssetDatabase.DeleteAsset(generatorData.generatorFolderPath);
		}


	}
}
