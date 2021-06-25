using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Action
{
    Vector3 target;

    public Move(AgentUnit unit, Vector3 target) : base(unit)
    {
        this.target = target;
    }

    public override void Execute()
    {
        unit.canMove = true;

        float distance = (unit.position - target).magnitude;

        if (distance <= unit.exteriorRadius)
            isComplete = true;
    }

    public override string ToString() => "ROUTED";
}
