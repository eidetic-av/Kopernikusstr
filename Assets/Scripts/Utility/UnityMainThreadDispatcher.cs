using OscJack;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// Author: Pim de Witte (pimdewitte.com) and contributors
/// Extended by eidetic
/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour
{

    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    public void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    /// <summary>
    /// Locks the queue and adds the IEnumerator to the queue
    /// </summary>
    /// <param name="action">IEnumerator function that will be executed from the main thread.</param>
    public void Enqueue(IEnumerator action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => {
                StartCoroutine(action);
            });
        }
    }

    /// <summary>
    /// Locks the queue and adds the Action to the queue
    /// </summary>
    /// <param name="action">function that will be executed from the main thread.</param>
    public void Enqueue(Action action)
    {
        Enqueue(ActionWrapper(action));
    }
    IEnumerator ActionWrapper(Action a)
    {
        a();
        yield return null;
    }
    public void Enqueue(Action<Int32> action, Int32 parameter)
    {
        Enqueue(ActionWrapper(action, parameter));
    }
    IEnumerator ActionWrapper(Action<Int32> a, Int32 parameter)
    {
        a(parameter);
        yield return null;
    }
    public void Enqueue(Action<Single> action, Single parameter)
    {
        Enqueue(ActionWrapper(action, parameter));
    }
    IEnumerator ActionWrapper(Action<Single> a, Single parameter)
    {
        a(parameter);
        yield return null;
    }
    public void Enqueue(Action<bool> action, bool parameter)
    {
        Enqueue(ActionWrapper(action, parameter));
    }
    IEnumerator ActionWrapper(Action<bool> a, bool parameter)
    {
        a(parameter);
        yield return null;
    }
    public void Enqueue(Action<string> action, string parameter)
    {
        Enqueue(ActionWrapper(action, parameter));
    }
    IEnumerator ActionWrapper(Action<string> a, string parameter)
    {
        a(parameter);
        yield return null;
    }
    public void Enqueue(Action<Vector2> action, Vector2 parameter)
    {
        Enqueue(ActionWrapper(action, parameter));
    }
    IEnumerator ActionWrapper(Action<Vector2> a, Vector2 parameter)
    {
        a(parameter);
        yield return null;
    }
    public void Enqueue(Action<Vector3> action, Vector3 parameter)
    {
        Enqueue(ActionWrapper(action, parameter));
    }
    IEnumerator ActionWrapper(Action<Vector3> a, Vector3 parameter)
    {
        a(parameter);
        yield return null;
    }


    private static UnityMainThreadDispatcher _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static UnityMainThreadDispatcher Instance()
    {
        if (!Exists())
        {
            throw new Exception("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
        }
        return _instance;
    }


    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }


}