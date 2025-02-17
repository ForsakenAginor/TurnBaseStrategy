
using System.Collections.Generic;
using UnityEngine;

namespace Polygen.HexagonGenerator
{
    [CreateAssetMenu(fileName = nameof(GraphicsSettings), menuName = "POLYGEN/Settings/" + nameof(GraphicsSettings))]
    public class GraphicsSettings : ScriptableObject
    {
        public enum Space
        {
            World,
            Local
        }

        public Space space = Space.Local;
        public Texture2D texture2D;
        [Space(5)]
        public Material terrainMaterial;
        public Material waterMaterial;
        [HideInInspector] public Material debugMaterial;


        [Space(5)]
        public int baseDetailSeed;
        [Tooltip("Controls detail object generation method -if true all detail objects will be spawned in same position based on Base Detail Seed, else they will be randomly generated independent from seed value")]
        public bool fixedDetails = false;
        [Range(0, 1), Tooltip("Base generation density of the detail object(s). -affects all biomes")]
        public float baseDetailDensity = .5f;
        [Range(0, 2), Tooltip("Base scale of the detail object(s). -affects all biomes")]
        public float baseDetailScale = 1;
        [Range(0, 200), Tooltip("Base generation noise of the detail object(s). -affects all biomes")]
        public float baseDetailNoiseScale = 50;
        [Tooltip("Base generation noise of the detail object(s). -affects all biomes")]
        public Vector2 detailOffset;
        [HideInInspector]public ExtendedGradient gradient;

        [Header("Water Settings")]
        public Color waterBaseColor = Color.blue;
        public BiomeData seaBiomeData;

        public BiomeData GetBiomeData(float time)
		{
            return gradient.EvaluateKey(time).biomeData;
		}


#if UNITY_EDITOR
        protected void OnValidate()
        {
            terrainMaterial.SetInt("_objectSpace", (int)space);
            waterMaterial.SetColor("_albedoColor", waterBaseColor);
        }

#endif
        /*
        public void RegisterBiomeDatas(System.Action cb)
		{
			for (int i = 0; i < gradient.NumKeys; i++)
			{
                if(gradient.GetKey(i).biomeData !=null)
                    gradient.GetKey(i).biomeData.onNotifyUpdatedValues += cb;
			}
		}

        public void UnregisterBiomeDatas(System.Action cb)
        {
            for (int i = 0; i < gradient.NumKeys; i++)
            {
                if (gradient.GetKey(i).biomeData != null)
                    gradient.GetKey(i).biomeData.onNotifyUpdatedValues -= cb;
            }
        }*/
    }
}