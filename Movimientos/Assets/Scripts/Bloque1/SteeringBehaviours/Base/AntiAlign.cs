using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiAlign : SteeringBehaviour
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    // Holds the radius for arriving at the target
    public float targetRadius;
    // Holds the radius for beginning to slow down
    public float slowRadius;
    // Holds the time over which to achieve target speed
    public float timeToTarget = 0.1f;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    override public Steering GetSteering(AgentNPC agent)
    {
        // ???:FIXME
        targetRadius = agent.interiorAngle; // 1
        slowRadius = agent.exteriorAngle;

        // Create the structure to hold our output
        Steering steer = new Steering();

        // Get the opposite direction to the target
        float rotation = target.orientation - agent.orientation + 180;

        // Map the result to the (-pi, pi) interval
        rotation = Utils.MapToRange(rotation);
        float rotationSize = Mathf.Abs(rotation);
        
        // Check if we are there, return no steering 
        if (rotationSize <= targetRadius)
            return null;
        
        float targetRotation;
        // If we are outside the slowRadius, then use maximum rotation
        if (rotationSize > slowRadius)
            targetRotation = agent.maxRotation;
        // Otherwise calculate a scaled rotation
        else
            targetRotation = agent.maxRotation * rotationSize / slowRadius;

        // The final target rotation combines speed and direction
        targetRotation *= rotation / rotationSize;

        // Acceleration tries to get to the target rotation
        steer.angular = targetRotation - agent.rotation;
        steer.angular /= timeToTarget;

        // Check if the acceleration is too great
        float angularAcceleration = Mathf.Abs(steer.angular);
        if (angularAcceleration > agent.maxAngularAcceleration) {
            steer.angular /= angularAcceleration;
            steer.angular *= agent.maxAngularAcceleration;
        }

        // Output the steering
        steer.linear = Vector3.zero;
        return steer;
    }

}
