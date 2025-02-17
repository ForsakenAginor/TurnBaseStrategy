using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;


namespace Polygen.HexagonGenerator {
    public class DropdownManager : MonoBehaviour
    {
        public TMP_Dropdown dropdown;
        public GeneratorDataSet generatorSet;
        // Start is called before the first frame update

        void Start()
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(generatorSet.generators.Select(generatorData => new TMP_Dropdown.OptionData(generatorData.generatorName)).ToList());
        }
    }
}