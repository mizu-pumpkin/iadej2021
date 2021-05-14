using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : Face
{
    // Holds the radius and forward offset of the wander circle
    public float wanderOffset;
    public float wanderRadius;
    // Holds the maximum rate at which the wander orientation can change
    public float wanderRate;
    // Holds the current orientation of the wander target
    float wanderOrientation;
    
    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public override void Awake() {
        target = new GameObject().AddComponent<Agent>();
        target.position = transform.position;
        base.Awake();
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        // 1. Calculate the target to delegate to face

        // Update the wander orientation
        wanderOrientation += Random.Range(-1.0f, 1.0f) * wanderRate;

        // Calculate the combined target orientation
        float targetOrientation = wanderOrientation + agent.orientation;

        // Calculate the center of the wander circle
        target.position = agent.position + wanderOffset * agent.OrientationToVector();

        // Calculate the target location
        target.position += wanderRadius * agent.OrientationToVector(targetOrientation);

        // 2. Delegate to Face
        Steering steer = base.GetSteering(agent);

        // 3. Now set the linear acceleration to be at full
        // acceleration in the direction of the orientation
        steer.linear = agent.maxAcceleration * agent.OrientationToVector().normalized;

        // Output the steering
        return steer;
    }
}