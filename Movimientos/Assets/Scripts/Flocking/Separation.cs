using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : SteeringBehaviour
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    

    //Holds a list of potential targets
    GameObject[] targets;

    //Holds the threshold to take action
    public float threshold;

    //Holds the constant coefficient of decay for the 
    //inverse square law force
    public float decayCoefficient;

    //Holds the maximum acceleration of the character
    //maxAcceleration


    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public override void Awake() {
        base.Awake();
        targets = GameObject.FindGameObjectsWithTag("Agent");
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        Vector3 direction;
        float distance;
        float strength;

        // The steering variable holds the output
        Steering steer = new Steering();

        // Loop through each target
        foreach (GameObject target in targets)
        {
            Agent targetAgent = target.GetComponent<Agent>();

            // Check if the target is close
            direction = targetAgent.position - agent.position;
            distance = direction.magnitude;

            if (distance < threshold)
            {
                // Calculate the strength of repulsion
                strength = Mathf.Min(decayCoefficient / (distance * distance), agent.maxAcceleration);
                strength *= -1; // ???: attraction (-1)^*stren
                
                // Add the acceleration
                direction.Normalize();
                steer.linear += strength * direction;
            }
        }

        // We've gone through all targets, return the result
        return steer;
    }

}
