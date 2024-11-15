using Assets.Scripts.HexGrid;
using Assets.Source.Scripts.GameLoop.StateMachine;
using System;
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
    [SerializeField] private Mover _firstWarriot;
    [SerializeField] private Mover _secondWarriot;

    private void Start()
    {
        _gridCreator.Init();
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);
        var unitsGrid = _gridCreator.UnitsGrid;
        InputSorter inputSorter = new InputSorter(unitsGrid, _cellSelector, _gridCreator.PathFinder);
        _cellHighlighter = new (inputSorter, _gridCreator.HexGrid);
        UnitManager unitManager = new UnitManager(inputSorter, _gridCreator.UnitsGrid);
        unitManager.AddUnit(new(6), _firstWarriot);
        unitManager.AddUnit(new(6), _secondWarriot);

        var stateMachine = _gameStateMachineCreator.Create(unitManager.Units, new List<IControllable>() { inputSorter });

        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();
    }
}