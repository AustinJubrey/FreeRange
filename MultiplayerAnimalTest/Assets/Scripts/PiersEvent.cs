using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// "If you wish to use a generic UnityEvent type you must override the class type."
public class UnityEvent1<T0> : UnityEvent<T0>
{
}

public class UnityEvent2<T0, T1> : UnityEvent<T0, T1>
{
}

public class PiersEvent : Singleton<PiersEvent>
{
    private Dictionary<PiersEventKey.EventKey, UnityEventBase> events = new Dictionary<PiersEventKey.EventKey, UnityEventBase>();

    // guarantee this will be always a singleton only
    private PiersEvent() { }

    #region Listen
    // No args
    public static void Listen(PiersEventKey.EventKey eventName, UnityAction listener)
    {
        UnityEventBase thisEvent = null;
        if (Instance().events.TryGetValue(eventName, out thisEvent))
        {
            UnityEvent castEvent = thisEvent as UnityEvent;

            // if typecast fail, there is a signature mismatch somewhere.
            if (castEvent != null)
            {
                castEvent.AddListener(listener);
            }
            else
            {
                Debug.LogError("Listen : Signature mismatch for PiersEvent." + eventName.ToString());
            }
        }
        else
        {
            UnityEvent castEvent = new UnityEvent();
            castEvent.AddListener(listener);
            Instance().events.Add(eventName, castEvent);
        }
    }

    // Single generic arg
    public static void Listen<T0>(PiersEventKey.EventKey eventName, UnityAction<T0> listener)
    {
        UnityEventBase thisEvent = null;
        if (Instance().events.TryGetValue(eventName, out thisEvent))
        {
            UnityEvent<T0> castEvent = thisEvent as UnityEvent<T0>;

            // if typecast fail, there is a signature mismatch somewhere.
            if (castEvent != null)
            {
                castEvent.AddListener(listener);
            }
            else
            {
                Debug.LogError("Listen : Signature mismatch for PiersEvent." + eventName.ToString());
            }
        }
        else
        {
            UnityEvent<T0> castEvent = new UnityEvent1<T0>();
            castEvent.AddListener(listener);
            Instance().events.Add(eventName, castEvent);
        }
    }

    // 2 generic args
    public static void Listen<T0, T1>(PiersEventKey.EventKey eventName, UnityAction<T0, T1> listener)
    {
        UnityEventBase thisEvent = null;
        if (Instance().events.TryGetValue(eventName, out thisEvent))
        {
            UnityEvent<T0, T1> castEvent = thisEvent as UnityEvent<T0, T1>;

            // if typecast fail, there is a signature mismatch somewhere.
            if (castEvent != null)
            {
                castEvent.AddListener(listener);
            }
            else
            {
                Debug.LogError("Listen : Signature mismatch for PiersEvent." + eventName.ToString());
            }
        }
        else
        {
            UnityEvent<T0, T1> castEvent = new UnityEvent2<T0, T1>();
            castEvent.AddListener(listener);
            Instance().events.Add(eventName, castEvent);
        }
    }
    #endregion

    #region Post
    // no args
    public static void Post(PiersEventKey.EventKey eventName)
    {
        UnityEventBase thisEvent = null;
        if (Instance().events.TryGetValue(eventName, out thisEvent))
        {
            UnityEvent castEvent = thisEvent as UnityEvent;

            // If the cast fails, there's a function call arguments mismatch.
            if (castEvent != null)
            {
                castEvent.Invoke();
            }
            else
            {
                Debug.LogError("Post : Signature mismatch for PiersEvent." + eventName.ToString());
            }
        }
    }

    // single generic arg
    public static void Post<T0>(PiersEventKey.EventKey eventName, T0 arg0)
    {
        UnityEventBase thisEvent = null;
        if (Instance().events.TryGetValue(eventName, out thisEvent))
        {
            UnityEvent<T0> castEvent = thisEvent as UnityEvent<T0>;

            // If the cast fails, there's a function call arguments mismatch.
            if (castEvent != null)
            {
                castEvent.Invoke(arg0);
            }
            else
            {
                Debug.LogError("Post : Signature mismatch for PiersEvent." + eventName.ToString());
            }
        }
    }

    // 2 generic args
    public static void Post<T0, T1>(PiersEventKey.EventKey eventName, T0 arg0, T1 arg1)
    {
        UnityEventBase thisEvent = null;
        if (Instance().events.TryGetValue(eventName, out thisEvent))
        {
            UnityEvent<T0, T1> castEvent = thisEvent as UnityEvent<T0, T1>;

            // If the cast fails, there's a function call arguments mismatch.
            if (castEvent != null)
            {
                castEvent.Invoke(arg0, arg1);
            }
            else
            {
                Debug.LogError("Post : Signature mismatch for PiersEvent." + eventName.ToString());
            }
        }
    }
    #endregion

    #region Forget
    // No args
    public static void Forget(PiersEventKey.EventKey eventName, UnityAction listener)
    {
        UnityEventBase thisEvent = null;
        if (Instance().events.TryGetValue(eventName, out thisEvent))
        {
            UnityEvent castEvent = thisEvent as UnityEvent;

            // if typecast fail, there is a signature mismatch somewhere.
            if (castEvent != null)
            {
                castEvent.RemoveListener(listener);
            }
            else
            {
                Debug.LogError("Forget : Signature mismatch for PiersEvent." + eventName.ToString());
            }
        }
    }

    // Single generic arg
    public static void Forget<T0>(PiersEventKey.EventKey eventName, UnityAction<T0> listener)
    {
        UnityEventBase thisEvent = null;
        if (Instance().events.TryGetValue(eventName, out thisEvent))
        {
            UnityEvent<T0> castEvent = thisEvent as UnityEvent<T0>;

            // if typecast fail, there is a signature mismatch somewhere.
            if (castEvent != null)
            {
                castEvent.RemoveListener(listener);
            }
            else
            {
                Debug.LogError("Forget : Signature mismatch for PiersEvent." + eventName.ToString());
            }
        }
    }

    // 2 generic args
    public static void Forget<T0, T1>(PiersEventKey.EventKey eventName, UnityAction<T0, T1> listener)
    {
        UnityEventBase thisEvent = null;
        if (Instance().events.TryGetValue(eventName, out thisEvent))
        {
            UnityEvent<T0, T1> castEvent = thisEvent as UnityEvent<T0, T1>;

            // if typecast fail, there is a signature mismatch somewhere.
            if (castEvent != null)
            {
                castEvent.RemoveListener(listener);
            }
            else
            {
                Debug.LogError("Forget : Signature mismatch for PiersEvent." + eventName.ToString());
            }
        }
    }
    #endregion
}
