using UnityEngine;

public class GridRaycaster : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private Plane _plane;

    private void Awake()
    {
        _plane = new Plane(Vector3.up, 0);
    }

    public bool TryGetPointerPosition(out Vector3 worldPosition)
    {
        //Create a ray from the Mouse click position
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

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
}
