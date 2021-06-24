using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviour
{
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

        Vector3 direction = target.position - agent.position;
        float distance = direction.magnitude;

        if (distance <= agent.interiorRadius)
            return null;
        
        // Get the direction to the target
        steer.linear = direction;

        // Give full acceleration along this direction
        steer.linear.Normalize();
        steer.linear *= agent.maxAcceleration;

        // Output the steering
        steer.angular = 0;
        return steer;
    }

}
