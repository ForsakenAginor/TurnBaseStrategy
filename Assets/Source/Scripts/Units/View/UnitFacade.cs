using UnityEngine;

[RequireComponent(typeof(UnitView))]
public class UnitFacade : MonoBehaviour, IUnitFacade
{
    private UnitView _unitView;

    public Vector3 Position => transform.position;

    public UnitView UnitView => _unitView;

    private void Awake()
    {
        _unitView = GetComponent<UnitView>();
        InitializeFieldsInAwake();
    }

    protected virtual void InitializeFieldsInAwake()
    {

    }
}
