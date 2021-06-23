using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action
{
    public int priority;
    public int expiryTime;
    
    protected AgentUnit unit; // The unit that performs the action
    protected bool isComplete; // If the action is complete

    public Action(AgentUnit unit)
    {
        this.unit = unit;
        this.isComplete = false;
    }

    public abstract void Execute();
    public virtual bool IsComplete() => isComplete;

    public virtual bool CanInterrupt() => false;
    public virtual bool CanDoBoth(Action other) => false;
}