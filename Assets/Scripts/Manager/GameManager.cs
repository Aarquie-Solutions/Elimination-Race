using System;
using AarquieSolutions.Base.Singleton;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action OnPaused;
    public event Action OnResumed;
    public event Action OnSceneOver;

    private bool isSceneOver = false;

    public void Pause()
    {
        OnPaused?.Invoke();
        Time.timeScale = 0.0f;
    }

    public void Resume()
    {
        OnResumed?.Invoke();
        Time.timeScale = 1.0f;
    }

    public void SceneOver()
    {
        OnSceneOver?.Invoke();
        isSceneOver = true;
        Time.timeScale = 0.0f;
    }
}
