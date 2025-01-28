using Assets.Scripts.General;
using Assets.Scripts.HexGrid;
using Assets.Scripts.Sound.AudioMixer;
using Assets.Source.Scripts.GameLoop.StateMachine;
using Lean.Touch;
using System;
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
    [SerializeField] private GridRaycaster _gridRaycaster;
    [SerializeField] private CellSelector _cellSelector;

    [Header("Game progress")]
    [SerializeField] private GameStateMachineCreator _gameStateMachineCreator;
    [SerializeField] private WinLoseMonitor _winLoseMonitor;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private CitySpawner _citySpawner;

    [Header("Gold")]
    [SerializeField] private int _startGold = 20;
    [SerializeField] private WalletView _walletView;
    [SerializeField] private CityShopView _cityShop;
    [SerializeField] private IncomeView _incomeView;
    [SerializeField] private IncomeCompositionView _incomeCompositionView;
    [SerializeField] private BunkruptView _bakruptView;

    [Header("Enemy")]
    [SerializeField] private EnemyBrain _enemyBrain;

    [Header("Camera")]
    [SerializeField] private Transform _camera;
    [SerializeField] private LeanFingerSwipe _leanSwipe;
    [SerializeField] private PinchDetector _pinchDetector;

    [Header("Dialogue")]
    [SerializeField] private DialogueView _dialogueView;

    [Header("Other")]
    [SerializeField] private SoundInitializer _soundInitializer;

    private void Start()
    {
        //******** Load Data ***********
        SaveSystem saveSystem = new SaveSystem();
        GameLevel currentLevel = saveSystem.Load();

        //********* Sound ************************
        _soundInitializer.Init();

        if (MusicSingleton.Instance.IsAdded == false)
            _soundInitializer.AddMusicSource(MusicSingleton.Instance.Music);
        else
            _soundInitializer.AddMusicSourceWithoutVolumeChanging(MusicSingleton.Instance.Music);

        //******** Init grid ***********
        _gridCreator.Init(currentLevel, _levelConfiguration);
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);
        var unitsGrid = _gridCreator.UnitsGrid;
        NewInputSorter inputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.BlockedCells, _gridCreator.PathFinder);
        CellHighlighter _cellHighlighter = new(inputSorter, _gridCreator.HexGrid, _gridColorConfiguration);
        _ = new HexContentSwitcher(unitsGrid, _gridCreator.BlockedCells);

        //******** Wallet ***********
        Resource wallet = new Resource(_startGold, int.MaxValue);
        TaxSystem taxSystem = new TaxSystem(wallet, _citySpawner, _unitSpawner,
            _levelConfiguration.GetCityConfiguration(currentLevel), _levelConfiguration.GetUnitConfiguration(currentLevel));
        _walletView.Init(wallet);
        _cityShop.Init(_levelConfiguration.GetUnitConfiguration(currentLevel));
        _incomeView.Init(taxSystem);
        _incomeCompositionView.Init(taxSystem);
        _bakruptView.Init(taxSystem);

        //********  Unit creation  ***********
        UnitsActionsManager unitManager = new UnitsActionsManager(inputSorter, unitsGrid, _enemyBrain, _gridCreator.Clouds);
        _unitSpawner.Init(unitManager, wallet, _levelConfiguration.GetUnitConfiguration(currentLevel), unitsGrid, _gridCreator.BlockedCells, AddAudioSourceToMixer);
        CitiesActionsManager cityManager = new CitiesActionsManager(inputSorter, unitsGrid);
        _citySpawner.Init(_levelConfiguration.GetCitiesNames(currentLevel),
            cityManager, _unitSpawner, wallet, _levelConfiguration.GetCityConfiguration(currentLevel), unitsGrid, AddAudioSourceToMixer);
        CityAtMapInitializer cityInitializer = new CityAtMapInitializer(currentLevel, _levelConfiguration, _citySpawner);

        //********* EnemyLogic ***************
        _enemyBrain.Init(unitsGrid, _gridCreator.PathFinderAI, _unitSpawner, unitManager);
        cityInitializer.SpawnEnemyCities();
        EnemyWaveSpawner waveSpawner = new(cityManager.GetEnemyCitiesUnits(), _unitSpawner, _levelConfiguration.GetEnemyWaveConfiguration(currentLevel));
        EnemyScaner scaner = new(cityManager.GetEnemyCities(), _unitSpawner, unitsGrid, _levelConfiguration.GetEnemySpawnerConfiguration(currentLevel));
        cityManager.SetScaner(scaner);

        //******** FogOfWar *********
        FogOfWar fogOfWar = new(_gridCreator.Clouds, unitsGrid, scaner);

        //********* Game state machine *******
        _winLoseMonitor.Init(cityManager, saveSystem, currentLevel);
        var resettables = unitManager.Units.Append(taxSystem);
        var stateMachine = _gameStateMachineCreator.Create(resettables, new List<IControllable>() { inputSorter }, inputSorter, waveSpawner, currentLevel);

        //********* Camera control *********
        SwipeHandler swipeHandler = new SwipeHandler(_leanSwipe);
        CameraMover cameraMover = new CameraMover(_camera, swipeHandler, _pinchDetector, currentLevel, _levelConfiguration,
            _gridCreator.HexGrid, scaner);

        //********* Dialogue *********
        Dialogue dialogue = new Dialogue(_levelConfiguration.GetCitiesBossInfo(currentLevel), scaner);
        _dialogueView.Init(dialogue);

        //********* Other ************************
        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();
        cityInitializer.SpawnPlayerCities();


        SceneChangerSingleton.Instance.FadeOut();
    }

    private void AddAudioSourceToMixer(AudioSource audioSource)
    {
        if(audioSource == null)
            throw new ArgumentNullException(nameof(audioSource));

        _soundInitializer.AddEffectSource(audioSource);
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
