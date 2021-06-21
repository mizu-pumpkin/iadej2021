using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FIXME: controla que esté bien en la página web
public class Wander : Face
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    // Holds the wanderRand forward offset of the wander circle
    public float wanderOffset = 2;
    public float wanderRadius = 8;
    // Holds the maximum wanderRat which the wander orientation can change
    public float wanderRate = 90;
    // Holds the current orientation of the wander target
    float wanderOrientation;
    
    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public override void Awake() {
        target = new GameObject("WanderTarget").AddComponent<Agent>();
        target.position = transform.position;
        base.Awake();
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        // 1. Calculate the target to delegate to face

        // Update the wander orientation
        float wanderOrientation = Random.Range(-1.0f, 1.0f) * wanderRate;

        // Calculate the combined target orientation
        float targetOrientation = wanderOrientation + agent.orientation;

        // Calculate the center of the wander circle
        targetAux.position = agent.position + wanderOffset * Utils.OrientationToVector(agent.orientation).normalized;

        // Calculate the target location
        targetAux.position += wanderRadius * Utils.OrientationToVector(targetOrientation).normalized;

        // 2. Delegate to Face
        Steering steer = base.GetSteering(agent);
        if (steer == null) steer = new Steering();

        // 3. Now set the linear acceleration to be at full
        // acceleration in the direction of the orientation
        steer.linear = agent.maxAcceleration * Utils.OrientationToVector(agent.orientation).normalized;

        // Output the steering
        return steer;
    }
    
}