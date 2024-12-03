using Lean.Touch;
using System;
using UnityEngine;

public class PinchDetector : MonoBehaviour
{
    public event Action<float> GotPinchInput;

    private void FixedUpdate()
    {
        var result = LeanGesture.GetPinchScale() - 1;

        //some magic
        if (result == -1)
            return;

        if (Mathf.Approximately(result, 0) == false)
            GotPinchInput?.Invoke(result);
    }
}
