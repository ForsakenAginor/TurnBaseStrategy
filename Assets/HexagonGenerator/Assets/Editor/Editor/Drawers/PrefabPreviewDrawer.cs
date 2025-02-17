using UnityEditor;
using Polygen.HexagonGenerator;
using UnityEngine;
using System.Collections.Generic;
using System;
using Polygen.Extensions;
using PolygenEditor.Extensions;

namespace PolygenEditor
{
    [CustomPropertyDrawer(typeof(PrefabPreviewAttribute))]
    public class PrefabPreviewDrawer : PropertyDrawer
    {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{

			EditorGUI.BeginProperty(position, label, property);

			//var propertyRect = Rect.zero;
			//Rect propertyRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

			//var guiContent = new GUIContent(property.displayName);

			//Debug.Log(property.serializedObject.targetObject);
			//Debug.Log(property.serializedObject);
			EditorGUI.ObjectField(position, property, label);
			BiomeData biomeData = property.serializedObject.targetObject as BiomeData;
			foreach (var item in biomeData.details)
			{
				if(item.detailPrefab != null)
					EditorExtensions.TextureField("Prefab", EditorExtensions.GetPrefabPreview(item.detailPrefab));
			}
			//EditorGUI.ObjectField(propertyRect, property, AssetPreview.GetAssetPreview(property.serializedObject.targetObject));


			property.serializedObject.ApplyModifiedProperties();
			EditorGUI.EndProperty();

		}
	}
}
