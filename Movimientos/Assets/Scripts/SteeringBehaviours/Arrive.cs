using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviour
{
    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    // Si el agente está cerca del objetivo no se moverá; pero si aún no
    // está cerca, entonces irá al objetivo a máxima velocidad
    override public Steering GetSteering(AgentNPC agent)
    {
        // Holds the max acceleration and speed of the character
        float maxAcceleration = agent.maxAcceleration;
        float maxSpeed = agent.maxSpeed;

        // Holds the radius for arriving at the target
        float targetRadius = target.exteriorRadius;

        // Holds the radius for beginning to slow down
        float slowRadius = targetRadius * 8;

        // Holds the time over which to achieve target speed
        float timeToTarget = 0.1f;

        // Create the structure to hold our output
        Steering steer = new Steering();

        // Get the direction to the target
        Vector3 direction = target.position - agent.position;
        float distance = direction.magnitude;
        //float x = this.target.position.x - agent.position.x;
        //float z = this.target.position.z - agent.position.z;
        //float distance = Mathf.Sqrt(x * x + z * z);

        // Check if we are there, return no steering
        if (distance < targetRadius)
            return steer;

        float targetSpeed;
        // If we are outside the slowRadius, then go max speed
        if (distance > slowRadius)
            targetSpeed = maxSpeed;
        // Otherwise calculate a scaled speed
        else
            targetSpeed = maxSpeed * distance / slowRadius;
        
        // The target velocity combines speed and direction
        Vector3 targetVelocity = direction;
        targetVelocity = targetVelocity.normalized * targetSpeed;

        // Acceleration tries to get to the target velocity
        steer.linear = targetVelocity - agent.velocity;
        steer.linear /= timeToTarget;

        // Check if the acceleration is too fast
        if (steer.linear.magnitude > maxAcceleration)
            steer.linear = steer.linear.normalized * maxAcceleration;

        // Output the steering
        steer.angular = agent.Heading(target.position);
        return steer;
        
    }

}