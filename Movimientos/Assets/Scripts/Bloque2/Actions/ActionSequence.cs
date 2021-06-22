using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSequence : Action
{
    // Holds the sub-actions
    public List<Action> actions;

    // Holds the index of the currently executing sub-action
    public int activeIndex = 0;

    public override bool CanInterrupt()
    {
        // We can interrupt if out first sub-action can
        return actions[0].CanInterrupt();
    }

    public override bool CanDoBoth(Action other)
    {
        // We can do both if all of our sub-actions can
        // If we only tested the first one, we'd be in danger
        // of suddenly finding ourselves incompatible
        // mid-sequence
        foreach (Action action in actions)
            if (!action.CanDoBoth(other))
                return false;
        return true;
    }

    public override bool IsComplete()
    {
        // We are complete if all of our sub-actions are
        return activeIndex >= actions.Count;
    }

    public override void Execute()
    {
        // Execute our current action
        actions[activeIndex].Execute();

        // If our current action is complete, go to the next
        if (actions[activeIndex].IsComplete())
            activeIndex++;
    }

}