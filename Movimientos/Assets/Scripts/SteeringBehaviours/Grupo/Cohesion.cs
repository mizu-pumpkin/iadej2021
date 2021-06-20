using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : Seek
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    //Holds a list of potential targets
    GameObject[] targets;

    //Holds the threshold to take action
    public float threshold;
    
    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public override void Awake() {
        base.Awake();
        this.target = new GameObject().AddComponent<Agent>();
        targets = GameObject.FindGameObjectsWithTag("Agent");
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        int count = 0;

        Vector3 centerOfMass = new Vector3();
        Vector3 direction;
        float distance;

        foreach (GameObject target in targets)
        {
            Agent targetAgent = target.GetComponent<Agent>();
            
            direction = targetAgent.position - agent.position;
            distance = direction.magnitude; // abs(direction)

            if (distance > threshold)
                continue;
            
            centerOfMass += targetAgent.position;
            count++;
        }

        if (count == 0)
            return null;
        
        centerOfMass /= count;

        this.target.position = centerOfMass;
        
        return base.GetSteering(agent);
    }

}
