using UnityEngine;

public interface ICameraConfigurationGetter
{
    public Vector2 GetMinimumCameraPosition(GameLevel level);

    public Vector2 GetMaximumCameraPosition(GameLevel level);

    public Vector3 GetCameraStartPosition(GameLevel level);
}
