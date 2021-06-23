using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : Action
{
    public Defend(AgentUnit unit) : base(unit) {}

    public override void Execute()
    {
        if (unit.position == unit.respawnPos)
            isComplete = true;
    }

    public override string ToString() => "DEFEND";
}
