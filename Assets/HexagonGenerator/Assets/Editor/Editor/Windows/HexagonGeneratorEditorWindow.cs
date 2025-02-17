using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using Polygen.Extensions;
using Polygen.HexagonGenerator;
using System.Reflection;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using PolygenEditor.Extensions;

namespace PolygenEditor
{
	public class HexagonGeneratorEditorWindow : EditorWindow
	{
		int toolbarIndex = 0;
		string[] toolbarTitles = new string[] { "General", "Topografia", "Noise", "Graphics", "Biomes" };

		GeneratorContainer targetObject;
		SerializedObject generatorContainerSerializedObject;
		SerializedProperty generatorsProperty;

		string selectedPropertyPath;
		SerializedProperty selectedGenerator;
		int selectedIndex = -1;

		static Texture2D homeTexture;
		static Texture2D generateTexture;
		static Texture2D createTexture;
		static Texture2D hBackgroundTexture;
		static Texture2D vBackgroundTexture;
		//static Texture2D helpTexture;
		static Texture2D binTexture;
		static Texture2D refreshTexture;
		static Texture2D duplicateTexture;
		static Texture2D homePanelTexture;

		//TODO: Add tutorial textures
		//static Texture2D tutorialText01;
		//static Texture2D tutorialText02;
		//static Texture2D tutorialText03;
		//static Texture2D tutorialText04;

		const float space = 10f;
		const int buttonSize = 32;
		const int iconButtonSize = 32;
		const int textSize = 32;
		const int borderSize = 10;
		const int gradientWidth = 25;
		const float keyWidth = 20;
		const float keyHeight = 10;

        private const string themeColor = "#FAFFFA";
        private const string selectionColor = "#D9E6C7";

        Rect gradientPreviewRect;
		Rect[] keyRects;
		bool mouseIsDownOverKey;
		int selectedKeyIndex;
		bool needsRepaint;
		string helpText = "Help me!";
		string helpPrefix;
		bool helpPanel = true;
		Vector2 scrollPos;
		bool graphicsSettingsFoldout = true;
		bool gradientAutoGenerate = true;

		GraphicsSettings graphicsSettings;
		ExtendedGradient gradient;


		[MenuItem("Window/Hexagon Generator")]
		public static void Open()
		{
			//Debug.Log("editor window open.");

			if (EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
			{
				//Debug.Log("Play Mode started!");
				return;
			}

			InitializeWindow();

			EventManager.OnEditorWindowOpen();
		}

		private static void InitializeWindow()
		{
			HexagonGeneratorEditorWindow window = GetWindow<HexagonGeneratorEditorWindow>("Hexagon Generator");


			ResourceManager.Initialize();


			if (GeneratorContainer.instance == null)
			{
				ResourceManager.SetGeneratorContainerInstance();
			}

			GeneratorContainer generatorContainer = GeneratorContainer.instance;

			window.generatorContainerSerializedObject = new SerializedObject(generatorContainer);
			window.targetObject = window.generatorContainerSerializedObject.targetObject as GeneratorContainer;

			window.minSize = new Vector2(350, 350);
			//window.maxSize = new Vector2(1920, 1080);

			window.autoRepaintOnSceneChange = true;

			if (ResourceManager.TryGetTexturesFolder(out string folder, "UI"))
			{
				homeTexture = ResourceManager.LoadTexture(folder,"home");
				generateTexture = ResourceManager.LoadTexture(folder, "generate");
				createTexture = ResourceManager.LoadTexture(folder, "create");
				hBackgroundTexture = ResourceManager.LoadTexture(folder, "hexBack_h");
				vBackgroundTexture = ResourceManager.LoadTexture(folder, "hexBack_v");
				//helpTexture = ResourceManager.LoadTexture(folder, "help");
				binTexture = ResourceManager.LoadTexture(folder, "bin");
				refreshTexture = ResourceManager.LoadTexture(folder, "refresh");
				duplicateTexture = ResourceManager.LoadTexture(folder, "duplicate");
				homePanelTexture = ResourceManager.LoadTexture(folder, "homePanel");
				//tutorialText01 = ResourceManager.LoadTexture(folder, "tutorial01");
				//tutorialText02 = ResourceManager.LoadTexture(folder, "tutorial02");
				//tutorialText03 = ResourceManager.LoadTexture(folder, "tutorial03");
				//tutorialText04 = ResourceManager.LoadTexture(folder, "tutorial04");
			}

		}

		public void OnDestroy()
		{
			//Debug.Log("Hexagon Generator Editor Windows has been closed.");
		}

		void OnGUI()
		{
			//Beginning of OnGUI

			if (generatorContainerSerializedObject == null)
			{
				EditorGUILayout.Space(10);
				if (GUILayout.Button("Open Window", GUILayout.Height(30)))
					InitializeWindow();
				return;
			}

			Color backgroundColor = GUI.backgroundColor;

			InitializeGUI();

			EditorGUILayout.BeginVertical();

			LoadMainPanel();
			
			LoadHelpPanel();

			EditorGUILayout.EndVertical();

			generatorContainerSerializedObject.Update();
			generatorContainerSerializedObject.ApplyModifiedProperties();

			//End of OnGUI

			void InitializeGUI()
			{
				
				//TODO: Fix background image streching
				EditorGUILayout.BeginVertical(new GUIStyle { normal = new GUIStyleState { background = hBackgroundTexture }, alignment = TextAnchor.MiddleRight }, GUILayout.MinHeight(60), GUILayout.ExpandWidth(true));
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10);
				GUILayout.Label(" Hexagon Generator", new GUIStyle { fontSize = textSize, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft }, GUILayout.ExpandHeight(true));
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(new GUIContent { image = refreshTexture, tooltip = "Reload editor window manually." }, GUILayout.Width(iconButtonSize), GUILayout.Height(iconButtonSize)))
				{
					AssetDatabase.Refresh();
					AssetDatabase.SaveAssets();
					Open();
				}
				//TODO:Add help button
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				
			}

