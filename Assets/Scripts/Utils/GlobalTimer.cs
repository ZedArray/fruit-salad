using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class TimerRequest
{
    public readonly float triggerTime;
    public readonly Action callback;

    public TimerRequest(float delay, Action callback)
    {
        this.triggerTime = Time.time + delay;
        this.callback = callback;
    }
}

public class GlobalTimer : Singleton<GlobalTimer>
{
    private readonly MinHeap<TimerRequest> timers = new(Comparer<TimerRequest>.Create(
        (a, b) => a.triggerTime.CompareTo(b.triggerTime)));

    public void AddTimer(TimerRequest request)
    {
        timers.Push(request);
    }

    void Update()
    {
        while (timers.Count > 0)
        {
            
            TimerRequest t = timers.Peek();

            if (Time.time < t.triggerTime)
                break;

            timers.Pop();
            t.callback?.Invoke();
        }
    }
}