using Lean.Touch;
using System;
using System.Linq;
using UnityEngine;

public class TouchInput : MonoBehaviour, ITouchInputReceiver
{
    [SerializeField] private LeanFingerFilter Use = new LeanFingerFilter(true);
    [SerializeField] private float _sensitivityPC = 0.1f;
    [SerializeField] private float _sensitivityMobile = 0.2f;
    [SerializeField] private float _clampValue = 50f;

    private float _sensitivity;

    public event Action<Vector3> TouchInputReceived;

    private void Start()
    {
        bool isMobile = Application.isMobilePlatform &&
            (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android);

        if (isMobile == false)
        {
            _sensitivity = _sensitivityPC;
            _clampValue = 3f;
        }
        else
        {
            _sensitivity = _sensitivityMobile;
        }
    }

    private void FixedUpdate()
    {
        //anti pinch system
        if (LeanTouch.Fingers.Count != 2)
            return;

        // Get the fingers we want to use
        var fingers = Use.UpdateAndGetFingers(true);

        if (fingers.Count == 0)
            return;

        var finger = fingers.First();
        Vector2 delta = finger.ScreenPosition - finger.LastScreenPosition;

        if (delta.magnitude <= _clampValue)
            return;

        Vector3 input = new Vector3(delta.x, 0, delta.y).normalized * _sensitivity;
        TouchInputReceived?.Invoke(input);
    }
}

public interface ITouchInputReceiver
{
    public event Action<Vector3> TouchInputReceived;
}