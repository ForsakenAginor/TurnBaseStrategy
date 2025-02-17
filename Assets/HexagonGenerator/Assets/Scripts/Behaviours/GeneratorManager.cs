using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polygen.HexagonGenerator
{
    public class GeneratorManager : MonoBehaviour
    {
        public GeneratorDataSet generatorDataSet;
        GameObject lastCreatedObj;
        public bool randomSeed;
        int index;

        private void Awake()
        {
            //To clear the scene from other worlds.
            foreach (var generatorObj in FindObjectsOfType<GeneratorObject>())
            {
                DestroyerOfWorlds(generatorObj.gameObject);
            }
        }

        public void SetIndex(int index)
        {
            this.index = index;
        }

        public void Generate()
        {
            GeneratorData generatorData = generatorDataSet.generators[index];

            if (lastCreatedObj)
            {
                DestroyerOfWorlds(lastCreatedObj);
            }

            //Cloning original generatordata
            GeneratorData cloneData = Instantiate(generatorData);

            if (randomSeed)
                cloneData.SetRandomSeed();

            //Generating object within cloned generator data
            lastCreatedObj = cloneData.CreateGeneratorGameobject();
        }

        private void DestroyerOfWorlds(GameObject obj)
        {
#if UNITY_EDITOR
            DestroyImmediate(obj);
#else
            Destroy(obj);
#endif
        }
    }
}