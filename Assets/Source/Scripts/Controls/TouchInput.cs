using Lean.Touch;
using System;
using System.Linq;
using UnityEngine;

public class TouchInput : MonoBehaviour, ITouchInputReceiver
{
    private readonly KeyboardCameraMovement _keyboardCameraMovement = new KeyboardCameraMovement();

    [SerializeField] private LeanFingerFilter Use = new LeanFingerFilter(true);
    [SerializeField] private float _keyboardSensitivity = 0.1f;
    [SerializeField] private float _sensitivityPC = 0.1f;
    [SerializeField] private float _sensitivityMobile = 0.2f;
    [SerializeField] private float _clampValue = 50f;

    private float _sensitivity;
    private bool _isSwiping;
    private bool _isMobile;

    public event Action<Vector3> TouchInputReceived;
    public event Action TouchInputStopped;

    private void Start()
    {
        _isMobile = Application.isMobilePlatform &&
            (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android);

        if (_isMobile == false)
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
        if (_isMobile == false)
        {
            Vector3 keyboardInput = _keyboardCameraMovement.GetInput();

            if (keyboardInput != Vector3.zero)
                TouchInputReceived?.Invoke(keyboardInput * _keyboardSensitivity);
        }

        //anti pinch system
        if (LeanTouch.Fingers.Count != 2)
            return;

        // Get the fingers we want to use
        var fingers = Use.UpdateAndGetFingers(true);

        if (fingers.Count == 0 && _isSwiping)
        {
            _isSwiping = false;
            TouchInputStopped?.Invoke();
            return;
        }
        else if (fingers.Count == 0)
        {
            return;
        }

        var finger = fingers.First();
        Vector2 delta = finger.ScreenPosition - finger.LastScreenPosition;

        if (delta.magnitude <= _clampValue)
            return;

        Vector3 input = new Vector3(delta.x, 0, delta.y).normalized * _sensitivity;
        TouchInputReceived?.Invoke(input);
        _isSwiping = true;
    }
}

public class KeyboardCameraMovement
{
    private const string Horizontal = nameof(Horizontal);
    private const string Vertical = nameof(Vertical);

    public Vector3 GetInput()
    {
        return new Vector3(-Input.GetAxis(Horizontal), 0, -Input.GetAxis(Vertical));
    }
}

public interface ITouchInputReceiver
{
    public event Action<Vector3> TouchInputReceived;

    public event Action TouchInputStopped;
}