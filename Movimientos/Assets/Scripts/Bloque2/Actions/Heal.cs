using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Action
{
    public AgentUnit allyUnit;
    private float time = -1;

    public Heal(AgentUnit unit, AgentUnit allyUnit) : base(unit) {
        this.allyUnit = allyUnit;
    }

    public override void Execute()
    {
        if (time == -1)
            time = Time.time;
        
        float distance = (unit.position - allyUnit.position).magnitude;

        // If it's in range, heal
        if (distance <= unit.attackRange)
        {
            unit.canMove = false;
            if (Time.time - time >= unit.attackSpeed) {
                time = -1;
                bool healed = allyUnit.TakeHeal(unit.healStrength);
                if (healed) isComplete = true;
            }
        }
        // If it's not in range, move closer
        else {
            unit.canMove = true;
        }
    }

    public override string ToString() => "HEAL";
}
