using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSingleton : MonoBehaviour
{
    [SerializeField] private Button _hireButton;
    [SerializeField] private TutorialPointer _hirePointer;
    [SerializeField] private GameObject _canvasPrefab;
    [SerializeField] private SwitchableElement _cityMenu;
    [SerializeField] private SwitchableElement _onGridPointer;
    [SerializeField] private Button _nextTurnButton;

    private IPlayerUnitSpawner _citySpawner;
    private IPlayerUnitSpawner _unitSpawner;
    private NewInputSorter _newInputSorter;
    private GameObject _pointer;
    private Tween _nextTurnPulse;

    public void Init(IPlayerUnitSpawner citySpawner, NewInputSorter inputSorter, IPlayerUnitSpawner unitSpawner)
    {
        _citySpawner = citySpawner != null ? citySpawner : throw new ArgumentNullException(nameof(citySpawner));
        _newInputSorter = inputSorter != null ? inputSorter : throw new ArgumentNullException(nameof(inputSorter));
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));


        _newInputSorter.FriendlyCitySelected += OnCitySelected;
        _newInputSorter.MovableUnitSelected += OnUnitSelected;
        _newInputSorter.UnitIsMoving += OnUnitMoving;
        _unitSpawner.UnitViewSpawned += OnUnitSpawned;
        _citySpawner.UnitViewSpawned += OnCitySpawned;
        _hireButton.onClick.AddListener(OnHireUnit);
        _nextTurnButton.onClick.AddListener(OnNextTurnButtonClick);
    }

    private void OnNextTurnButtonClick()
    {
        _nextTurnButton.onClick.RemoveListener(OnNextTurnButtonClick);

        _nextTurnPulse.Kill();
        _nextTurnButton.transform.localScale = Vector3.one;
    }

    private void OnUnitMoving(WalkableUnit _, IEnumerable<Vector2Int> _1, Action _2)
    {
        _newInputSorter.UnitIsMoving -= OnUnitMoving;
        _onGridPointer.Disable();
        Destroy(_onGridPointer.gameObject);

        _nextTurnButton.interactable = true;
        _nextTurnPulse = _nextTurnButton.transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnUnitSelected(Vector2Int _, IEnumerable<IEnumerable<Vector2Int>> _1, IEnumerable<Vector2Int> _2,
        IEnumerable<Vector2Int> _3, IEnumerable<Vector2Int> _4)
    {
        _newInputSorter.MovableUnitSelected -= OnUnitSelected;
        Destroy(_pointer);
        _onGridPointer.Enable();
    }

    private void OnUnitSpawned(UnitView view)
    {
        _unitSpawner.UnitViewSpawned -= OnUnitSpawned;
        _pointer = Instantiate(_canvasPrefab, view.transform);
    }

    private void OnHireUnit()
    {
        _hireButton.onClick.RemoveListener(OnHireUnit);
        _cityMenu.Disable();
        _newInputSorter.Deselect();
        HideHirePointer();
    }

    private void OnCitySelected(Vector2Int _)
    {
        _newInputSorter.FriendlyCitySelected -= OnCitySelected;
        Destroy(_pointer);
        _nextTurnButton.interactable = false;
    }

    private void OnCitySpawned(UnitView view)
    {
        _citySpawner.UnitViewSpawned -= OnCitySpawned;
        _pointer = Instantiate(_canvasPrefab, view.transform);
    }

    private void HideHirePointer()
    {
        Destroy(_hirePointer.gameObject);
    }
}
