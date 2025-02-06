using Lean.Touch;
using System;
using System.Linq;
using UnityEngine;

public class TouchInput : MonoBehaviour, ITouchInputReceiver
{
    [SerializeField] private LeanFingerFilter Use = new LeanFingerFilter(true);
    [SerializeField] private float _sensitivity = 0.25f;
    [SerializeField] private float _clampValue = 50f;

    public event Action<Vector3> TouchInputReceived;

    private void Start()
    {
        bool isMobile = Application.isMobilePlatform &&
            (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android);

        if (isMobile == false)
            _clampValue = 3f;
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