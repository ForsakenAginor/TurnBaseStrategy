using UnityEngine;

public interface ICityNamesGetter
{
    public SerializedPair<Vector2Int, string>[] GetCitiesNames(GameLevel level);
}