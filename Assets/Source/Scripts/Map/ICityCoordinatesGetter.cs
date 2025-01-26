using System.Collections.Generic;
using UnityEngine;

public interface ICityCoordinatesGetter
{
    public SerializedPair<Vector2Int, CitySize>[] GetEnemyCities(GameLevel level);

    public SerializedPair<Vector2Int, CitySize>[] GetPlayerCities(GameLevel level);

    public SerializedPair<Vector2Int, string>[] GetCitiesNames(GameLevel level);

    public Dictionary<Vector2Int, SerializedPair<Sprite, string>> GetCitiesBossInfo(GameLevel level);
}