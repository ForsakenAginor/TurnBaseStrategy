using Assets.Scripts.General;
using Assets.Scripts.HexGrid;
using Assets.Scripts.Sound.AudioMixer;
using Assets.Source.Scripts.GameLoop.StateMachine;
using Lean.Touch;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Root : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private LevelConfiguration _levelConfiguration;
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
    [SerializeField] private CityShopView _cityShop;

    [Header("Enemy")]
    [SerializeField] private EnemyBrain _enemyBrain;

    [Header("Camera")]
    [SerializeField] private Transform _camera;
    [SerializeField] private LeanFingerSwipe _leanSwipe;
    [SerializeField] private PinchDetector _pinchDetector;

    [Header("Other")]
    [SerializeField] private SoundInitializer _soundInitializer;

    private void Start()
    {
        //******** Load Data ***********
        SaveSystem saveSystem = new SaveSystem();
        GameLevel currentLevel = saveSystem.Load();

        //******** Init grid ***********
        _gridCreator.Init(currentLevel, _levelConfiguration);
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);
        var unitsGrid = _gridCreator.UnitsGrid;
        NewInputSorter inputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.BlockedCells);
        _cellHighlighter = new(inputSorter, _gridCreator.HexGrid, _gridColorConfiguration);

        //******** FogOfWar *********
        FogOfWar fogOfWar = new(_gridCreator.Clouds, unitsGrid);

        //******** Wallet ***********
        Resource wallet = new Resource(20, int.MaxValue);
        TaxSystem taxSystem = new TaxSystem(wallet, _citySpawner, _unitSpawner,
            _levelConfiguration.GetCityConfiguration(currentLevel), _levelConfiguration.GetUnitConfiguration(currentLevel));
        _walletView.Init(wallet);
        _cityShop.Init(_levelConfiguration.GetUnitConfiguration(currentLevel));

        //********  Unit creation  ***********
        UnitsActionsManager unitManager = new UnitsActionsManager(inputSorter, unitsGrid, _enemyBrain);
        _unitSpawner.Init(unitManager, wallet, _levelConfiguration.GetUnitConfiguration(currentLevel), unitsGrid, _gridCreator.BlockedCells);
        CitiesActionsManager cityManager = new CitiesActionsManager(inputSorter, unitsGrid);
        _citySpawner.Init(cityManager, _unitSpawner, wallet, _levelConfiguration.GetCityConfiguration(currentLevel), unitsGrid);
        CityAtMapInitializer cityInitializer = new CityAtMapInitializer(currentLevel, _levelConfiguration, _citySpawner);

        //********* EnemyLogic ***************
        _enemyBrain.Init(unitsGrid, _gridCreator.PathFinder, _unitSpawner, unitManager);
        cityInitializer.SpawnEnemyCities();
        EnemyWaveSpawner waveSpawner = new(cityManager.GetEnemyCitiesUnits(), _unitSpawner, _levelConfiguration.GetEnemyWaveConfiguration(currentLevel));
        EnemyScaner scaner = new(cityManager.GetEnemyCities(), _unitSpawner, unitsGrid, _levelConfiguration.GetEnemySpawnerConfiguration(currentLevel));

        //********* Game state machine *******
        _winLoseMonitor.Init(cityManager, saveSystem, currentLevel);
        var resettables = unitManager.Units.Append(taxSystem);
        var stateMachine = _gameStateMachineCreator.Create(resettables, new List<IControllable>() { inputSorter }, waveSpawner, currentLevel);

        //********* Camera control *********
        SwipeHandler swipeHandler = new SwipeHandler(_leanSwipe);
        CameraMover cameraMover = new CameraMover(_camera, swipeHandler, _pinchDetector, currentLevel, _levelConfiguration);

        //********* Other ************************
        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();
        cityInitializer.SpawnPlayerCities();

        //********* Sound ************************
        _soundInitializer.Init();

        if (MusicSingleton.Instance.IsAdded == false)
            _soundInitializer.AddMusicSource(MusicSingleton.Instance.Music);
        else
            _soundInitializer.AddMusicSourceWithoutVolumeChanging(MusicSingleton.Instance.Music);

        SceneChangerSingleton.Instance.FadeOut();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_gridCreator.HexGrid == null)
            return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 60;

        for (int x = 0; x < _gridCreator.HexGrid.Width; x++)
        {
            for (int z = 0; z < _gridCreator.HexGrid.Height; z++)
            {
                Vector3 cellPosition = _gridCreator.HexGrid.GetCellWorldPosition(x, z);
                UnityEditor.Handles.Label(cellPosition + Vector3.up * 0.5f, $"{x},{z}", style);
            }
        }
    }
#endif
}
