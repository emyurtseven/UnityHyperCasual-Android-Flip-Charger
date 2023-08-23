using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An event invoker
/// </summary>
public class Invoker : MonoBehaviour
{
    protected Dictionary<EventType, NoArgumentEvent> noArgEventDict =
        new Dictionary<EventType, NoArgumentEvent>();

    protected Dictionary<EventType, DoubleFloatArgumentEvent> doubleFloatArgEventDict = 
        new Dictionary<EventType, DoubleFloatArgumentEvent>();

    /// <summary>
    /// Adds the given listener to the no argument event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddNoArgumentListener(UnityAction listener, EventType eventType)
    {
        noArgEventDict[eventType].AddListener(listener);
    }

    public void InvokeNoArgumentEvent(EventType eventType)
    {
        noArgEventDict[eventType].Invoke();
    }

    /// <summary>
    /// Adds the given listener to the one argument event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddDoubleFloatArgListener(UnityAction<float, float> listener, EventType eventType)
    {
        doubleFloatArgEventDict[eventType].AddListener(listener);
    }

    /// <summary>
    /// Removes the given listener to the one argument event
    /// </summary>
    /// <param name="listener">listener</param>
    public void RemoveOneArgumentListener(UnityAction<float, float> listener, EventType eventType)
    {
        doubleFloatArgEventDict[eventType].RemoveListener(listener);
    }

    public void InvokeDoubleFloatArgEvent(EventType eventType, float argument1, float argument2)
    {
        doubleFloatArgEventDict[eventType].Invoke(argument1, argument2);
    }
}
