using Assets.Scripts.General;
using Assets.Scripts.HexGrid;
using Assets.Scripts.Sound.AudioMixer;
using Assets.Source.Scripts.GameLoop.StateMachine;
using Lean.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HotSitRoot : MonoBehaviour
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
    [SerializeField] private TouchInput _swipeInputReceiver;

    [Header("Dialogue")]
    [SerializeField] private DialogueView _dialogueView;

    [Header("Other")]
    [SerializeField] private SoundInitializer _soundInitializer;
    [SerializeField] private Quests _quests;
    [SerializeField] private DayView _dayView;
    [SerializeField] private SaveSystemView _saveSystemView;
    [SerializeField] private Tutorial _tutorial;

    private void Start()
    {
        //******** Load Data ***********
        GameLevel currentLevel;
        SaveLevelSystem saveLevelSystem = new SaveLevelSystem();
        SaveSystem saveSystem = new SaveSystem();
        DaySystem daySystem;
        SavedData loadedGame = null;
        bool isLoaded = LoadGameSingleton.Instance.IsContinueGame;

        if (isLoaded)
        {
            loadedGame = saveSystem.Load();
            currentLevel = loadedGame.GameLevel;
            daySystem = new DaySystem(loadedGame.Day);
        }
        else
        {
            //**********************
            //until we have 1 lvl for hotsit
            currentLevel = GameLevel.First;
            //**********************
            daySystem = new();
        }

        _dayView.Init(daySystem);

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
        NewInputSorter player1InputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.BlockedCells, _gridCreator.PathFinder, Side.Player, Side.Enemy);
        NewInputSorter player2InputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.BlockedCells, _gridCreator.PathFinder, Side.Enemy, Side.Player);
        CellHighlighter _cellHighlighterFirst = new(player1InputSorter, _gridCreator.HexGrid, _gridColorConfiguration);
        CellHighlighter _cellHighlighterSecond = new(player2InputSorter, _gridCreator.HexGrid, _gridColorConfiguration);
        _ = new HexContentSwitcher(unitsGrid, _gridCreator.BlockedCells);

        //******** Tutorial ***********
        //_tutorial.Init(_citySpawner, player1InputSorter, _unitSpawner);

        //******** Wallet ***********
        Resource wallet1 = isLoaded ? new Resource(loadedGame.Wallet, int.MaxValue) : new Resource(_startGold, int.MaxValue);
        Resource wallet2 = isLoaded ? new Resource(loadedGame.Wallet, int.MaxValue) : new Resource(_startGold, int.MaxValue);
        TaxSystem taxSystem = new TaxSystem(wallet1, _citySpawner, _unitSpawner,
            _levelConfiguration.GetCityConfiguration(currentLevel), _levelConfiguration.GetUnitConfiguration(currentLevel), Side.Player);
        _walletView.Init(wallet1);
        _cityShop.Init(_levelConfiguration.GetUnitConfiguration(currentLevel));
        _incomeView.Init(taxSystem);
        _incomeCompositionView.Init(taxSystem);
        _bakruptView.Init(taxSystem);

        //********  Unit creation  ***********
        UnitsActionsManager unitManager = new UnitsActionsManager(player1InputSorter, unitsGrid, _enemyBrain, _gridCreator.Clouds);
        _unitSpawner.Init(unitManager, wallet1, _levelConfiguration.GetUnitConfiguration(currentLevel), unitsGrid, _gridCreator.BlockedCells, AddAudioSourceToMixer);
        CitiesActionsManager cityManager = new CitiesActionsManager(player1InputSorter, unitsGrid);
        _citySpawner.Init(_levelConfiguration.GetCitiesNames(currentLevel),
            cityManager, _unitSpawner, wallet1, _levelConfiguration.GetCityConfiguration(currentLevel), unitsGrid, AddAudioSourceToMixer);
        CityAtMapInitializer cityInitializer = new CityAtMapInitializer(currentLevel, _levelConfiguration, _citySpawner);

        //********* EnemyLogic ***************
        _enemyBrain.Init(unitsGrid, _gridCreator.PathFinderAI, _unitSpawner, unitManager);

        if (isLoaded)
            cityInitializer.SpawnCitiesFromLoadedData(loadedGame.Cities, loadedGame.CitiesWithAvailableSpawns);
        else
            cityInitializer.SpawnEnemyCities();

        EnemyWaveSpawner waveSpawner = new(cityManager.GetEnemyCitiesUnits(), _unitSpawner, _levelConfiguration.GetEnemyWaveConfiguration(currentLevel), daySystem);
        var citiesWithGuards = isLoaded ? loadedGame.CitiesWithAvailableSpawns : cityManager.GetEnemyCities();
        EnemyScaner scaner = new(citiesWithGuards, _unitSpawner, unitsGrid, _levelConfiguration.GetEnemySpawnerConfiguration(currentLevel));
        cityManager.SetScaner(scaner);

        //******** FogOfWar *********
        FogOfWar fogOfWar = new(_gridCreator.Clouds, unitsGrid, scaner);

        if (isLoaded)
            fogOfWar.ApplyLoadedData(loadedGame.DiscoveredCells);

        //********* Game state machine *******
        _winLoseMonitor.Init(cityManager, saveLevelSystem, currentLevel);
        var resettables = unitManager.Units.Append(taxSystem);
        resettables = resettables.Append(daySystem);
        List<IControllable> controllables = new List<IControllable>() { player1InputSorter, _saveSystemView };
        var stateMachine = _gameStateMachineCreator.Create(resettables, controllables,
            player1InputSorter, currentLevel);

        //********* Camera control *********
        bool isMobile = Application.isMobilePlatform &&
            (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android);
        IZoomInput zoomInput = isMobile ? new MobileInput() : new PCInput();
        _pinchDetector.Init(zoomInput);
        CameraMover cameraMover = new CameraMover(_camera, _pinchDetector, currentLevel, _levelConfiguration,
            _gridCreator.HexGrid, scaner, unitManager, _swipeInputReceiver, _gridRaycaster);
        controllables.Add(cameraMover);

        //********* Dialogue *********
        Dialogue dialogue = new Dialogue(_levelConfiguration.GetCitiesBossInfo(currentLevel), scaner);
        _dialogueView.Init(dialogue);

        //********* Other ************************
        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();

        if (isLoaded == false)
            cityInitializer.SpawnPlayerCities();
        else
            _unitSpawner.SpawnLoadedUnits(loadedGame.Units);

        _quests.Init(cityManager, _levelConfiguration.GetCitiesNames(currentLevel));
        saveSystem.Init(fogOfWar, unitManager, cityManager, wallet1, daySystem, currentLevel, scaner);
        _saveSystemView.Init(saveSystem);

        SceneChangerSingleton.Instance.FadeOut();
    }

    private void AddAudioSourceToMixer(AudioSource audioSource)
    {
        if (audioSource == null)
            throw new ArgumentNullException(nameof(audioSource));

        _soundInitializer.AddEffectSource(audioSource);
    }
}
