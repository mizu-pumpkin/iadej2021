using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Action
{
    private float time = -1;

    public Heal(AgentUnit unit) : base(unit) {}

    public override void Execute()
    {
        if (time == -1)
            time = Time.time;
        
        float distance = (unit.position - unit.healingZonePosition).magnitude;

        // If it's in range, heal
        // TODO: if (unit.terrain == TerrainType.heal)
        if (distance <= unit.interiorRadius*2)
        {
            unit.canMove = false;
            if (Time.time - time >= unit.healSpeed) {
                time = -1;
                bool healed = unit.TakeHeal(unit.healPower);
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
