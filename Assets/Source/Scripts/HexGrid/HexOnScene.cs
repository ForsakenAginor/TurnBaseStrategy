using Assets.Scripts.HexGrid;
using System;
using UnityEngine;

public class HexOnScene : MonoBehaviour, IHexOnScene
{
    [SerializeField] private bool _isBlocked;

    private UIElement _hexContent;

    public bool IsBlocked => _isBlocked;

    public void HideContent() => _hexContent.Disable();

    public void ShowContent() => _hexContent.Enable();

    private void Awake()
    {
        _hexContent = GetComponentInChildren<UIElement>();
    }
}

public interface IHexOnScene
{
    public bool IsBlocked { get;}

    public void ShowContent();

    public void HideContent();
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

        if (unit != null)
            _contentGrid.GetGridObject(cell).HideContent();
        else
            _contentGrid.GetGridObject(cell).ShowContent();
    }
}
