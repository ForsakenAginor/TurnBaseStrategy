using Assets.Scripts.General;
using Assets.Scripts.HexGrid;
using Assets.Scripts.Sound.AudioMixer;
using Assets.Source.Scripts.GameLoop.StateMachine;
using Lean.Touch;
using Sirenix.Utilities;
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
    [SerializeField] private BunkruptView _bankruptView;

    [Header("Camera")]
    [SerializeField] private Transform _camera;
    [SerializeField] private LeanFingerSwipe _leanSwipe;
    [SerializeField] private PinchDetector _pinchDetector;
    [SerializeField] private TouchInput _swipeInputReceiver;

    [Header("Other")]
    [SerializeField] private SoundInitializer _soundInitializer;
    [SerializeField] private Quests _questsPlayer1;
    [SerializeField] private Quests _questsPlayer2;
    [SerializeField] private DayView _dayView;
    [SerializeField] private SaveSystemView _saveSystemView;
    [SerializeField] private Tutorial _tutorial;
    [SerializeField] private EnemyActivityMonitorView _enemyActivityMonitorView;

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
            //---------------------
            //until we have 1 lvl for hotsit
            currentLevel = GameLevel.First;
            //---------------------
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
        _gridCreator.InitHotSit(currentLevel, _levelConfiguration);
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);
        var unitsGrid = _gridCreator.UnitsGrid;
        NewInputSorter player1InputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.BlockedCells, _gridCreator.PathFinder, Side.Player, Side.Enemy);
        NewInputSorter player2InputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.BlockedCells, _gridCreator.PathFinder, Side.Enemy, Side.Player);
        CellHighlighter _cellHighlighterFirst = new(player1InputSorter, _gridCreator.HexGrid, _gridColorConfiguration);
        CellHighlighter _cellHighlighterSecond = new(player2InputSorter, _gridCreator.HexGrid, _gridColorConfiguration);
        _ = new HexContentSwitcher(unitsGrid, _gridCreator.BlockedCells);

        //******** Tutorial ***********
        _tutorial.Init(_citySpawner, player1InputSorter, _unitSpawner);

        //******** Wallet ***********
        Resource wallet1 = isLoaded ? new Resource(loadedGame.Wallet, int.MaxValue) : new Resource(_startGold, int.MaxValue);
        Resource wallet2 = isLoaded ? new Resource(loadedGame.Wallet, int.MaxValue) : new Resource(_startGold, int.MaxValue);
        TaxSystem taxSystem1 = new TaxSystem(wallet1, _citySpawner, _unitSpawner,
            _levelConfiguration.GetCitiesUpgradeCost(currentLevel), _levelConfiguration.GetUnitConfiguration(currentLevel), Side.Player);
        TaxSystem taxSystem2 = new TaxSystem(wallet2, _citySpawner, _unitSpawner,
            _levelConfiguration.GetCitiesUpgradeCost(currentLevel), _levelConfiguration.GetUnitConfiguration(currentLevel), Side.Enemy);
        _cityShop.Init(_levelConfiguration.GetUnitConfiguration(currentLevel), _levelConfiguration.GetCitiesUpgradeCost(currentLevel));
        EconomyFacade economyFacadeFirst = new EconomyFacade(_walletView, _incomeView, _incomeCompositionView, _bankruptView, wallet1, taxSystem1);
        EconomyFacade economyFacadeSecond = new EconomyFacade(_walletView, _incomeView, _incomeCompositionView, _bankruptView, wallet2, taxSystem2);

        //********  Unit creation  ***********
        UnitsActionsManager unitManager = new UnitsActionsManager(new List<NewInputSorter>() { player1InputSorter, player2InputSorter },
            unitsGrid,
            new Dictionary<Side, HexGridXZ<ICloud>>() { { Side.Player, _gridCreator.OtherPlayerClouds }, { Side.Enemy, _gridCreator.Clouds } });
        _unitSpawner.Init(unitManager, wallet1, _levelConfiguration.GetUnitConfiguration(currentLevel), unitsGrid, _gridCreator.BlockedCells, AddAudioSourceToMixer);
        CitiesActionsManager cityManager = new CitiesActionsManager(new List<NewInputSorter>() { player1InputSorter, player2InputSorter }, unitsGrid);
        _citySpawner.InitHotSit(_levelConfiguration.GetCitiesNames(currentLevel),
            cityManager, _unitSpawner, wallet1, wallet2, _levelConfiguration.GetCityConfiguration(currentLevel), _levelConfiguration.GetCitiesUpgradeCost(currentLevel),
            unitsGrid, AddAudioSourceToMixer);
        CityAtMapInitializer cityInitializer = new CityAtMapInitializer(currentLevel, _levelConfiguration, _citySpawner);

        if (isLoaded)
        {
            cityInitializer.SpawnCitiesFromLoadedData(loadedGame.Cities, loadedGame.CitiesWithAvailableSpawns);
            _unitSpawner.SpawnLoadedUnits(loadedGame.Units);
        }
        else
        {
            cityInitializer.SpawnPlayerCities();
            cityInitializer.SpawnEnemyCities();
            cityInitializer.SpawnNeutralCities();
        }

        CitySearcher scanerFirst = new CitySearcher(cityManager.GetEnemyCities().Union(cityManager.GetNeutralities()), unitsGrid, Side.Enemy);
        CitySearcher scanerSecond = new CitySearcher(cityManager.GetPlayerCities().Union(cityManager.GetNeutralities()), unitsGrid, Side.Player);

        //******** FogOfWar *********
        FogOfWar fogOfWar = new(_gridCreator.Clouds, unitsGrid, scanerFirst, new List<Side>(){Side.Enemy });
        FogOfWar fogOfWarSecond = new(_gridCreator.OtherPlayerClouds, unitsGrid, scanerSecond, new List<Side>() { Side.Player });

        fogOfWar.ApplyDiscoveredCells(cityManager.GetPlayerCities());
        fogOfWarSecond.ApplyDiscoveredCells(cityManager.GetEnemyCities());

        //********* Game state machine *******
        _winLoseMonitor.Init(cityManager, saveLevelSystem, currentLevel);
        var resettables = unitManager.Units.Append(taxSystem1).Append(taxSystem2).Append(daySystem);
        List<IControllable> controllables1 = new List<IControllable>() { player1InputSorter, _saveSystemView, fogOfWar, economyFacadeFirst };
        List<IControllable> controllables2 = new List<IControllable>() { player2InputSorter, fogOfWarSecond, economyFacadeSecond};
        var stateMachine = _gameStateMachineCreator.CreateHotSit(resettables, controllables1, player1InputSorter,
            controllables2, player2InputSorter,
            currentLevel);

        //********* Camera control *********
        bool isMobile = Application.isMobilePlatform &&
            (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android);
        IZoomInput zoomInput = isMobile ? new MobileInput() : new PCInput();
        _pinchDetector.Init(zoomInput);
        EnemyActivityMonitor activityMonitor = new(unitManager);
        _enemyActivityMonitorView.Init(activityMonitor);
        HotSitCameraMover cameraMover1 = new HotSitCameraMover(_camera, _pinchDetector, currentLevel, _levelConfiguration,
            _gridCreator.HexGrid, scanerFirst, unitManager, _swipeInputReceiver, _gridRaycaster, _enemyActivityMonitorView);
        HotSitCameraMover cameraMover2 = new HotSitCameraMover(_camera, _pinchDetector, currentLevel, _levelConfiguration,
            _gridCreator.HexGrid, scanerSecond, unitManager, _swipeInputReceiver, _gridRaycaster,
            _enemyActivityMonitorView, true);

        controllables1.Add(cameraMover1);
        controllables1.Add(activityMonitor);
        controllables2.Add(cameraMover2);
        controllables2.Add(activityMonitor);

        //********* Other ************************
        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();
        _questsPlayer1.Init(cityManager, _levelConfiguration.GetCitiesNames(currentLevel), Side.Player);
        _questsPlayer2.Init(cityManager, _levelConfiguration.GetCitiesNames(currentLevel), Side.Enemy);
        controllables1.Add(_questsPlayer1);
        controllables2.Add(_questsPlayer2);
        _questsPlayer1.EnableControl();
        //saveSystem.Init(fogOfWar, unitManager, cityManager, wallet1, daySystem, currentLevel, scaner);
        //_saveSystemView.Init(saveSystem);

        SceneChangerSingleton.Instance.FadeOut();
    }

    private void AddAudioSourceToMixer(AudioSource audioSource)
    {
        if (audioSource == null)
            throw new ArgumentNullException(nameof(audioSource));

        _soundInitializer.AddEffectSource(audioSource);
    }
}
