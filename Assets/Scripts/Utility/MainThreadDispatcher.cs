using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Eidetic.Unity.Utility;
using Eidetic.Utility;
using UnityEngine;

/// Originally written by Pim de Witte (pimdewitte.com) and contributors
/// Expanded and made generic by Eidetic

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher Instance;

    public static void Instantiate()
    {
        if (Instance != null) return;
        Instance = new GameObject("MainThreadDispatcher")
            .AddComponent<MainThreadDispatcher>();
    }

    private static Queue<Action> startQueue;
    private static Queue<Action> StartQueue
    {
        get
        {
            if (startQueue == null)
                startQueue = new Queue<Action>();
            return startQueue;
        }
    }
    private static Queue<Action> updateQueue;
    private static Queue<Action> UpdateQueue
    {
        get
        {
            if (updateQueue == null)
                updateQueue = new Queue<Action>();
            return updateQueue;
        }
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        lock(StartQueue)
        {
            while (StartQueue.Count > 0)
            {
                StartQueue.Dequeue().Invoke();
            }
        }
    }

    void Update()
    {
        lock(UpdateQueue)
        {
            while (UpdateQueue.Count > 0)
            {
                UpdateQueue.Dequeue().Invoke();
            }
        }
    }

    /// <summary>
    /// Locks the queue and adds the IEnumerator to the Start queue
    /// </summary>
    /// <param name="action">IEnumerator function that will be executed from the main thread.</param>
    public static void AddToStartQueue(IEnumerator action) => StartQueue.Enqueue(() => Instance.StartCoroutine(action));

    /// <summary>
    /// Locks the queue and adds the IEnumerator to the Start queue
    /// </summary>
    /// <param name="action">IEnumerator function that will be executed from the main thread.</param>
    public static void AddToStartQueue(Action action) => AddToStartQueue(ActionWrapper(action));

    /// <summary>
    /// Locks the queue and adds the IEnumerator to the Update queue
    /// </summary>
    /// <param name="action">IEnumerator function that will be executed from the main thread.</param>
    public static void AddToUpdateQueue(IEnumerator action) => UpdateQueue.Enqueue(() => Instance.StartCoroutine(action));

    /// <summary>
    /// Locks the queue and adds the Action to the Update queue
    /// </summary>
    /// <param name="action">function that will be executed from the main thread.</param>
    public static void AddToUpdateQueue(Action action) => AddToUpdateQueue(ActionWrapper(action));

    static IEnumerator ActionWrapper(Action a)
    {
        a();
        yield return null;
    }
    static IEnumerator ActionWrapper<T>(Action<T> a, T parameter)
    {
        a(parameter);
        yield return null;
    }
}

public static class Threads
{
    public static void RunOnMain(this Action action) => MainThreadDispatcher.AddToUpdateQueue(action);
    public static void RunOnMain<T>(this Action<T> action, T parameter) => MainThreadDispatcher.AddToUpdateQueue(() => action(parameter));
    public static void InvokeOnMain(this MethodBase methodBase, object obj, object[] parameters) => Threads.RunOnMain(() => methodBase.Invoke(obj, parameters));
    public static void ForEachOnMain<T>(this List<T> list, Action<T> action) =>
        list.ForEach(item => MainThreadDispatcher.AddToUpdateQueue(() => action.Invoke(item)));
    public static void ForEachOnMain<T, K>(this Dictionary<T, K> dictionary, Action<T, K> action) =>
        dictionary.ForEach((key, value) => MainThreadDispatcher.AddToUpdateQueue(() => action.Invoke(key, value)));
    public static void RunAtStart(this Action action) => MainThreadDispatcher.AddToStartQueue(action);
    public static void RunAtStart<T>(this Action<T> action, T parameter) => MainThreadDispatcher.AddToStartQueue(() => action.Invoke(parameter));
}