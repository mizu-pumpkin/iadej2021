using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMatching : SteeringBehaviour
{
    // Holds the time over which to achieve target speed
    public float timeToTarget = 0.1f;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */
    
    override public Steering GetSteering(AgentNPC agent)
    {
        // Create the structure to hold our output
        Steering steer = new Steering();
        
        // Acceleration tries to get to the target rotation
        steer.linear = target.velocity - agent.velocity;
        steer.linear /= timeToTarget;

        // Check if the acceleration is too fast
        if (steer.linear.magnitude > agent.maxAcceleration) {
            steer.linear.Normalize();
            steer.linear *= agent.maxAcceleration;
        }

        // Output the steering
        //steering.angular = 0;
        return steer;
    }
}
