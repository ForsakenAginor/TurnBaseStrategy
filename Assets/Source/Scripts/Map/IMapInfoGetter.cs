using UnityEngine;
using UnityEngine.Tilemaps;

public interface IMapInfoGetter
{
    public Vector2Int GetMapSize(GameLevel level);

    public Tilemap GetMapPrefab(GameLevel level);
}