			void LoadMainPanel()
			{
				generatorsProperty = new SerializedObject(generatorContainerSerializedObject.FindProperty("generatorSet").objectReferenceValue).FindProperty("generators");

				EditorGUILayout.BeginHorizontal();

				//LeftMenu
				DrawLeftMenu();

				//MainPanel or HomePanel
				if (IsSelectionValid())
				{
					DrawMainPanel();
				}
				else
					DrawHomePanel();

				//RightMenu
				if (IsSelectionValid())
					DrawRightMenu();

				EditorGUILayout.EndHorizontal();

				#region DrawMenus
				void DrawLeftMenu()
				{
					EditorGUILayout.BeginVertical(new GUIStyle { normal = new GUIStyleState { background = vBackgroundTexture } }, GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
					GUILayout.Space(space);
					int i = 0;
					
					//Debug.Log("i: " + i);
					foreach (SerializedProperty p in generatorsProperty)
					{
						EditorGUILayout.BeginHorizontal();
						if (selectedIndex == i)
							EditorExtensions.BeginBackgroundColor(selectionColor);

						if (p.objectReferenceValue != null && GUILayout.Button(new GUIContent { text = (p.objectReferenceValue as GeneratorData).generatorName, tooltip = "Select Generator" }))
						{
							selectedPropertyPath = p.propertyPath;
							EditorGUIUtility.editingTextField = false;
							selectedIndex = i;
						}

						if (selectedIndex == i)
							EditorExtensions.EndBackgroundColor(backgroundColor);

						if (!string.IsNullOrEmpty(selectedPropertyPath))
						{
							selectedGenerator = generatorsProperty.serializedObject.FindProperty(selectedPropertyPath);
							SelectedGeneratorChanged(targetObject.Get(i));
						}
						//TODO: Generator duplication
					/*	if (GUILayout.Button(new GUIContent { image = duplicateTexture, tooltip = "Duplicate Generator" }, GUILayout.Width(20), GUILayout.Height(20)))
						{
							selectedIndex = i;
							
						}*/

						if (GUILayout.Button(new GUIContent { text = "x", tooltip = "Remove Generator" }, GUILayout.Width(20)))
						{
							selectedIndex = i;
							RemoveGenerator();
						}


						EditorGUILayout.EndHorizontal();
						GUILayout.Space(space);
						i++;
					}

					//GUILayout.FlexibleSpace();

					if (GUILayout.Button("Add Generator"))
					{
						AddGenerator();
					}

					EditorGUILayout.EndVertical();

					void AddGenerator()
					{
						GeneratorData generatorData = ResourceManager.CreateGenerator(generatorsProperty.arraySize, targetObject.generatorSettings);

						if (generatorData != null)
						{
							targetObject.Add(generatorData);
						}

						AssetDatabase.Refresh();
					}
					/*void DuplicateGenerator(GeneratorData generatorData)
					{
						GeneratorData newGeneratorData = ResourceManager.CreateGenerator(generatorsProperty.arraySize, targetObject.generatorSettings);

						if (generatorData != null)
						{
							targetObject.Add(generatorData);
						}

					}*/
					void RemoveGenerator()
					{
						targetObject.Remove(selectedIndex);
						selectedGenerator = null;
						selectedIndex = -1;

						AssetDatabase.Refresh();
					}
				}
				void DrawMainPanel()
				{
					Rect mainPanelRect = EditorGUILayout.BeginVertical("box");

					#region Title and Options
					if (selectedGenerator.objectReferenceValue != null)
					{
                       
                        EditorGUILayout.BeginVertical();
						GUILayout.Space(space / 2);
						EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
						GUILayout.Label((selectedGenerator.objectReferenceValue as GeneratorData).generatorName, new GUIStyle { fontSize = textSize / 2, fontStyle = FontStyle.Bold });
						GUILayout.FlexibleSpace();

						//AutoGenerate Toggle
						if (!targetObject.notAutoGenerate)
							EditorExtensions.BeginBackgroundColor(selectionColor);
						EditorGUI.BeginChangeCheck();
						targetObject.notAutoGenerate = GUILayout.Toggle(targetObject.notAutoGenerate, "Auto Generate", "Button");
						if (EditorGUI.EndChangeCheck())
						{
							needsRepaint = true;
						}
						if (!targetObject.notAutoGenerate)
							EditorExtensions.EndBackgroundColor(backgroundColor);

						//Debug Toggle
						if (!DebugManager.debugNotActivated)
							EditorExtensions.BeginBackgroundColor(selectionColor);

						EditorGUI.BeginChangeCheck();

						DebugManager.debugNotActivated = GUILayout.Toggle(DebugManager.debugNotActivated, "Debug Mode", "Button");

						if (EditorGUI.EndChangeCheck())
						{
							SerializedObject graphicsSettingsObject = new SerializedObject(selectedGenerator.objectReferenceValue);
							SerializedProperty graphicProp = graphicsSettingsObject.FindProperty("graphicsSettings");

							graphicsSettings = graphicProp.objectReferenceValue as GraphicsSettings;

							if (DebugManager.debugNotActivated)
                            {
								graphicsSettings.debugMaterial = null;
                            }
                            else
                            {
                                SetDebugMode();
                            }

                            (selectedGenerator.objectReferenceValue as GeneratorData).ForceNotifyUpdatedValues();

						}
						if (!DebugManager.debugNotActivated)
							EditorExtensions.EndBackgroundColor(backgroundColor);

						EditorGUILayout.EndHorizontal();
						GUILayout.Space(space / 2);

						if(!DebugManager.debugNotActivated)
                        {
							EditorGUI.BeginChangeCheck();
							DebugManager.debugMode = (DebugManager.DebugMode)EditorGUILayout.EnumPopup("Debug Mode", DebugManager.debugMode);
							if(EditorGUI.EndChangeCheck())
							{
								SetDebugMode();

								(selectedGenerator.objectReferenceValue as GeneratorData).ForceNotifyUpdatedValues();
							}
						}

						EditorGUILayout.EndVertical();

						void SetDebugMode()
						{
							switch (DebugManager.debugMode)
							{
								case DebugManager.DebugMode.Default:
									graphicsSettings.debugMaterial = null;
									break;
								case DebugManager.DebugMode.Height:
									graphicsSettings.debugMaterial = targetObject.generatorSettings.debugHeight;
									break;
								default:
									graphicsSettings.debugMaterial = null;
									break;
							}
                        }
                    }
					else
					{
                        Debug.LogError("SelectedProperty is null!");
                        EditorGUILayout.EndVertical(); // Закрываем открытую группу
						return;

                    }

					#endregion

					//toolbar
					DrawToolbar();

					scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

					EditorGUI.BeginChangeCheck();
					//TODO: Add sky settings panel
					switch (toolbarIndex)
					{
                        case 0:
                            DrawGeneratorPanel();
                            SetStatusPanel("General Panel", "Edit general settings as required.");
                            break;
                        case 1:
							DrawTopografiaPanel();
							SetStatusPanel("Topografia Panel", "Edit topografia settings as required.");
							break;
						case 2:
							DrawNoisePanel();
							SetStatusPanel("Noise Panel", "Edit noise settings as required.");
							break;
						case 3:
							DrawGraphicsPanel();
							SetStatusPanel("Graphics Panel", "Edit graphics settings as required." +
								"\nEdit sea biome settings as required");
							break;
						case 4:
							DrawBiomesPanel();
							SetStatusPanel("Biomes Panel", "Click on gradient to add new biome." +
								"\nDrag gradient keys to organize biomes according to height." +
								"\nClick on gradient key to edit, load or create biomes as required." +
								"\nEdit detail settings and prefab preferences under detail settings section as required." +
								"\n");
							break;
						default:
							break;
					}
					if (EditorGUI.EndChangeCheck())
					{
						needsRepaint = true;
					}
					if (needsRepaint)
					{
						if(!targetObject.notAutoGenerate)
							(selectedGenerator.objectReferenceValue as GeneratorData).ForceNotifyUpdatedValues();

						needsRepaint = false;
						Repaint();
					}

					EditorGUILayout.EndScrollView();
					EditorGUILayout.EndVertical();

					#region LocalFuncs

					void DrawToolbar()
					{
						GUILayout.BeginVertical();
						EditorExtensions.DrawUILine(Color.gray);

						EditorExtensions.DrawSelectionBox(new Color(1,1,1,.5f), (int)EditorGUIUtility.singleLineHeight, 100, toolbarIndex, 2);
						toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarTitles, EditorExtensions.toolbarStyle);
						
						EditorExtensions.DrawUILine(Color.gray);
                        GUILayout.EndVertical(); // Изменено!
                    }
					
					void DrawGeneratorPanel()
					{
						if (selectedGenerator != null)
                        {
							EditorGUILayout.BeginVertical();
							
							EditorGUILayout.PropertyField(selectedGenerator, includeChildren: true);

							EditorGUILayout.Space(250);
							(selectedGenerator.objectReferenceValue as GeneratorData).tileLayer = EditorGUILayout.LayerField(new GUIContent(text: "Tile Layer", tooltip: "Select generated tile gameobject's layer"),(selectedGenerator.objectReferenceValue as GeneratorData).tileLayer);
							(selectedGenerator.objectReferenceValue as GeneratorData).detailLayer = EditorGUILayout.LayerField(new GUIContent(text: "Detail Layer", tooltip: "Select generated detail gameobject's layer"), (selectedGenerator.objectReferenceValue as GeneratorData).detailLayer);
							
							EditorGUILayout.EndVertical();
						}
					}

					void DrawTopografiaPanel()
					{
						SerializedObject topoSettingsObject = new SerializedObject(selectedGenerator.objectReferenceValue);
						if (topoSettingsObject.FindProperty("topografiaSettings") != null)
							EditorGUILayout.PropertyField(topoSettingsObject.FindProperty("topografiaSettings"), includeChildren: true);
					}

					void DrawNoisePanel()
					{
						SerializedObject noiseSettingsObject = new SerializedObject(selectedGenerator.objectReferenceValue);
						if (noiseSettingsObject.FindProperty("noiseSettings") != null)
							EditorGUILayout.PropertyField(noiseSettingsObject.FindProperty("noiseSettings"), includeChildren: true);
					}

					void DrawGraphicsPanel()
					{
						SerializedObject graphicsSettingsObject = new SerializedObject(selectedGenerator.objectReferenceValue);
						SerializedProperty graphicProp = graphicsSettingsObject.FindProperty("graphicsSettings");

						if (graphicProp != null)
						{
							graphicsSettingsFoldout = EditorGUILayout.Foldout(graphicsSettingsFoldout, graphicsSettingsFoldout ? "" : "Graphics Settings");
							if (graphicsSettingsFoldout)
							{
								EditorGUILayout.PropertyField(graphicProp, includeChildren: true);
							}

							DrawSeaBiome();

							

							void DrawSeaBiome()
							{
								SerializedProperty biomeProp = new SerializedObject(graphicProp.objectReferenceValue).FindProperty("seaBiomeData");
								BiomeData selectedBiomeData = biomeProp.objectReferenceValue as BiomeData;
								Editor biomeEditor = Editor.CreateEditor(selectedBiomeData);
								if (biomeEditor != null)
								{
									GUILayout.Space(space);
									GUILayout.Label("Sea Biome Settings", EditorStyles.boldLabel);
									//EditorGUILayout.ObjectField(biomeProp);
									biomeEditor.DrawDefaultInspector();
								}
								
							}
						}
					}

					void DrawBiomesPanel()
					{
						SerializedObject graphicsSettingsObject = new SerializedObject(selectedGenerator.objectReferenceValue);
						SerializedProperty graphicProp = graphicsSettingsObject.FindProperty("graphicsSettings");

						graphicsSettings = graphicProp.objectReferenceValue as GraphicsSettings;
						gradient = graphicsSettings.gradient;

						//TODO: Redesign with horizontal layout
						DrawGradient();
						DrawBiomeEditor();
						HandleInput();

						void DrawGradient()
						{
							//TODO: Fix to get correct value of main panel rect size 
							gradientPreviewRect = new Rect(mainPanelRect.width - borderSize - gradientWidth, borderSize, gradientWidth, mainPanelRect.height - borderSize - 120);

							GUI.DrawTexture(gradientPreviewRect, gradient.GetTexture((int)Mathf.Abs(gradientPreviewRect.height), false));

							keyRects = new Rect[gradient.NumKeys];
							for (int i = 0; i < gradient.NumKeys; i++)
							{
								ExtendedGradient.BiomeKey key = gradient.GetKey(i);
								Rect keyRect = new Rect(gradientPreviewRect.x - keyWidth, borderSize + gradientPreviewRect.height - gradientPreviewRect.height * key.Time - keyHeight / 2f, keyWidth, keyHeight);

								if (i == selectedKeyIndex)
								{
									EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.red);
								}
								else
								{
									EditorGUI.DrawRect(new Rect(keyRect.x - 1, keyRect.y - 1, keyRect.width + 2, keyRect.height + 2), Color.white);
								}
								EditorGUI.DrawRect(keyRect, key.Color);

								keyRects[i] = keyRect;

								//Show biome names.
								if (key.biomeData == null)
									continue;
								EditorGUI.LabelField(new Rect(keyRect.x - keyRect.width - 5, keyRect.y, keyRect.width, keyRect.height), key.biomeData.biomeName, new GUIStyle { alignment = TextAnchor.MiddleRight });
							}
						}
						void DrawBiomeEditor()
						{
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.BeginVertical();
							EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

							Color color = EditorExtensions.BeginBackgroundColor(themeColor);
							EditorGUILayout.LabelField("Gradient Settings", EditorStyles.boldLabel);

							//Are you sure dialog.
							if (GUILayout.Button("Reset Gradient"))
							{
								if (EditorUtility.DisplayDialog("",	"Are you sure you want to reset gradient and clear all biome keys?", "Yes", "No"))
									gradient.Reset();
							}

							gradient.blendMode = (ExtendedGradient.BlendMode)EditorGUILayout.EnumPopup("Blend mode", gradient.blendMode);
							EditorGUILayout.BeginHorizontal();
							gradient.biomeBlendWeight = EditorGUILayout.Slider("Biome Blend Weight", gradient.biomeBlendWeight, .1f, 10f);
							if (GUILayout.Button("Reset", GUILayout.Width(50)))
								gradient.biomeBlendWeight = 1;
							EditorGUILayout.EndHorizontal();
							gradient.randomizeColor = EditorGUILayout.Toggle("Rand New Key Color", gradient.randomizeColor);
							gradientAutoGenerate = EditorGUILayout.Toggle("Gradient Auto Generate", gradientAutoGenerate);

                            EditorExtensions.EndBackgroundColor(color);

							EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
							EditorExtensions.BeginBackgroundColor(themeColor);

							SerializedProperty keysProp = new SerializedObject(graphicProp.objectReferenceValue).FindProperty("gradient.keys");
							if (keysProp.arraySize > selectedKeyIndex)
							{

								SerializedProperty selectedBiomeProp = keysProp.GetArrayElementAtIndex(selectedKeyIndex).FindPropertyRelative("biomeData");
								BiomeData selectedBiomeData = selectedBiomeProp.objectReferenceValue as BiomeData;
								Editor biomeEditor = Editor.CreateEditor(selectedBiomeData);

								if (biomeEditor != null)
								{
									color = EditorExtensions.BeginBackgroundColor(EditorExtensions.FadeColor(selectedBiomeData.biomeBaseColor, .5f, .5f));
									//EditorExtensions.DrawUILine(Color.grey);
									EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
									EditorGUILayout.LabelField("Selected Biome", EditorExtensions.titleStyle, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));
									EditorGUILayout.LabelField(selectedBiomeData.biomeName, EditorExtensions.titleStyle);

									if (GUILayout.Button(new GUIContent { image = duplicateTexture, tooltip = "Duplicate selected biome." }, GUILayout.Width(24), GUILayout.Height(28)))
									{
										DuplicateKey();
									}
									if (GUILayout.Button(new GUIContent { image = binTexture, tooltip = "Delete selected biome." }, GUILayout.Width(24), GUILayout.Height(28)))
									{
										DeleteKey();
									}


									EditorGUILayout.EndHorizontal();
									//EditorExtensions.DrawUILine(Color.grey);
									EditorExtensions.EndBackgroundColor(color);

									EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

									EditorGUILayout.ObjectField(selectedBiomeProp);
									biomeEditor.DrawDefaultInspector();

									//GUILayout.Label(EditorExtensions.GetPrefabPreview(selectedBiomeData.));
								}
								else
								{
									EditorGUILayout.LabelField("Key Settings", EditorStyles.boldLabel);

									int currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive) + 100;

									gradient.SetKeyColor(selectedKeyIndex, EditorGUILayout.ColorField(gradient.GetKey(selectedKeyIndex).Color));

									if (GUILayout.Button("Load Biome"))
									{
										EditorGUIUtility.ShowObjectPicker<BiomeData>(null, false, "", currentPickerWindow);
									}

									if (GUILayout.Button("Create New Biome"))
									{
										BiomeData newBiomeData = EditorExtensions.CreateAssetWithSavePrompt(typeof(BiomeData), "Assets") as BiomeData;

										if (newBiomeData != null)
										{
											if (gradient.randomizeColor)
												newBiomeData.Initialize(gradient.GetKey(selectedKeyIndex).Color);
											else
												newBiomeData.Initialize();
											gradient.SetKeyData(selectedKeyIndex, newBiomeData);
										}
									}

									if (GUILayout.Button("Delete Key"))
									{
										DeleteKey();
									}

									if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
									{
										gradient.SetKeyData(selectedKeyIndex, EditorGUIUtility.GetObjectPickerObject() as BiomeData);

										currentPickerWindow = -1;
									}
								}
							}

							EditorExtensions.EndBackgroundColor(color);
							EditorGUILayout.EndVertical();
							EditorGUILayout.Space(gradientPreviewRect.width * 5);
							EditorGUILayout.EndHorizontal();
						}
						void HandleInput()
						{
							Event guiEvent = Event.current;
							if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
							{
								for (int i = 0; i < keyRects.Length; i++)
								{
									if (keyRects[i].Contains(guiEvent.mousePosition))
									{
										mouseIsDownOverKey = true;
										selectedKeyIndex = i;
										//needsRepaint = true;
										break;
									}
								}

								if (!mouseIsDownOverKey && gradientPreviewRect.Contains(guiEvent.mousePosition))
								{
									//Debug.Log(guiEvent.mousePosition);
									float keyTime = Mathf.InverseLerp(gradientPreviewRect.yMax, gradientPreviewRect.y, guiEvent.mousePosition.y);
									Color interpolatedColor = gradient.Evaluate(keyTime);
									Color randomColor = new Color(Random.value, Random.value, Random.value);

									selectedKeyIndex = gradient.AddKey((gradient.randomizeColor) ? randomColor : interpolatedColor, keyTime);

									mouseIsDownOverKey = true;
									needsRepaint = true;
								}
							}
							else if (gradientPreviewRect.Contains(guiEvent.mousePosition) && helpPanel)
							{
								GUI.Label(new Rect(guiEvent.mousePosition.x - 105, guiEvent.mousePosition.y, 100, 30), "Click on gradient \nto add new biome", EditorStyles.helpBox);
							}

							if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
							{
								mouseIsDownOverKey = false;
								needsRepaint = true;
							}

							if (mouseIsDownOverKey && guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
							{
								float keyTime = Mathf.InverseLerp(gradientPreviewRect.yMax, gradientPreviewRect.y, guiEvent.mousePosition.y);
								int currentKey = selectedKeyIndex;
								selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, keyTime);
								if (selectedKeyIndex != currentKey)
								{
									Debug.Log("Key Changed");
								}
								needsRepaint = gradientAutoGenerate;
							}

							if (guiEvent.keyCode == KeyCode.Delete && guiEvent.type == EventType.KeyDown)
							{
								DeleteKey();
							}
						}

