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
            currentLevel = saveLevelSystem.LoadLevel();
            daySystem = new ();
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
        NewInputSorter inputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.BlockedCells, _gridCreator.PathFinder, Side.Player, Side.Enemy);
        CellHighlighter _cellHighlighter = new(inputSorter, _gridCreator.HexGrid, _gridColorConfiguration);
        _ = new HexContentSwitcher(unitsGrid, _gridCreator.BlockedCells);

        //******** Tutorial ***********
        _tutorial.Init(_citySpawner, inputSorter, _unitSpawner);

        //******** Wallet ***********
        Resource wallet = isLoaded ? new Resource(loadedGame.Wallet, int.MaxValue) : new Resource(_startGold, int.MaxValue);
        TaxSystem taxSystem = new TaxSystem(wallet, _citySpawner, _unitSpawner,
            _levelConfiguration.GetCityConfiguration(currentLevel), _levelConfiguration.GetUnitConfiguration(currentLevel), Side.Player);
        AITaxSystem aITaxSystem = new AITaxSystem(_citySpawner, _unitSpawner, Side.Enemy);
        _cityShop.Init(_levelConfiguration.GetUnitConfiguration(currentLevel));
        EconomyFacade economyFacade = new EconomyFacade(_walletView, _incomeView, _incomeCompositionView, _bakruptView, wallet, taxSystem);
        economyFacade.EnableControl();

        //********  Unit creation  ***********
        UnitsActionsManager unitManager = new UnitsActionsManager(inputSorter, unitsGrid, _enemyBrain,
            new Dictionary<Side, HexGridXZ<ICloud>>() { { Side.Player, _gridCreator.Clouds } });
        _unitSpawner.Init(unitManager, wallet, _levelConfiguration.GetUnitConfiguration(currentLevel), unitsGrid, _gridCreator.BlockedCells, AddAudioSourceToMixer);
        CitiesActionsManager cityManager = new CitiesActionsManager(inputSorter, unitsGrid);
        _citySpawner.Init(_levelConfiguration.GetCitiesNames(currentLevel),
            cityManager, _unitSpawner, wallet, _levelConfiguration.GetCityConfiguration(currentLevel), unitsGrid, AddAudioSourceToMixer);
        CityAtMapInitializer cityInitializer = new CityAtMapInitializer(currentLevel, _levelConfiguration, _citySpawner);

        //********* EnemyLogic ***************
        _enemyBrain.Init(unitsGrid, _gridCreator.PathFinderAI, _unitSpawner, unitManager);

        if (isLoaded)
            cityInitializer.SpawnCitiesFromLoadedData(loadedGame.Cities,loadedGame.CitiesWithAvailableSpawns);
        else
            cityInitializer.SpawnEnemyCities();

        EnemyWaveSpawner waveSpawner = new(cityManager.GetEnemyCitiesUnits(), _unitSpawner, _levelConfiguration.GetEnemyWaveConfiguration(currentLevel), daySystem);
        var citiesWithGuards = isLoaded ? loadedGame.CitiesWithAvailableSpawns : cityManager.GetEnemyCities();
        EnemyScaner scaner = new(citiesWithGuards, _unitSpawner, unitsGrid, _levelConfiguration.GetEnemySpawnerConfiguration(currentLevel));

        //******** FogOfWar *********
        FogOfWar fogOfWar = new(_gridCreator.Clouds, unitsGrid, scaner, new List<Side>() { Side.Enemy });

        if (isLoaded)
            fogOfWar.ApplyLoadedData(loadedGame.DiscoveredCells);

        //********* Game state machine *******
        _winLoseMonitor.Init(cityManager, saveLevelSystem, currentLevel);
        var resettables = unitManager.Units.Append(taxSystem).Append(aITaxSystem);
        resettables = resettables.Append(daySystem);
        List<IControllable> controllables = new List<IControllable>() { inputSorter, _saveSystemView };
        var stateMachine = _gameStateMachineCreator.Create(resettables, controllables,
            inputSorter, currentLevel);

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
        saveSystem.Init(fogOfWar, unitManager, cityManager, wallet, daySystem, currentLevel, scaner);
        _saveSystemView.Init(saveSystem);

        SceneChangerSingleton.Instance.FadeOut();
    }

    private void AddAudioSourceToMixer(AudioSource audioSource)
    {
        if (audioSource == null)
            throw new ArgumentNullException(nameof(audioSource));

        _soundInitializer.AddEffectSource(audioSource);
    }
    /*
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_gridCreator.HexGrid == null)
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
    */
}
