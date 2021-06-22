using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeCover : Action
{
    public override bool CanInterrupt()
    {
        return false;
    }

    public override bool CanDoBoth(Action other)
    {
        return false;
    }

    public override bool IsComplete()
    {
        return false;
    }

    public override void Execute()
    {
        
    }
}
