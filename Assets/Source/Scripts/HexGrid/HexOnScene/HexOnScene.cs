using Assets.Scripts.HexGrid;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HexOnScene : MonoBehaviour, IHexOnScene
{
    [SerializeField] private bool _isBlocked;

    private SwitchableElement _hexGoodContent;
    private SwitchableElement _hexEvilContent;
    private SwitchableElement _hexGood;
    private SwitchableElement _hexEvil;

    public bool IsBlocked => _isBlocked;

    public void HideContent()
    {
        _hexEvilContent.Disable();
        _hexGoodContent.Disable();
    }

    public void ShowContent()
    {
        _hexEvilContent.Enable();
        _hexGoodContent.Enable();
    }

    public void SwtichToEvil()
    {
        _hexGood.Disable();
        _hexEvil.Enable();
    }

    public void SwtichToGood()
    {
        _hexGood.Enable();
        _hexEvil.Disable();
    }

    private void Awake()
    {
        _hexGood = GetComponentInChildren<GoodHex>();
        _hexEvil = GetComponentInChildren<EvilHex>();
        _hexGoodContent = _hexGood.gameObject.GetComponentInChildren<GoodContent>();
        _hexEvilContent = _hexEvil.gameObject.GetComponentInChildren<EvilContent>();
        _hexEvil.Disable();
    }
}

public interface IHexOnScene
{
    public bool IsBlocked { get; }

    public void ShowContent();

    public void HideContent();

    public void SwtichToGood();

    public void SwtichToEvil();
}

public class HexContentSwitcher
{
    private readonly HexGridXZ<Unit> _unitsGrid;
    private readonly HexGridXZ<IHexOnScene> _contentGrid;

    public HexContentSwitcher(HexGridXZ<Unit> unitsGrid, HexGridXZ<IHexOnScene> contentGrid)
    {
        _unitsGrid = unitsGrid != null ? unitsGrid : throw new ArgumentNullException(nameof(unitsGrid));
        _contentGrid = contentGrid != null ? contentGrid : throw new ArgumentNullException(nameof(contentGrid));

        _unitsGrid.GridObjectChanged += OnGridChanged;
    }

    ~HexContentSwitcher()
    {
        _unitsGrid.GridObjectChanged -= OnGridChanged;
    }

    private void OnGridChanged(Vector2Int cell)
    {
        var unit = _unitsGrid.GetGridObject(cell);

        if (unit == null)
        {
            _contentGrid.GetGridObject(cell).ShowContent();
            return;
        }

        _contentGrid.GetGridObject(cell).HideContent();

        if (unit is CityUnit city)
        {
            if (city.Side == Side.Enemy)
            {
                foreach (var convertedCell in GetConvertedCells(cell))
                    _contentGrid.GetGridObject(convertedCell).SwtichToGood();
            }
            else
            {
                foreach (var convertedCell in GetConvertedCells(cell))
                    _contentGrid.GetGridObject(convertedCell).SwtichToEvil();
            }
        }
    }

    private IEnumerable<Vector2Int> GetConvertedCells(Vector2Int cell)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        int radius = 4;
        Vector2Int newCell;

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                newCell = cell + new Vector2Int(i, j);

                if (_contentGrid.IsValidGridPosition(newCell) && Vector2Int.Distance(newCell, cell) <= radius)
                    result.Add(newCell);
            }
        }

        return result;
    }
}