using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursue : Seek
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    // Holds the maximum prediction time
    public float maxPrediction;

    protected Agent targetAgent, targetAux;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */
    
    public override void Awake() {
        base.Awake();
        targetAgent = target.GetComponent<Agent>();
        targetAux = target;
        target = new GameObject().AddComponent<Agent>();
    }

    void OnDestroy ()
    {
        Destroy(targetAux);
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        // 1. Calculate the target to delegate to seek

        // Work out the distance to target
        Vector3 direction = targetAux.position - agent.position;
        float distance = direction.magnitude;

        // Work ut our current speed
        float speed = agent.velocity.magnitude;

        float prediction;
        // Check if speed is too small to give a reasonable prediction time
        if (speed <= (distance / maxPrediction))
            prediction = maxPrediction;
        // Otherwise calculate the prediction time
        else
            prediction = distance / speed;

        // Put the target together
        target.position = targetAux.position;
        target.position += targetAgent.velocity * prediction; // FIXME

        // 2. Delegate to seek
        return base.GetSteering(agent);
    }
    
}