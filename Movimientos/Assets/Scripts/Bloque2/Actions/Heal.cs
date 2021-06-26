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
        if (unit.terrain == CombatSystem.TerrainType.Heal)
        {
            unit.canMove = false;
            if (Time.time - time >= unit.healSpeed) {
                time = -1;
                bool healed = unit.TakeHeal(unit.healPower);
                if (healed) isComplete = true;
                unit.color = unit.colorHeal;
            }
            else unit.color = unit.colorOriginal;
        }
        // If it's not in range, move closer
        else {
            unit.canMove = true;
            unit.color = unit.colorOriginal;
        }
    }

    public override string ToString() => "HEAL";
}
