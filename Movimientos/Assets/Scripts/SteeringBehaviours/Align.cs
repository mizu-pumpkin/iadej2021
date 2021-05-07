using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBehaviour
{
    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    // Si el agente está cerca del objetivo no se moverá; pero si aún no
    // está cerca, entonces irá al objetivo a máxima velocidad
    override public Steering GetSteering(AgentNPC agent)
    {

        //Holdas the max angular acceleration and rotation of the character
        //maxAngularAcceleration = ??
        maxRotation = agent.maxRotation;

        // Holds the radius for arriving at the target
        float targetRadius = target.exteriorRadius;

        // Holds the radius for beginning to slow down
        float slowRadius = targetRadius * 8;

        // Holds the time over which to achieve target speed
        float timeToTarget = 0.1f;

        // Create the structure to hold our output
        Steering steer = new Steering();

        // Get the naive direction to the target
        float rotation = target.orientation - agent.orientation;

        // Map the result to the (-pi, pi) interval
        rotation = mapToRange(rotation);
        float rotationSize = Mathf.Abs(rotation);

        // Check if we are there, return no steering 
        if (rotationSize < targetRadius)
            return new Steering();

        // If we are outside the slowRadius, then use maximum rotation
        if (rotationSize > slowRadius)
            target.rotation = agent.maxRotation;

        // Otherwise calculate a scaled rotation
        else target.rotation = agent.maxRotation * rotationSize / slowRadius;
     
    }
}
