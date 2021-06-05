using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendedSteering : SteeringBehaviour
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    //struct BehaviorAndWeight {
    //    SteeringBehaviour behavior;
    //    float weight;
    //}

    // Holds a list of BehaviorAndWeight instances    
    public List<SteeringBehaviour> behaviors; //List<BehaviorAndWeight> behaviors;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public override void Awake()
    {
        base.Awake();
        this.behaviors = new List<SteeringBehaviour>();
    }

    // Return the acceleration required
    override public Steering GetSteering(AgentNPC agent) {
        // Create the steering structure for accumulation
        Steering accum = new Steering();

        // Accumulate all accelerations
        foreach (SteeringBehaviour behavior in behaviors) {
            Steering steer = behavior.GetSteering(agent);
            if (steer == null) continue;
            accum.linear += behavior.weight * steer.linear;
            accum.angular += behavior.weight * steer.angular;
        }

        // Crop the result and return
        //accum.linear = Mathf.Max(accum.linear, agent.maxAcceleration);
        if (accum.linear.magnitude > agent.maxSpeed)
            accum.linear = accum.linear.normalized * agent.maxSpeed;
        accum.angular = Mathf.Max(accum.angular, agent.maxRotation);

        return accum;
    }

}