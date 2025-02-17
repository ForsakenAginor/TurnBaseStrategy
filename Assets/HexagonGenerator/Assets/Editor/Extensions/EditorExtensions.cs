using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PolygenEditor.Extensions
{

    public static class EditorExtensions
    {
        
        public static GUIStyle titleStyle = new GUIStyle { fontStyle = FontStyle.Bold, fontSize = 16 };
        public static GUIStyle h1Style = new GUIStyle { fontStyle = FontStyle.Bold, fontSize = 24 };
        public static GUIStyle h2Style = new GUIStyle { fontStyle = FontStyle.Bold, fontSize = 18 };
        public static GUIStyle h3Style = new GUIStyle { fontStyle = FontStyle.Bold, fontSize = 16 };
        public static GUIStyle h4Style = new GUIStyle { fontStyle = FontStyle.Bold, fontSize = 14 };
        public static GUIStyle toolbarStyle = new GUIStyle{fontStyle = FontStyle.Bold, fontSize = 14 };


        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public static void DrawUILine(int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, Color.grey);
        }

        public static void DrawSelectionBox(Color color, int height, int width, int index, int offset = 2)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(0));
            r.height = height + (offset * 2);
            r.y -= offset - 1;
            //r.x -= 2;
            r.x += index * (r.width / 5) - 4;
            r.width = width - 8;
            EditorGUI.DrawRect(r, color);
        }

        public static Color BeginBackgroundColor(Color newColor)
        {
            Color color = GUI.backgroundColor;
            GUI.backgroundColor = newColor;
            GUI.Box(EditorGUILayout.BeginVertical(), "");
            return color;
        }

        public static Color BeginBackgroundColor(string colorHTML)
        {
            Color color = GUI.backgroundColor;
            ColorUtility.TryParseHtmlString(colorHTML, out Color newColor);
           // Debug.Log(newColor);
            GUI.backgroundColor = newColor;
            GUI.Box(EditorGUILayout.BeginVertical(), "");
            return color;
        }
        public static void EndBackgroundColor(Color color)
        {
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = color;
        }

        public static Color FadeColor(Color color, float fadeAmount = .2f, float alphaAmount = 0)
		{
            //return new Color(color.r + (-color.r + .95f) * fadeAmount, color.g + (-color.g + .95f) * fadeAmount, color.b + (-color.b + .95f) * fadeAmount, color.a - alphaAmount);
            return new Color(color.r + fadeAmount, color.g + fadeAmount, color.b + fadeAmount, color.a + fadeAmount);
		}

        public static Texture2D GetPrefabPreview(UnityEngine.Object prefab)
        {
           // Debug.Log("Generate preview for " + path);
            //GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var editor = Editor.CreateEditor(prefab);
          //  Debug.Log(prefab);
            Texture2D tex = editor.RenderStaticPreview(AssetDatabase.GetAssetPath(prefab), null, 200, 200);
            Debug.Log(tex);
            EditorWindow.DestroyImmediate(editor);
            return tex;
        }

        public static Texture2D TextureField(string name, Texture2D texture)
        {
            GUILayout.BeginVertical();
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.UpperCenter;
            style.fixedWidth = 70;
            GUILayout.Label(name, style);
            var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
            GUILayout.EndVertical();
            return result;
        }


        /// <summary>
        /// Gets the object the property represents.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static T GetTargetObjectOfProperty<T>(this SerializedProperty prop) where T : class
        {
            if (prop == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj as T;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        static string lastUsedPath;
        // Creates a new ScriptableObject via the default Save File panel
        public static ScriptableObject CreateAssetWithSavePrompt(Type type, string path)
        {
            string tempPath = path;
            if (lastUsedPath != null)
                path = lastUsedPath;
            path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", type.Name + ".asset", "asset", "Enter a file name for the ScriptableObject.", path);
            if (path == "") return null;
            ScriptableObject asset = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            EditorGUIUtility.PingObject(asset);
            lastUsedPath = tempPath;
            return asset;
        }
    }
}