using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCombination : Action
{
    // Holds the sub-actions
    public List<Action> actions;

    public override bool CanInterrupt()
    {
        // We can interrupt if any of our sub-actions can
        foreach (Action action in actions)
            if (action.CanInterrupt())
                return true;
        return false;
    }

    public override bool CanDoBoth(Action other)
    {
        // We can do both if all of our sub-actions can
        foreach (Action action in actions)
            if (!action.CanDoBoth(other))
                return false;
        return true;
    }

    public override bool IsComplete()
    {
        // We are complete if all of our sub-actions are
        foreach (Action action in actions)
            if (!action.IsComplete())
                return false;
        return true;
    }

    public override void Execute()
    {
        // Execute all of our sub-actions
        foreach (Action action in actions)
            action.Execute();
    }

}