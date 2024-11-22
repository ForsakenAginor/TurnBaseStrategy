using Assets.Scripts.HexGrid;
using Assets.Source.Scripts.GameLoop.StateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Root : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private UnitsConfiguration _unitConfiguration;

    [Header("Grid")]
    [SerializeField] private HexGridCreator _gridCreator;
    [SerializeField] private MeshUpdater _meshUpdater;
    [SerializeField] private CellHighlighter _cellHighlighter;
    [SerializeField] private GridRaycaster _gridRaycaster;
    [SerializeField] private CellSelector _cellSelector;

    [Header("Game progress")]
    [SerializeField] private GameStateMachineCreator _gameStateMachineCreator;
    [SerializeField] private UnitSpawner _unitSpawner;

    [Header("Debug")]
    [SerializeField] private Button _testButton;
    [SerializeField] private UnitFacade _firstWarrior;
    [SerializeField] private UnitFacade _secondWarrior;
    [SerializeField] private UnitFacade _castle;

    private void Start()
    {
        _gridCreator.Init();
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);
        var unitsGrid = _gridCreator.UnitsGrid;
        NewInputSorter inputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.HexGridView);
        _cellHighlighter = new (inputSorter, _gridCreator.HexGrid);

        //********  Unit creation  ***********
        UnitsManager unitManager = new UnitsManager(inputSorter, _gridCreator.UnitsGrid, _gridCreator.PathFinder);
        _unitSpawner.Init(unitManager, _unitConfiguration, unitsGrid, _gridCreator.HexGridView);
        //************************************
        _testButton.onClick.AddListener(OnTestButtonClick);
        /*
        UnitFactory factory = new();
        unitManager.AddUnit(factory.CreateInfantry(Side.Player), _firstWarrior);
        unitManager.AddUnit(factory.CreateInfantry(Side.Player), _secondWarrior);
        unitManager.AddUnit(factory.CreateCity(Side.Enemy), _castle);
        */

        var stateMachine = _gameStateMachineCreator.Create(unitManager.Units, new List<IControllable>() { inputSorter });

        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();

    }

    private void OnTestButtonClick()
    {
        _unitSpawner.TrySpawnUnit(new Vector2Int(5, 0), UnitType.Infantry, Side.Player);
    }
}