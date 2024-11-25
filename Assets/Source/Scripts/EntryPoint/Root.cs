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
    [SerializeField] private CitiesConfiguration _cityConfiguration;

    [Header("Grid")]
    [SerializeField] private HexGridCreator _gridCreator;
    [SerializeField] private MeshUpdater _meshUpdater;
    [SerializeField] private CellHighlighter _cellHighlighter;
    [SerializeField] private GridRaycaster _gridRaycaster;
    [SerializeField] private CellSelector _cellSelector;

    [Header("Game progress")]
    [SerializeField] private GameStateMachineCreator _gameStateMachineCreator;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private CitySpawner _citySpawner;

    [Header("Debug")]
    [SerializeField] private Button _testButton;

    private void Start()
    {
        _gridCreator.Init();
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);
        var unitsGrid = _gridCreator.UnitsGrid;
        NewInputSorter inputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.HexGridView);
        _cellHighlighter = new (inputSorter, _gridCreator.HexGrid);

        //********  Unit creation  ***********
        UnitsActionsManager unitManager = new UnitsActionsManager(inputSorter, unitsGrid);
        _unitSpawner.Init(unitManager, _unitConfiguration, unitsGrid, _gridCreator.HexGridView);
        CitiesActionsManager cityManager = new CitiesActionsManager(inputSorter, unitsGrid);
        _citySpawner.Init(cityManager, _cityConfiguration, unitsGrid);

        _citySpawner.SpawnCity(new Vector2Int(0, 0), CitySize.Village, Side.Player);
        _citySpawner.SpawnCity(new Vector2Int(5, 5), CitySize.Village, Side.Enemy);

        var stateMachine = _gameStateMachineCreator.Create(unitManager.Units, new List<IControllable>() { inputSorter });

        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();

        //********  Debug  ***********
        _testButton.onClick.AddListener(OnTestButtonClick);
    }

    private void OnTestButtonClick()
    {
        _unitSpawner.TrySpawnUnit(new Vector2Int(0, 0), UnitType.Infantry, Side.Player);
    }
}