using UnityEngine;

public class GridRaycaster : MonoBehaviour, ICameraFocusGetter
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _planeHeight;

    private Plane _plane;

    private void Awake()
    {
        _plane = new Plane(Vector3.up, _planeHeight);
    }

    public bool TryGetPointerPosition(Vector2 screenPosition, out Vector3 worldPosition)
    {
        //Create a ray from the Mouse click position
        Ray ray = _camera.ScreenPointToRay(screenPosition);

        if (_plane.Raycast(ray, out float enter))
        {
            //Get the point that is clicked
            worldPosition = ray.GetPoint(enter);
            return true;
        }
        else
        {
            worldPosition = Vector3.zero;
            return false;
        }
    }

    public Vector3 GetCameraFocus()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        Vector3 worldPosition = Vector3.zero;

        if (_plane.Raycast(ray, out float enter))
            worldPosition = ray.GetPoint(enter);

        return worldPosition;
    }
}

public interface ICameraFocusGetter
{
    public Vector3 GetCameraFocus();
}
