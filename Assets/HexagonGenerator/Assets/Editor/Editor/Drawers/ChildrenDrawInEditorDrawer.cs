using UnityEditor;
using Polygen.HexagonGenerator;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace PolygenEditor
{
	[CustomPropertyDrawer(typeof(ChildrenDrawInEditorAttribute))]
	public class ChildrenDrawInEditorDrawer : PropertyDrawer
	{

		const int buttonWidth = 66;

		static readonly List<string> ignoreClassFullNames = new List<string> { "TMPro.TMP_FontAsset" };

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			//Debug.Log(label.text);
			EditorGUI.BeginProperty(position, label, property);
			var type = GetFieldType();

			if (type == null || ignoreClassFullNames.Contains(type.FullName))
			{
				EditorGUI.PropertyField(position, property, label);
				EditorGUI.EndProperty();
				return;
			}

			var buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, EditorGUIUtility.singleLineHeight);

			if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
			{
				var data = (ScriptableObject)property.objectReferenceValue;

				//if (property.isExpanded)
				//{
				// Draw a background that shows us clearly which fields are part of the ScriptableObject

				EditorGUI.indentLevel++;
				SerializedObject serializedObject = new SerializedObject(data);

				// Iterate over all the values and draw them
				SerializedProperty prop = serializedObject.GetIterator();
				float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				if (prop.NextVisible(true))
				{
					do
					{
						// Don't bother drawing the class file
						if (prop.name == "m_Script") continue;

						switch (prop.propertyType)
						{
							case SerializedPropertyType.ObjectReference:
								EditorGUI.ObjectField(new Rect(position.x, y, position.width - buttonWidth, EditorGUIUtility.singleLineHeight), prop);
								break;
							default:
								EditorGUI.PropertyField(new Rect(position.x, y, position.width - buttonWidth, EditorGUIUtility.singleLineHeight), prop, true);
								break;
						}
						y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
					}
					while (prop.NextVisible(false));
				}
				if (GUI.changed)
					serializedObject.ApplyModifiedProperties();

				EditorGUI.indentLevel--;
				//}
			}
			else
			{
				if (GUI.Button(buttonRect, "Create"))
				{
					string selectedAssetPath = "Assets";
					if (property.serializedObject.targetObject is MonoBehaviour)
					{
						MonoScript ms = MonoScript.FromMonoBehaviour((MonoBehaviour)property.serializedObject.targetObject);
						selectedAssetPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(ms));
					}

					property.objectReferenceValue = CreateAssetWithSavePrompt(type, selectedAssetPath);
				}
			}
			property.serializedObject.ApplyModifiedProperties();
			EditorGUI.EndProperty();
		}

		// Creates a new ScriptableObject via the default Save File panel
		static ScriptableObject CreateAssetWithSavePrompt(Type type, string path)
		{
			path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", type.Name + ".asset", "asset", "Enter a file name for the ScriptableObject.", path);
			if (path == "") return null;
			ScriptableObject asset = ScriptableObject.CreateInstance(type);
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			EditorGUIUtility.PingObject(asset);
			return asset;
		}

		Type GetFieldType()
		{
			Type type = fieldInfo.FieldType;
			if (type.IsArray) type = type.GetElementType();
			else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) type = type.GetGenericArguments()[0];
			return type;
		}

	}
}
