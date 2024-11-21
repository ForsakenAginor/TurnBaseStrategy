using Assets.Scripts.HexGrid;
using Assets.Source.Scripts.GameLoop.StateMachine;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private HexGridCreator _gridCreator;
    [SerializeField] private MeshUpdater _meshUpdater;
    [SerializeField] private CellHighlighter _cellHighlighter;
    [SerializeField] private GridRaycaster _gridRaycaster;
    [SerializeField] private CellSelector _cellSelector;
    [SerializeField] private GameStateMachineCreator _gameStateMachineCreator;

    [Header("Debug")]
    [SerializeField] private Transform _firstWarrior;
    [SerializeField] private Transform _secondWarrior;
    [SerializeField] private Transform _castle;

    private void Start()
    {
        _gridCreator.Init();
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);
        var unitsGrid = _gridCreator.UnitsGrid;
        InputSorter inputSorter = new InputSorter(unitsGrid, _cellSelector, _gridCreator.PathFinder);
        TestInputSorter test = new TestInputSorter(unitsGrid, _cellSelector, _gridCreator.HexGridView);
        _cellHighlighter = new (inputSorter, test, _gridCreator.HexGrid);

        //********  Unit creation  ***********
        UnitManager unitManager = new UnitManager(inputSorter, test, _gridCreator.UnitsGrid, _gridCreator.PathFinder);
        UnitFactory factory = new();
        unitManager.AddUnit(factory.CreateInfantry(Side.Player), _firstWarrior);
        unitManager.AddUnit(factory.CreateInfantry(Side.Player), _secondWarrior);
        unitManager.AddUnit(factory.CreateCity(Side.Enemy), _castle);
        //************************************

        var stateMachine = _gameStateMachineCreator.Create(unitManager.Units, new List<IControllable>() { inputSorter, test });

        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();
    }
}