using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The event manager that holds customizable event dictionaries 
/// and manages the communication of invokers and listeners.
/// </summary>
public static class EventManager
{
    // double float argument event invokers and listeners
    static Dictionary<EventType, List<Invoker>> doubleFloatEventInvokers = 
        new Dictionary<EventType, List<Invoker>>();
    static Dictionary<EventType, List<UnityAction<float, float>>> doubleFloatEventListeners = 
        new Dictionary<EventType, List<UnityAction<float, float>>>();

    // No argument event invokers and listeners
    static Dictionary<EventType, List<Invoker>> noArgEventInvokers =
    new Dictionary<EventType, List<Invoker>>();
    static Dictionary<EventType, List<UnityAction>> noArgEventListeners =
        new Dictionary<EventType, List<UnityAction>>();


    public static void Initialize()
    {
        // create empty lists for all the dictionary entries
        foreach (EventType name in Enum.GetValues(typeof(EventType)))
        {
            if (!doubleFloatEventInvokers.ContainsKey(name))
            {
                doubleFloatEventInvokers.Add(name, new List<Invoker>());
                doubleFloatEventListeners.Add(name, new List<UnityAction<float, float>>());
            }
            else
            {
                doubleFloatEventInvokers[name].Clear();
                doubleFloatEventListeners[name].Clear();
            }

            if (!noArgEventInvokers.ContainsKey(name))
            {
                noArgEventInvokers.Add(name, new List<Invoker>());
                noArgEventListeners.Add(name, new List<UnityAction>());
            }
            else
            {
                noArgEventInvokers[name].Clear();
                noArgEventListeners[name].Clear();
            }
        }
    }

    public static void DisplayListeners(EventType eventType)
    {
        foreach (var item in noArgEventListeners[eventType])
        {
            Debug.Log(eventType);
            Debug.Log(item.Method);
        }
    }

    /// <summary>
    /// Adds the invoker for the given float argument event type
    /// </summary>
    /// <param name="invoker">invoker</param>
    public static void AddDoubleFloatArgumentInvoker(Invoker invoker, EventType eventType)
    {
        InitializeFloatEventDict(eventType);

        foreach (UnityAction<float, float> listener in doubleFloatEventListeners[eventType])
        {
            invoker.AddDoubleFloatArgListener(listener, eventType);
        }
        doubleFloatEventInvokers[eventType].Add(invoker);
    }

    /// <summary>
    /// Adds the listener for the given float argument event type
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddDoubleFloatArgumentListener(UnityAction<float, float> listener, EventType eventType)
    {
        InitializeFloatEventDict(eventType);

        foreach (Invoker invoker in doubleFloatEventInvokers[eventType])
        {
            invoker.AddDoubleFloatArgListener(listener, eventType);
        }
        doubleFloatEventListeners[eventType].Add(listener);
    }


    private static void InitializeFloatEventDict(EventType eventType)
    {
        if (!doubleFloatEventListeners.ContainsKey(eventType))
        {
            doubleFloatEventListeners.Add(eventType, new List<UnityAction<float, float>>());
        }

        if (!doubleFloatEventInvokers.ContainsKey(eventType))
        {
            doubleFloatEventInvokers.Add(eventType, new List<Invoker>());
        }
    }

    /// <summary>
    /// Adds the invoker for the given no argument event type
    /// </summary>
    /// <param name="invoker">invoker</param>
    public static void AddNoArgumentInvoker(Invoker invoker, EventType eventType)
    {
        InitializeNoArgEventDict(eventType);

        foreach (UnityAction listener in noArgEventListeners[eventType])
        {
            invoker.AddNoArgumentListener(listener, eventType);
        }
        noArgEventInvokers[eventType].Add(invoker);
    }

    /// <summary>
    /// Adds the listener for the given no argument event
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddNoArgumentListener(UnityAction listener, EventType eventType)
    {
        InitializeNoArgEventDict(eventType);

        foreach (Invoker invoker in noArgEventInvokers[eventType])
        {
            invoker.AddNoArgumentListener(listener, eventType);
        }

        noArgEventListeners[eventType].Add(listener);
    }

    /// <summary>
    /// Adds the event if it is not already in the respective dictionary.
    /// </summary>
    /// <param name="eventType"></param>
    private static void InitializeNoArgEventDict(EventType eventType)
    {
        if (!noArgEventListeners.ContainsKey(eventType))
        {
            noArgEventListeners.Add(eventType, new List<UnityAction>());
        }

        if (!noArgEventInvokers.ContainsKey(eventType))
        {
            noArgEventInvokers.Add(eventType, new List<Invoker>());
        }
    }
}
