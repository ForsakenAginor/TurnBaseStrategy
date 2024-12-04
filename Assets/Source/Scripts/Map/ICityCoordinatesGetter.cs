using UnityEngine;

public interface ICityCoordinatesGetter
{
    public SerializedPair<Vector2Int, CitySize>[] GetEnemyCities(GameLevel level);

    public SerializedPair<Vector2Int, CitySize>[] GetPlayerCities(GameLevel level);
}