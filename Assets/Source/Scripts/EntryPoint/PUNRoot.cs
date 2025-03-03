using Assets.Scripts.General;
using Assets.Scripts.HexGrid;
using Assets.Scripts.Sound.AudioMixer;
using Assets.Source.Scripts.GameLoop.StateMachine;
using Lean.Touch;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PUNRoot : MonoBehaviour
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
    [SerializeField] private SwitchableElement[] _enabledWindows;
    [SerializeField] private SwitchableElement[] _disabledWindows;
    [SerializeField] private SoundInitializer _soundInitializer;
    [SerializeField] private Quests _questsPlayer1;
    [SerializeField] private Quests _questsPlayer2;
    [SerializeField] private DayView _dayView;
    [SerializeField] private SaveSystemView _saveSystemView;
    [SerializeField] private Tutorial _tutorial;
    [SerializeField] private EnemyActivityMonitorView _enemyActivityMonitorView;
    [SerializeField] private Button _nextTurnButton;

    private void Start()
    {
        bool isFirstPlayer = PhotonNetwork.IsMasterClient;

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
        _gridCreator.Init(currentLevel, _levelConfiguration);
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);
        var unitsGrid = _gridCreator.UnitsGrid;
        NewInputSorter inputSorter;
        Side playerSide = isFirstPlayer ? Side.Player : Side.Enemy;

        if (isFirstPlayer)
            inputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.BlockedCells, _gridCreator.PathFinder, Side.Player, Side.Enemy);
        else
            inputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.BlockedCells, _gridCreator.PathFinder, Side.Enemy, Side.Player);

        CellHighlighter _cellHighlighterFirst = new(inputSorter, _gridCreator.HexGrid, _gridColorConfiguration);
        _ = new HexContentSwitcher(unitsGrid, _gridCreator.BlockedCells);

        //******** Network ***********
        PhotonEventReceiver photonReceiver = new PhotonEventReceiver(unitsGrid);
        PhotonEventSender photonSender = new PhotonEventSender(inputSorter, _citySpawner, _unitSpawner, _nextTurnButton);

        //******** Wallet ***********
        Resource wallet1 = isLoaded ? new Resource(loadedGame.Wallet, int.MaxValue) : new Resource(_startGold, int.MaxValue);
        Resource wallet2 = isLoaded ? new Resource(loadedGame.Wallet, int.MaxValue) : new Resource(_startGold, int.MaxValue);
        TaxSystem taxSystem1 = new TaxSystem(wallet1, _citySpawner, _unitSpawner,
            _levelConfiguration.GetCitiesUpgradeCost(currentLevel), _levelConfiguration.GetUnitConfiguration(currentLevel), Side.Player);
        TaxSystem taxSystem2 = new TaxSystem(wallet2, _citySpawner, _unitSpawner,
            _levelConfiguration.GetCitiesUpgradeCost(currentLevel), _levelConfiguration.GetUnitConfiguration(currentLevel), Side.Enemy);
        _cityShop.Init(_levelConfiguration.GetUnitConfiguration(currentLevel), _levelConfiguration.GetCitiesUpgradeCost(currentLevel));

        if (isFirstPlayer)
        {
            EconomyFacade economyFacadeFirst = new EconomyFacade(_walletView, _incomeView, _incomeCompositionView, _bankruptView, wallet1, taxSystem1);
            economyFacadeFirst.EnableControl();
        }
        else
        {
            EconomyFacade economyFacadeSecond = new EconomyFacade(_walletView, _incomeView, _incomeCompositionView, _bankruptView, wallet2, taxSystem2);
            economyFacadeSecond.EnableControl();
        }

        //********  Unit creation  ***********
        UnitsActionsManager unitManager = new UnitsActionsManager(inputSorter, unitsGrid, photonReceiver,
            new Dictionary<Side, HexGridXZ<ICloud>>() { { playerSide, _gridCreator.Clouds } });
        _unitSpawner.InitPUN(unitManager, wallet1, wallet2, _levelConfiguration.GetUnitConfiguration(currentLevel),
            unitsGrid, _gridCreator.BlockedCells, AddAudioSourceToMixer, photonReceiver);
        CitiesActionsManager cityManager = new CitiesActionsManager(inputSorter, unitsGrid);
        _citySpawner.InitPUN(_levelConfiguration.GetCitiesNames(currentLevel),
            cityManager, _unitSpawner, wallet1, wallet2, _levelConfiguration.GetCityConfiguration(currentLevel), _levelConfiguration.GetCitiesUpgradeCost(currentLevel),
            unitsGrid, AddAudioSourceToMixer, photonReceiver);
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

        //******** FogOfWar *********
        CitySearcher scaner;
        FogOfWar fogOfWar;

        if (isFirstPlayer)
        {
            scaner = new CitySearcher(cityManager.GetEnemyCities().Union(cityManager.GetNeutralities()), unitsGrid, Side.Enemy);
            fogOfWar = new(_gridCreator.Clouds, unitsGrid, scaner, new List<Side>() { Side.Enemy });
            fogOfWar.ApplyDiscoveredCells(cityManager.GetPlayerCities());
        }
        else
        {
            scaner = new CitySearcher(cityManager.GetPlayerCities().Union(cityManager.GetNeutralities()), unitsGrid, Side.Player);
            fogOfWar = new(_gridCreator.Clouds, unitsGrid, scaner, new List<Side>() { Side.Player });
            fogOfWar.ApplyDiscoveredCells(cityManager.GetEnemyCities());
        }

        //********* Game state machine *******
        TurnDependantWindowSwitcher turnDependantWindowSwitcher = new (_enabledWindows, _disabledWindows);
        _winLoseMonitor.Init(cityManager, saveLevelSystem, currentLevel);
        var resettables = unitManager.Units.Append(taxSystem1).Append(taxSystem2).Append(daySystem);
        List<IControllable> controllables = new List<IControllable>() { inputSorter, _saveSystemView, turnDependantWindowSwitcher };
        var stateMachine = _gameStateMachineCreator.CreateMultiplayer(photonReceiver, resettables, controllables, inputSorter, currentLevel, isFirstPlayer);

        //********* Camera control *********
        bool isMobile = Application.isMobilePlatform &&
            (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android);
        IZoomInput zoomInput = isMobile ? new MobileInput() : new PCInput();
        _pinchDetector.Init(zoomInput);
        EnemyActivityMonitor activityMonitor = new(unitManager);
        _enemyActivityMonitorView.Init(activityMonitor);
        CameraMover cameraMover = new CameraMover(_camera, _pinchDetector, currentLevel, _levelConfiguration,
            _gridCreator.HexGrid, scaner, unitManager, _swipeInputReceiver, _gridRaycaster, isFirstPlayer, _enemyActivityMonitorView);
        controllables.Add(cameraMover);
        controllables.Add(activityMonitor);

        //********* Other ************************
        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();
        _questsPlayer1.Init(cityManager, _levelConfiguration.GetCitiesNames(currentLevel), Side.Player);
        _questsPlayer2.Init(cityManager, _levelConfiguration.GetCitiesNames(currentLevel), Side.Enemy);


        if (isFirstPlayer)
        {
            controllables.Add(_questsPlayer1);
            _questsPlayer1.EnableControl();
        }
        else
        {
            controllables.Add(_questsPlayer2);
            _questsPlayer2.EnableControl();
        }
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
