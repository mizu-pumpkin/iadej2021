using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetPursuit : Arrive
{
    /*
       █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
       █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
    */
    
    protected Agent targetLeader;
    public Vector3 offset;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
    */

    public override void Awake() {
        base.Awake();
        targetLeader = target;
        target = new GameObject("OffsetPursuitTarget").AddComponent<Agent>();
    }

    void OnDestroy ()
    {
        if (targetLeader != null)
            Destroy(targetLeader);
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        target.position = targetLeader.position + offset;
 
        float distance = (target.position - agent.position).magnitude;

        float lookAhead = (distance / agent.maxSpeed);

        target.position = target.position + (lookAhead * targetLeader.velocity);

        // Delegate to arrive
        return base.GetSteering(agent);
    }

}