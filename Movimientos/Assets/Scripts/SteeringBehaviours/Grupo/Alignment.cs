using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FIXME: sigue teniendo algo raro
public class Alignment : Align
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    // Holds a list of potential targets
    GameObject[] targets;

    // Holds the threshold to take action
    public float threshold;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public override void Awake() {
        base.Awake();
        target = new GameObject().AddComponent<Agent>();
        targets = GameObject.FindGameObjectsWithTag("Agent");
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        int count = 0;
        //Vector3 heading = new Vector3(); // 2D
        float heading = 0;
        Vector3 direction;
        float distance;

        foreach (GameObject target in targets)
        {
            Agent targetAgent = target.GetComponent<Agent>();

            direction = targetAgent.position - agent.position;
            distance = direction.magnitude; // abs(direction)

            if (distance > threshold)
                continue;
            
            //heading += Utils.OrientationToVector(targetAgent.orientation).normalized;
            heading += targetAgent.orientation;
            count++;
        }

        if (count > 0) {
            heading /= count;
            //heading -= Utils.OrientationToVector(agent.orientation).normalized;
            //heading -= agent.orientation;
        }

        //target.orientation = Utils.PositionToAngle(heading);
        target.orientation = heading;
        return base.GetSteering(agent);
    }

}
