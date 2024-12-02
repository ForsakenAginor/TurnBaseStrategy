using Assets.Scripts.HexGrid;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GridHighlightConfiguration")]
public class GridColorConfiguration : UpdatableConfiguration<CellColor, CellSprite>
{
    public CellSprite GetBlockedCellColor() => Content.First(o => o.Key == CellColor.Blocked).Value;

    public CellSprite GetEnemyCellColor() => Content.First(o => o.Key == CellColor.Enemy).Value;

    public CellSprite GetAllyCellColor() => Content.First(o => o.Key == CellColor.Ally).Value;

    public CellSprite GetSelectedCellColor() => Content.First(o => o.Key == CellColor.Selected).Value;

    public CellSprite GetAvailableCellColor() => Content.First(o => o.Key == CellColor.Available).Value;
}
