using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Polygen.HexagonGenerator {
    public static class TextureGenerator
    {
        public const int textureRes = 50;

        public static void UpdateColours(GraphicsSettings graphicsSettings)
        {
            //TODO: add Debug.Error
            if (graphicsSettings == null || graphicsSettings.texture2D == null)
                return;

            Color[] colours = new Color[textureRes];
            for (int i = 0; i < textureRes; i++)
            {
                colours[i] = graphicsSettings.gradient.Evaluate(i / (textureRes - 1f));
            }
            graphicsSettings.texture2D.SetPixels(colours);
            graphicsSettings.texture2D.Apply();

            graphicsSettings.terrainMaterial.SetTexture("_topoTexture", graphicsSettings.texture2D);
        }
    }
}
