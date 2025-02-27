using Assets.Scripts.HexGrid;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PhotonEventReceiver : IUnitActionController
{
    private readonly Dictionary<byte, Action<object>> _eventHandlers = new Dictionary<byte, Action<object>>();
    private readonly HexGridXZ<Unit> _grid;

    public PhotonEventReceiver(HexGridXZ<Unit> grid)
    {
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));

        _eventHandlers.Add(1, UnitMove);
        _eventHandlers.Add(2, UnitAttack);
        _eventHandlers.Add(3, CityUpgrade);
        _eventHandlers.Add(4, UnitHire);
        _eventHandlers.Add(5, SkipTurn);

        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    ~PhotonEventReceiver()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public event Action<WalkableUnit, Vector3, Unit, Action> UnitAttacking;
    public event Action<WalkableUnit, IEnumerable<Vector2Int>, Action> UnitMoving;
    public event Action<Upgrades, Vector3> CityUpgrading;
    public event Action<Vector2Int, UnitType, Side> UnitHiring;
    public event Action TurnSkiping;

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (_eventHandlers.ContainsKey(eventCode))
            _eventHandlers[eventCode].Invoke(photonEvent.CustomData);
        else
            throw new Exception("Incorrect photon event type");
    }

    private void SkipTurn(object _)
    {
        TurnSkiping?.Invoke();
    }

    private void UnitHire(object data)
    {
        object[] array = (object[])data;

        Vector2Int position = (Vector2Int)array[0];
        UnitType type = (UnitType)(int)array[1];
        Side side = (Side)(int)array[2];

        UnitHiring?.Invoke(position, type, side);
    }

    private void CityUpgrade(object data)
    {
        object[] array = (object[])data;

        Upgrades upgradeType = (Upgrades)(int)array[0];
        Vector3 position = (Vector3)array[1];

        CityUpgrading?.Invoke(upgradeType, position);
    }


    private void UnitMove(object data)
    {
        object[] array = (object[])data;

        Vector2Int unitPosition = (Vector2Int)array[0];
        Vector2Int[] path = (Vector2Int[])array[1];
        WalkableUnit unit = _grid.GetGridObject(unitPosition) as WalkableUnit;

        UnitMoving?.Invoke(unit, path, null);
    }

    private void UnitAttack(object data)
    {
        object[] array = (object[])data;

        Vector2Int unitPosition = (Vector2Int)array[0];
        Vector3 position = (Vector3)array[1];
        Vector2Int targetPosition = (Vector2Int)array[2];
        WalkableUnit unit = _grid.GetGridObject(unitPosition) as WalkableUnit;
        Unit target = _grid.GetGridObject(targetPosition);

        UnitAttacking?.Invoke(unit, position, target, null);
    }
}

public class PhotonEventSender
{
    private const byte CodeUnitMove = 1;
    private const byte CodeUnitAttack = 2;
    private const byte CodeCityUpgrade = 3;
    private const byte CodeUnitHire = 4;
    private const byte CodeNextTurn = 5;

    private readonly NewInputSorter _input;
    private readonly CitySpawner _citySpawner;
    private readonly UnitSpawner _unitSpawner;
    private readonly Button _nextTurnButton;
    private readonly RaiseEventOptions _eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

    public PhotonEventSender(NewInputSorter input, CitySpawner citySpawner, UnitSpawner unitSpawner, Button nextTurnButton)
    {
        _input = input != null ? input : throw new ArgumentNullException(nameof(input));
        _citySpawner = citySpawner != null ? citySpawner : throw new ArgumentNullException(nameof(citySpawner));
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));

        _input.UnitIsMovingPUN += OnUnitMoving;
        _input.UnitIsAttackingPUN += OnUnitAttacking;
        _citySpawner.CityUpgraded += OnCityUpgrading;
        _unitSpawner.UnitHired += OnUnitHiring;
        _nextTurnButton.onClick.AddListener(OnNextTurnButtonClick);
    }

    ~PhotonEventSender()
    {
        _input.UnitIsMovingPUN -= OnUnitMoving;
        _input.UnitIsAttackingPUN -= OnUnitAttacking;
        _citySpawner.CityUpgraded -= OnCityUpgrading;
        _unitSpawner.UnitHired -= OnUnitHiring;
        _nextTurnButton.onClick.RemoveListener(OnNextTurnButtonClick);
    }

    private void OnNextTurnButtonClick()
    {
        PhotonNetwork.RaiseEvent(CodeNextTurn, null, _eventOptions, SendOptions.SendReliable);
    }

    private void OnUnitHiring(Vector2Int position, UnitType type, Side side)
    {
        object[] content = new object[] { position, (int)type, (int)side };

        PhotonNetwork.RaiseEvent(CodeUnitHire, content, _eventOptions, SendOptions.SendReliable);
    }

    private void OnCityUpgrading(Upgrades upgrades, Vector3 vector)
    {
        object[] content = new object[] { (int)upgrades, vector };

        PhotonNetwork.RaiseEvent(CodeCityUpgrade, content, _eventOptions, SendOptions.SendReliable);
    }

    private void OnUnitAttacking(Vector2Int unitPosition, Vector3 vector, Vector2Int targetPosition)
    {
        object[] content = new object[] { unitPosition, vector, targetPosition};

        PhotonNetwork.RaiseEvent(CodeUnitAttack, content, _eventOptions, SendOptions.SendReliable);
    }

    private void OnUnitMoving(Vector2Int unitPosition, IEnumerable<Vector2Int> path)
    {
        object[] content = new object[] { unitPosition, path.ToArray()};

        PhotonNetwork.RaiseEvent(CodeUnitMove, content, _eventOptions, SendOptions.SendReliable);
    }
}
