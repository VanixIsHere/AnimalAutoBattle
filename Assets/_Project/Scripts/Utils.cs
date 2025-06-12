using System;
using System.Collections;
using UnityEngine;

public static class Utils
{
    // This function returns a coroutine you can start from any MonoBehaviour
    public static IEnumerator Delay(float delaySeconds, Action callback)
    {
        yield return new WaitForSeconds(delaySeconds);
        callback?.Invoke();
    }

    public static void CallAfterDelay(MonoBehaviour caller, float delaySeconds, Action callback)
    {
        caller.StartCoroutine(Delay(delaySeconds, callback));
    }

    // Delay with return value
    public static IEnumerator Delay<T>(float delaySeconds, Func<T> resultFunc, Action<T> callback)
    {
        yield return new WaitForSeconds(delaySeconds);
        if (resultFunc != null && callback != null)
        {
            T result = resultFunc();
            callback(result);
        }
    }

    public static void CallAfterDelay<T>(MonoBehaviour caller, float delaySeconds, Func<T> resultFunc, Action<T> callback)
    {
        caller.StartCoroutine(Delay(delaySeconds, resultFunc, callback));
    }
}