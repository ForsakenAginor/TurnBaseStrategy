using Assets.Scripts.HexGrid;
using Assets.Source.Scripts.GameLoop.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Gold")]
    [SerializeField] private WalletView _walletView;

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

        //******** Wallet ***********
        Resource wallet = new Resource(0, int.MaxValue);
        TaxSystem taxSystem = new TaxSystem(wallet, _citySpawner, _unitSpawner, _cityConfiguration, _unitConfiguration);
        _walletView.Init(wallet);
        //********  Unit creation  ***********
        UnitsActionsManager unitManager = new UnitsActionsManager(inputSorter, unitsGrid);
        _unitSpawner.Init(unitManager, wallet, _unitConfiguration, unitsGrid, _gridCreator.HexGridView);
        CitiesActionsManager cityManager = new CitiesActionsManager(inputSorter, unitsGrid);
        _citySpawner.Init(cityManager, _unitSpawner, wallet, _cityConfiguration, unitsGrid);

        _citySpawner.SpawnCity(new Vector2Int(0, 0), CitySize.Village, Side.Player);
        _citySpawner.SpawnCity(new Vector2Int(5, 5), CitySize.Village, Side.Enemy);

        //********* EnemyLogic ***************
        EnemyScaner scaner = new(cityManager.GetEnemyCities(), _unitSpawner, unitsGrid);

        //********* Game state machine *******
        var resettables = unitManager.Units.Append(taxSystem);
        var stateMachine = _gameStateMachineCreator.Create(resettables, new List<IControllable>() { inputSorter });

        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();

        //********  Debug  ***********
        _testButton.onClick.AddListener(OnTestButtonClick);
    }

    private void OnTestButtonClick()
    {
        _unitSpawner.TrySpawnUnit(new Vector2Int(0, 0), UnitType.Infantry, Side.Player);
    }
}

public class EnemyScaner
{
    private readonly Dictionary<Vector2Int, Action<Vector2Int>> _enemyCities = new ();
    private readonly UnitSpawner _unitSpawner;
    private readonly HexGridXZ<Unit> _grid;
    private readonly int _squareDetectDistance;

    public EnemyScaner(IEnumerable<(Vector2Int position, CitySize size)> cities, UnitSpawner unitSpawner, HexGridXZ<Unit> grid)
    {
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        
        int detectDistance = 4;
        _squareDetectDistance = (new Vector2Int(0, 0) - new Vector2Int(0, detectDistance)).sqrMagnitude;

        foreach (var city in cities)
            _enemyCities.Add(city.position, SpawnEnemies);

        _grid.GridObjectChanged += OnGridChanged;
    }

    private void OnGridChanged(Vector2Int coordinates)
    {
        var cell = _grid.GetGridObject(coordinates);

        if (cell == null || cell.Side == Side.Enemy)
            return;

        var cities = _enemyCities.Keys.ToArray();

        for( int i = 0; i < cities.Length; i++ )
        {
            bool inDetectRange = (cities[i] - coordinates).sqrMagnitude <= _squareDetectDistance;

            if (inDetectRange && _enemyCities.ContainsKey(cities[i]))
            {
                _enemyCities[cities[i]].Invoke(cities[i]);
                _enemyCities.Remove(cities[i]);
            }
        }
    }

    private void SpawnEnemies(Vector2Int position)
    {
        _unitSpawner.TrySpawnUnit(position, UnitType.Knight, Side.Enemy);
        _unitSpawner.TrySpawnUnit(position, UnitType.Knight, Side.Enemy);
        _unitSpawner.TrySpawnUnit(position, UnitType.Knight, Side.Enemy);
    }
}
