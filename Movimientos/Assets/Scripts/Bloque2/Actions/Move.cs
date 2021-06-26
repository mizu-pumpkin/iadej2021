using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Action
{
    Vector3 target;
    CombatSystem.TerrainType terrain;

    public Move(AgentUnit unit, Vector3 target, CombatSystem.TerrainType terrain = CombatSystem.TerrainType.Unknown) : base(unit)
    {
        this.target = target;
        this.terrain = terrain;
    }

    public override void Execute()
    {
        unit.canMove = true;

        // Si se sabe el tipo del terreno objetivo, se comprueba donde estamos
        if (terrain != CombatSystem.TerrainType.Unknown) {
            if (unit.terrain == terrain)
                isComplete = true;
        }
        // Si solo se sabe una posición, se mira la distancia
        else {
            float distance = (unit.position - target).magnitude;
            if (distance <= unit.exteriorRadius)
                isComplete = true;
        }

    }

    public override string ToString() => "ROUTED: ("+target.x+","+target.z+")"+terrain;
}
