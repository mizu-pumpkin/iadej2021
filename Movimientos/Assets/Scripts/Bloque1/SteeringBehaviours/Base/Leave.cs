using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Lo opuesto de Arrive es Leave
// Es poco creíble acelerar desde cero a máxima velocidad, si acaso todo lo contrario.
// En la práctica, lo opuesto de Arrive es Flee.
public class Leave : SteeringBehaviour
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    // Holds the radius for escaping the target
    public float escapeRadius;
    // Holds the radius for the danger zone from the target
    public float dangerRadius;
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
        Vector3 direction = agent.position - target.position;
        float distance = direction.magnitude;

        // Check if we are out of danger, return no steering
        if (distance > dangerRadius)
            return null;

        float reduce;
        // Keep speed if we are still inside escapeRadius
        if (distance < escapeRadius)
            reduce = 0f;
        // Start reducing speed if we are outside of escapeRadius
        else
            reduce = distance / dangerRadius * agent.maxSpeed;
        float targetSpeed = agent.maxSpeed - reduce;
        
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
