using Lean.Touch;
using System;
using UnityEngine;

public class SwipeHandler
{
    private readonly LeanFingerSwipe _swipe;

    public SwipeHandler(LeanFingerSwipe swipe)
    {
        _swipe = swipe != null ? swipe : throw new ArgumentNullException(nameof(swipe));
        _swipe.onWorldDelta.AddListener(OnSwipe);
    }

    ~SwipeHandler()
    {
        _swipe.onWorldDelta.RemoveListener(OnSwipe);        
    }

    public event Action<Vector3> SwipeInputReceived;

    private void OnSwipe(Vector3 delta)
    {
        Vector3 swipeDelta = new Vector3(delta.x, 0, delta.z);
        SwipeInputReceived?.Invoke(swipeDelta);
    }
}