						void DeleteKey()
						{
							gradient.RemoveKey(selectedKeyIndex);
							if (selectedKeyIndex >= gradient.NumKeys)
							{
								selectedKeyIndex--;
							}
							needsRepaint = true;
						}
						void DuplicateKey()
						{
							ExtendedGradient.BiomeKey biomeKey = gradient.GetKey(selectedKeyIndex);
							selectedKeyIndex = gradient.AddKey(biomeKey.biomeData, biomeKey.Time);

							needsRepaint = true;
						}
					}
                    #endregion
                }
				void DrawHomePanel()
				{

					SetStatusPanel("Help Panel", "The information about selected features will be shown here.");

					EditorGUILayout.BeginVertical();
					//Debug.Log(rect);
					GUILayout.Space(space);
					GUILayout.Label("   <- Select or create a generator from side menu to start!", EditorExtensions.h4Style);

					GUILayout.Space(space*2);

					GUILayout.Label("Welcome to Hexagon Generator!");
					GUILayout.Label("A hexagon worlds generator for unity3d.");
					GUILayout.Label("With simple pluggable system based on scriptable objects, you can create various worlds.", EditorStyles.wordWrappedLabel, GUILayout.MaxWidth(Screen.width - 150));

					GUILayout.Space(space);
					EditorExtensions.DrawUILine();
					GUILayout.Label(" • Creating a new generator will come with new default settings, you can modify them as required.", EditorStyles.wordWrappedLabel, GUILayout.MaxWidth(Screen.width-150));
					//GUILayout.Label(tutorialText01);

					GUILayout.Space(space);
					//EditorExtensions.DrawUILine();
					GUILayout.Label(" • You can also use your own created settings -topo, noise, graphics- for your generators and share settings between them with pluggable system.", EditorStyles.wordWrappedLabel, GUILayout.MaxWidth(Screen.width - 150));
					//GUILayout.Label(tutorialText02);

					GUILayout.Space(space);
					//EditorExtensions.DrawUILine();
					GUILayout.Label(" • The working logic of the pluggable system of the generator is based on using settings as templates and sharing them between generator.", EditorStyles.wordWrappedLabel, GUILayout.MaxWidth(Screen.width - 150));
					//GUILayout.Label(tutorialText03);

					GUILayout.Space(space);
					//EditorExtensions.DrawUILine();
					GUILayout.Label(" • Also you can simply use settings individualy for your generators.", EditorStyles.wordWrappedLabel, GUILayout.MaxWidth(Screen.width - 150));
					//GUILayout.Label(tutorialText04);

					GUILayout.Space(space);
					//EditorExtensions.DrawUILine();
					GUILayout.Label(" • Graphics Settings panel lets you manage biome settings based on terrain height.", EditorStyles.wordWrappedLabel, GUILayout.MaxWidth(Screen.width - 150));
					
					GUILayout.Space(space);
					//EditorExtensions.DrawUILine();
					GUILayout.Label(" • It is not recommended to change the folder structure of the tool, otherwise the tool may give errors and may not work properly.", EditorStyles.wordWrappedLabel, GUILayout.MaxWidth(Screen.width - 150));

					GUILayout.Space(space);
					EditorExtensions.DrawUILine();
					GUILayout.Label("Roadmap", EditorStyles.boldLabel);

					GUILayout.Label(
						" • hexagon tile models variations" +
						"\n • different noise types"+
						"\n • terrain animation system -now not ready for production" +
						"\n • more gameplay-runtime features" +
						"\n • voxel support" + 
						"\n • minor ui fixes" +
						"\n    - prefab preview section for detail objects" +
						"\n    - generator duplication button" +
						"\n • documentation webpage" +
						"\n • optimized workflow with compute shaders",
						
						EditorStyles.wordWrappedLabel, GUILayout.MaxWidth(Screen.width - 150));

					//GUILayout.FlexibleSpace();
					GUILayout.Label(homePanelTexture, GUILayout.MaxHeight(400));
					EditorGUILayout.EndVertical();


				}

				//TODO: Add debug overlay & its button
				void DrawRightMenu()
				{
					EditorGUILayout.BeginVertical(GUILayout.MaxWidth(buttonSize), GUILayout.ExpandHeight(true));
					
					if (GUILayout.Button(new GUIContent(homeTexture, "Load home panel."), GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
					{
						selectedIndex = -1;
					}

					//Generate manually button
					if (GUILayout.Button(new GUIContent(generateTexture, "Generate terrain manually."), GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
					{
						GeneratorData generatorData = targetObject.Get(selectedIndex);

						if (generatorData == null)
							return;

						generatorData.ForceNotifyUpdatedValues();
					}

					//Create gameobject button
					if (GUILayout.Button(new GUIContent(createTexture, "Create Gameobject with selected generators current settings."), GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
					{
						GeneratorData generatorData = targetObject.Get(selectedIndex);

						Debug.Log(generatorData);

						if (generatorData == null)
							return;

						//TODO: Bunu prefaba çevir.
						generatorData.CreateGeneratorGameobject();
					}

					EditorGUILayout.EndVertical();
				}
				#endregion
			}

			//TODO: Add global settings panel.
			/*
			void LoadSettingsPanel()
			{
				generatorsProperty = generatorContainerSerializedObject.FindProperty("generatorSettings");
				EditorGUILayout.PropertyField(generatorsProperty, includeChildren: true);
			}
			*/

			void LoadHelpPanel()
			{
				EditorGUILayout.BeginHorizontal();
				if (helpPanel)
				{
					EditorGUILayout.BeginVertical();
					EditorGUILayout.PrefixLabel(helpPrefix);
					//TODO: Move help button to right menu
					GUILayout.Label(helpText, EditorStyles.largeLabel, GUILayout.MinHeight(20));
					EditorGUILayout.EndVertical();
					if (GUILayout.Button(new GUIContent("Hide", "Hide help panel.")))
						helpPanel = false;
				}
				else
				{
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(new GUIContent("Help me!", "Show help panel."), GUILayout.MinWidth(120)))
						helpPanel = true;
				}

				EditorGUILayout.EndHorizontal();
			}
		}

        private void SetStatusPanel(string prefix = "", string text = "")
        {
			helpPrefix = prefix;
			helpText = text;
		}

        #region Helper Funcs

       
		

		protected void SelectedGeneratorChanged(GeneratorData generatorData)
		{
			//graphicsSettings = generatorData.graphicsSettings;
			//gradient = graphicsSettings.gradient;
		}

		private bool IsSelectionValid()
		{
			return selectedGenerator != null && selectedIndex < generatorsProperty.arraySize && selectedIndex > -1;
		}
		#endregion
	}
}

