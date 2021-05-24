using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : SteeringBehaviour
{
    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    // Si el agente está cerca del objetivo no se moverá; pero si aún no
    // está cerca, entonces irá al objetivo a máxima velocidad
    // TODO: añadir distancia limite para que deje de huir
    override public Steering GetSteering(AgentNPC agent)
    {
        // Create the structure to hold our output
        Steering steer = new Steering();

        // Get the direction to the target
        steer.linear = agent.position - target.position;

        // Give full acceleration along this direction
        steer.linear.Normalize();
        steer.linear *= agent.maxAcceleration;

        // Output the steering
        steer.angular = 0;
        return steer;
    }

}
