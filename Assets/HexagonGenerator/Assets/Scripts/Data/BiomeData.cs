using UnityEngine;

namespace Polygen.HexagonGenerator
{
    /// <summary>
    /// Scriptable object based class to manage individual biome settings.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(BiomeData), menuName = "POLYGEN/" + nameof(BiomeData))]
    public class BiomeData : ScriptableObject
    {
        public string biomeName;
        public Color biomeBaseColor;

        [Header("Detail Settings")]
        [Range(0, 1), Tooltip("Base generation density of the detail object(s) of selected biome. -affects all detail objects")]
        public float detailDensity = .5f;
        [Range(0, 2), Tooltip("Base scale of the detail object(s) of selected biome. -affects all detail objects")]
        public float biomeDetailScale = 1;
        [Range(0, 200), Tooltip("Base generation noise of the detail object(s) of selected biome. -affects all detail objects")]
        public float biomeDetailNoiseScale = 1;
        [Tooltip("The array of added detail objects of selected biome.")]
        public DetailData[] details;

        public void Initialize()
        {
            biomeName = name;
            biomeBaseColor.a = 1;
        }

		public void Initialize(Color color)
        {
            biomeBaseColor = color;
            biomeName = name;
        }

		[System.Serializable]
        public class DetailData
        {
            //TODO: Add randomize rotation angle limits.
            [Tooltip("Randomize detail object(s) rotations btw 0-360 on y axis.")]
            public bool randomizeRotation = false;
            [Range(0,1), Tooltip("Generation density of the detail object(s).")]
            public float density = .5f;
            [Range(0, 2), Tooltip("Scale of the detail object(s).")]
            public float detailScale = 1;
            [Range(0,2f), Tooltip("Value of the dispersion of detail object(s) from tile center.")]
            public float variantOffset = .3f; 
            [Range(-2f,2f), Tooltip("Value of the vertical offset of detail object(s).")]
            public float verticalOffset = 0;
            [Range(0, 10), Tooltip("Maximum amount of object on a single tile.")]
            public int maxVariantCount = 3;
            //TODO: Prefab preview property
            //[PrefabPreview]
            [Tooltip("The prefab to use as detail object.")]
            public GameObject detailPrefab;

            public DetailData()
            {
                randomizeRotation = false;
                density = .5f;
                detailScale = 1;
                variantOffset = .3f;
                maxVariantCount = 3;
            }
        }
    } 
}




