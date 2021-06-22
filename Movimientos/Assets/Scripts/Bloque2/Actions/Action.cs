using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action
{
    public int priority;
    public int expiryTime;
    // The unit that performs the action
    public AgentUnit unit;

    public abstract bool CanInterrupt();
    public abstract bool CanDoBoth(Action other);
    public abstract bool IsComplete();
    public abstract void Execute();
}