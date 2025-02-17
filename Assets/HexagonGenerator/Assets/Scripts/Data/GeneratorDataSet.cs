using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Polygen.HexagonGenerator
{
    [System.Serializable, CreateAssetMenu(fileName = nameof(GeneratorDataSet), menuName = "POLYGEN/Sets/" + nameof(GeneratorDataSet))]
    public class GeneratorDataSet : ScriptableObject
    {
        [ChildrenDrawInEditor] public List<GeneratorData> generators;

        public GeneratorData GetGeneratorData(string generatorName)
        {
            return generators.Find(generatorData => generatorData.generatorName == generatorName);
        }

    }
}
