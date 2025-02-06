using Lean.Touch;
using System;
using System.Linq;
using UnityEngine;

public class PinchDetector : MonoBehaviour, IZoomInputReceiver
{
    private IZoomInput _zoomInput;

    public event Action<float> GotPinchInput;

    private void FixedUpdate()
    {
        if (_zoomInput == null)
            return;

        float result = 0;

        if (_zoomInput.CheckZoomInput(ref result))
            GotPinchInput?.Invoke(result);
    }

    public void Init(IZoomInput zoomInput)
    {
        _zoomInput = zoomInput != null ? zoomInput : throw new ArgumentNullException(nameof(zoomInput));
    }
}

public interface IZoomInputReceiver
{
    public event Action<float> GotPinchInput;
}

public interface IZoomInput
{
    public bool CheckZoomInput(ref float result);
}

public class PCInput : IZoomInput
{
    public bool CheckZoomInput(ref float result)
    {
        Vector2 input = Input.mouseScrollDelta;

        if (input == Vector2.zero)
            return false;

        result = input.y;
        return true;
    }
}

public class MobileInput : IZoomInput
{
    public bool CheckZoomInput(ref float result)
    {
        if (LeanTouch.Fingers.Count != 3)
            return false;

        var fingers = LeanTouch.Fingers.Skip(1).ToList();
        result = LeanGesture.GetPinchScale(fingers);
        float noInput = 0;

        if (result == noInput)
            return false;

        // results will be in range [-1; 1]
        result += -1;

        //deleting wrong results when u free fingers
        float clampValue = 0.1f;

        if (MathF.Abs(result) < clampValue)
            return false;

        return true;
    }
}

