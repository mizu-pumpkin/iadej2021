using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviour
{    // Holds the radius for arriving at the target
    public float targetRadius;
    // Holds the radius for beginning to slow down
    public float slowRadius;
    // Holds the time over which to achieve target speed
    public float timeToTarget = 0.1f;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    // Si el agente está cerca del objetivo no se moverá; pero si aún no
    // está cerca, entonces irá al objetivo a máxima velocidad
    override public Steering GetSteering(AgentNPC agent)
    {
        // Create the structure to hold our output
        Steering steer = new Steering();

        // Get the direction to the target
        Vector3 direction = target.position - agent.position;
        float distance = direction.magnitude;

        // Check if we are there, return no steering
        if (distance < targetRadius)
            return steer;

        float targetSpeed;
        // If we are outside the slowRadius, then go max speed
        // TODO: añadir que pare antes de tocar al target (interiorRadius)
        if (distance > slowRadius)
            targetSpeed = agent.maxSpeed;
        // Otherwise calculate a scaled speed
        else
            targetSpeed = agent.maxSpeed * distance / slowRadius;
        
        // The target velocity combines speed and direction
        Vector3 targetVelocity = direction;
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        // Acceleration tries to get to the target velocity
        steer.linear = targetVelocity - agent.velocity;
        steer.linear /= timeToTarget;

        // Check if the acceleration is too fast
        if (steer.linear.magnitude > agent.maxAcceleration) {
            steer.linear.Normalize();
            steer.linear *= agent.maxAcceleration;
        }

        // Output the steering
        steer.angular = 0;
        return steer;
    }

}