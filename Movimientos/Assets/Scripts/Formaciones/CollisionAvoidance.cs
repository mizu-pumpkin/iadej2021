using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAvoidance : SteeringBehaviour
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */

    // Holds a list of potential targets
    GameObject[] targets;

    // Holds the collision radius of a character
    // (we assume all character have the same radius here)
    public float radius = 0.4f;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("Agent");
        //targets =
        //    from c in GameObject.FindGameObjectsWithTag("NPC");
        //    select c.gameObject.transform.parent.gameObject.transform.parent.gameObject;
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        // 1. Find the target that's closest to collision

        // Store the first collision time
        float shortestTime = Mathf.Infinity;

        // Store the target that collides then, and other data
        // that we will need and can avoid recalculating
        Agent firstTarget = null;
        float firstMinSeparation = 0.0f;
        float firstDistance = 0.0f;
        Vector3 firstRelativePos = Vector3.zero;
        Vector3 firstRelativeVel = Vector3.zero;

        // Loop through each target
        foreach (GameObject target in targets)
        {
            Agent targetAgent = target.GetComponent<Agent>();
            
            // Calculate the time to collision
            Vector3 relativePos = targetAgent.position - agent.position;
            Vector3 relativeVel = targetAgent.velocity - agent.velocity;
            float relativeSpeed = relativeVel.magnitude;
            float timeToCollision = -Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

            // Check if it is going to be a collision at all
            float distance = relativePos.magnitude;
            float minSeparation = distance - relativeSpeed * timeToCollision; // CHECK
            if (minSeparation > 2 * radius)
                continue;
            
            // Check if it is the shortest
            if (timeToCollision > 0.0f && timeToCollision < shortestTime)
            {
                // Store the time, target and other data
                shortestTime = timeToCollision;
                firstTarget = targetAgent;
                firstMinSeparation = minSeparation;
                firstDistance = distance;
                firstRelativePos = relativePos;
                firstRelativeVel = relativeVel;
            }
        }

        // 2. Calculate the steering

        // If we have no target, then exit
        if (firstTarget == null)
            return null;
        
        // If we're going to hit exactly, or if we're already
        // colliding, then do the steering based on current position
        if (firstMinSeparation <= 0.0f || firstDistance < 2 * radius)
            firstRelativePos = firstTarget.position - agent.position;
        // Otherwise calculate the future relative position
        else
            firstRelativePos = firstRelativePos + firstRelativeVel * shortestTime;
        
        // Avoid the target
        firstRelativePos.Normalize();
        Steering steer = new Steering();
        steer.linear = -firstRelativePos * agent.maxAcceleration;

        // Return the steering
        return steer;
    }

}