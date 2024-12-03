using Assets.Scripts.HexGrid;
using Assets.Source.Scripts.GameLoop.StateMachine;
using Lean.Touch;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Root : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private UnitsConfiguration _unitConfiguration;
    [SerializeField] private CitiesConfiguration _cityConfiguration;
    [SerializeField] private EnemySpawnerConfiguration _enemySpawnerConfiguration;
    [SerializeField] private EnemyWaveConfiguration _enemyWaveConfiguration;
    [SerializeField] private GridColorConfiguration _gridColorConfiguration;

    [Header("Grid")]
    [SerializeField] private HexGridCreator _gridCreator;
    [SerializeField] private MeshUpdater _meshUpdater;
    [SerializeField] private CellHighlighter _cellHighlighter;
    [SerializeField] private GridRaycaster _gridRaycaster;
    [SerializeField] private CellSelector _cellSelector;

    [Header("Game progress")]
    [SerializeField] private GameStateMachineCreator _gameStateMachineCreator;
    [SerializeField] private WinLoseMonitor _winLoseMonitor;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private CitySpawner _citySpawner;

    [Header("Gold")]
    [SerializeField] private WalletView _walletView;

    [Header("Enemy")]
    [SerializeField] private EnemyBrain _enemyBrain;

    [Header("Camera")]
    [SerializeField] private Transform _camera;
    [SerializeField] private LeanFingerSwipe _leanSwipe;
    [SerializeField] private PinchDetector _pinchDetector;

    [Header("Debug")]
    [SerializeField] private Button _testButton;

    private void Start()
    {
        _gridCreator.Init();
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);
        var unitsGrid = _gridCreator.UnitsGrid;
        NewInputSorter inputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.BlockedCells);
        _cellHighlighter = new (inputSorter, _gridCreator.HexGrid, _gridColorConfiguration);

        //******** FogOfWar *********
        FogOfWar fogOfWar = new(_gridCreator.Clouds, unitsGrid);

        //******** Wallet ***********
        Resource wallet = new Resource(20, int.MaxValue);
        TaxSystem taxSystem = new TaxSystem(wallet, _citySpawner, _unitSpawner, _cityConfiguration, _unitConfiguration);
        _walletView.Init(wallet);
        //********  Unit creation  ***********
        UnitsActionsManager unitManager = new UnitsActionsManager(inputSorter, unitsGrid, _enemyBrain);
        _unitSpawner.Init(unitManager, wallet, _unitConfiguration, unitsGrid, _gridCreator.BlockedCells);
        CitiesActionsManager cityManager = new CitiesActionsManager(inputSorter, unitsGrid);
        _citySpawner.Init(cityManager, _unitSpawner, wallet, _cityConfiguration, unitsGrid);

        //********* EnemyLogic ***************
        _enemyBrain.Init(unitsGrid, _gridCreator.PathFinder, _unitSpawner, unitManager);
        _citySpawner.SpawnCity(new Vector2Int(6, 5), CitySize.Village, Side.Enemy);
        EnemyWaveSpawner waveSpawner = new(cityManager.GetEnemyCities(), _unitSpawner, _enemyWaveConfiguration);
        EnemyScaner scaner = new(cityManager.GetEnemyCities(), _unitSpawner, unitsGrid, _enemySpawnerConfiguration);

        //********* Game state machine *******
        _winLoseMonitor.Init(cityManager);
        var resettables = unitManager.Units.Append(taxSystem);
        var stateMachine = _gameStateMachineCreator.Create(resettables, new List<IControllable>() { inputSorter }, waveSpawner);

        //********* Camera control *********
        SwipeHandler swipeHandler = new SwipeHandler(_leanSwipe);
        CameraMover cameraMover = new CameraMover(_camera, swipeHandler, _pinchDetector);
        
        //********* Other ************************
        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();
        _citySpawner.SpawnCity(new Vector2Int(0, 0), CitySize.Village, Side.Player);
        _citySpawner.SpawnCity(new Vector2Int(9, 0), CitySize.Village, Side.Player);

        //********  Debug  ***********
        _testButton.onClick.AddListener(OnTestButtonClick);
    }

    private void OnTestButtonClick()
    {
        _unitSpawner.TrySpawnUnit(new Vector2Int(0, 0), UnitType.Infantry, Side.Player);
    }
}
