using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : SteeringBehaviour
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    // Holds a list of potential targets
    GameObject[] targets;

    // Holds the threshold to take action
    public float threshold;

    // Holds the constant coefficient of decay for the 
    // inverse square law force
    //decayCoefficient

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public override void Awake() {
        base.Awake();
        //target = new GameObject().AddComponent<Agent>();
        targets = GameObject.FindGameObjectsWithTag("Agent");
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        int count = 0;
        Vector3 heading = new Vector3(); // 2D
        //float heading = 0;
        Vector3 direction;
        float distance;

        foreach (GameObject target in targets)
        {
            Agent targetAgent = target.GetComponent<Agent>();

            direction = targetAgent.position - agent.position;
            distance = Mathf.Abs(direction.magnitude); // abs(direction)

            if (distance > threshold)
                continue;
            
            heading += Heading(targetAgent); // targetAgent.Heading();
            count++;
        }

        if (count > 0) {
            heading /= count;
            heading -= Heading(agent); // agent.Heading();
        }
        else return null;

        Steering steer = new Steering();
        steer.angular = Utils.PositionToAngle(heading);
        //target.orientation = heading;

        return steer;
    }

    Vector3 Heading(Agent agent) {
        return agent.OrientationToVector();
    }

}
