using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : Face
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    // Holds the wanderRand forward offset of the wander circle
    public float wanderOffset;
    public float wanderRadius;
    // Holds the maximum wanderRat which the wander orientation can change
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

    public Vector3 OrientationToVector(float orientation) {
        Vector3 vector  = Vector3.zero;
        vector.x = Mathf.Sin(orientation * Mathf.Deg2Rad) * 1.0f;
        vector.z = Mathf.Cos(orientation * Mathf.Deg2Rad) * 1.0f;
        return vector.normalized;
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        // 1. Calculate the target to delegate to face

        // Update the wander orientation
        float wanderOrientation = Random.Range(-1.0f, 1.0f) * wanderRate;

        // Calculate the combined target orientation
        float targetOrientation = wanderOrientation + agent.orientation;

        // Calculate the center of the wander circle
        targetAux.position = agent.position + wanderOffset * OrientationToVector(agent.orientation);

        // Calculate the target location
        targetAux.position += wanderRadius * OrientationToVector(targetOrientation);

        // 2. Delegate to Face
        Steering steer = base.GetSteering(agent);
        if (steer == null) steer = new Steering();

        // 3. Now set the linear acceleration to be at full
        // acceleration in the direction of the orientation
        steer.linear = agent.maxAcceleration * OrientationToVector(agent.orientation);

        // Output the steering
        return steer;
    }
    
}