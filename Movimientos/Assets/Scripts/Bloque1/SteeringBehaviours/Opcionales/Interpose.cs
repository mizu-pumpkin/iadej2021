using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Interpose produces a force that moves a vehicle to the midpoint
    of the imaginary line connecting two other agents
*/
public class Interpose : Arrive
{
    public Agent pointA;
    public Agent pointB;

    public override void Awake()
    {
        base.Awake();
        target = new GameObject("InterposeTarget").AddComponent<Agent>();
    }

    public override Steering GetSteering(AgentNPC agent)
    {
        // Find the middle point
        target.position = (pointA.position + pointB.position) / 2;

        return base.GetSteering(agent);
    }
}