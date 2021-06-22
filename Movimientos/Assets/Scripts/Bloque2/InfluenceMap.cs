using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMap : MonoBehaviour
{

    public float InfluenceOverLocation(List<AgentUnit> team, Vector3 location)
    {
        float influence = 0;
        foreach(AgentUnit unit in team) {
            float distance = (location - unit.position).magnitude;
            // FIXME: elige uno de estos dos
            // rapid initial drop off, but with a longer range of influence
            influence += unit.influence / Mathf.Sqrt(1 + distance);
            // plateaus first before rapidly tailing off at a distance
            //influence += unit.influence / Mathf.Pow(1 + distance);
        }
        return influence;
    }

    // Si gana teamA, devuelve un número positivo
    // Si gana teamB, devuelve un número negativo
    public float ControlOverLocation(List<AgentUnit> teamA, List<AgentUnit> teamB, Vector3 location)
    {
        float influenceA = InfluenceOverLocation(teamA, location);
        float influenceB = InfluenceOverLocation(teamB, location);

        return influenceA - influenceB;
    }
}
