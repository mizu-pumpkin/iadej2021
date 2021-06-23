using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAway : Action
{
    private AgentUnit enemyUnit;

    public RunAway(AgentUnit unit, AgentUnit enemyUnit) : base(unit)
    {
        this.enemyUnit = enemyUnit;
    }

    public override void Execute()
    {
        unit.canMove = true;

        float distance = (unit.position - enemyUnit.position).magnitude;

        if (distance > enemyUnit.attackRange * 3)
            isComplete = true;
    }

    public override string ToString() => "FLEE";
}
