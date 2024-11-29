using Assets.Scripts.HexGrid;
using Assets.Source.Scripts.GameLoop.StateMachine;
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

    [Header("Enemy")]
    [SerializeField] private EnemyBrain _enemyBrain;

    [Header("Debug")]
    [SerializeField] private Button _testButton;

    private void Start()
    {
        _gridCreator.Init();
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);
        var unitsGrid = _gridCreator.UnitsGrid;
        NewInputSorter inputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.BlockedCells);
        _cellHighlighter = new (inputSorter, _gridCreator.HexGrid);

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
        _citySpawner.SpawnCity(new Vector2Int(6, 5), CitySize.City, Side.Enemy);
        EnemyScaner scaner = new(cityManager.GetEnemyCities(), _unitSpawner, unitsGrid, _enemySpawnerConfiguration);

        //********* Game state machine *******
        var resettables = unitManager.Units.Append(taxSystem);
        var stateMachine = _gameStateMachineCreator.Create(resettables, new List<IControllable>() { inputSorter });
        
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
