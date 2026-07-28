using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class TimerRequest
{
    public readonly float triggerTime;
    public readonly Action callback;
    public readonly bool retainAcrossScenes;
    public TimerRequest(float delay, Action callback, bool retainAcrossScenes = false)
    {
        this.triggerTime = Time.time + delay;
        this.callback = callback;
        this.retainAcrossScenes = retainAcrossScenes;
    }
}

public class GlobalTimer : Singleton<GlobalTimer>
{
    private readonly MinHeap<TimerRequest> timers = new(Comparer<TimerRequest>.Create(
        (a, b) => a.triggerTime.CompareTo(b.triggerTime)));

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

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
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        List<TimerRequest> req =  new List<TimerRequest>();
        while (timers.Count > 0)
        {
            if (timers.Peek().retainAcrossScenes)
            {
                req.Add(timers.Peek());
            }
            else
            {
                timers.Pop();
            }
        }

        foreach (TimerRequest r in req)
        {
            timers.Push(r);
        }
    }
    
}