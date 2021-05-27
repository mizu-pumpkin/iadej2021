using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : Align
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    protected Agent targetAux;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */
    
    public override void Awake() {
        base.Awake();
        priority = 2;
        targetAux = target;
        target = new GameObject().AddComponent<Agent>();
    }

    void OnDestroy()
    {
        Destroy(target);
    }
    
    // Implemented as it was in Pursue
    override public Steering GetSteering(AgentNPC agent)
    {
        // 1. Calculate the target to delegate to align

        // Work out the direction to target
        Vector3 direction = targetAux.position - agent.position;

        // Check for a zero direction, and make no change if so
        if (direction.magnitude == 0.0f)
            return null;
        
        // Put the target together
        target.orientation = agent.PositionToAngle(direction);

        // 2. Delegate to align
        return base.GetSteering(agent);
    }
    
}