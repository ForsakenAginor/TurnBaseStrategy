using Lean.Touch;
using System;
using System.Linq;
using UnityEngine;

public class IDKWhatImDoing : MonoBehaviour, ITouchInputReceiver
{
    [SerializeField] private LeanFingerFilter Use = new LeanFingerFilter(true);

    public event Action<Vector2> TouchInputReceived;

    private void FixedUpdate()
    {
        // Get the fingers we want to use
        var fingers = Use.UpdateAndGetFingers(true);


        if (fingers.Count == 0)
            return;

        var finger = fingers.First();
        Vector2 delta = finger.ScreenPosition - finger.LastScreenPosition;
        TouchInputReceived?.Invoke(delta);
    }
}

public interface ITouchInputReceiver
{
    public event Action<Vector2> TouchInputReceived;
}