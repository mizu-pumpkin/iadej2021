using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAvoid : Seek
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    // Holds a colision detector
    public CollisionDetector collisionDetector;

    // Holds the minimum distance to a wall
    // should be greater than the radius of character
    public float avoidDistance;

    // Holds the distance to look ahead for a collision
    public float lookAhead = 1;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    void Awake() {
        target = new GameObject().AddComponent<Agent>();
    }

    void OnDestroy ()
    {
        Destroy(target);
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        avoidDistance = agent.interiorRadius * 1.5f;

        // 1. Calculate the target to delegate to seek

        // Calculate the collision ray vector
        Vector3 rayVector = agent.velocity.normalized * lookAhead;

        // Find the collision
        //Collision collision = collisionDetector.GetCollision(agent.position, rayVector);
        RaycastHit hit;
        bool collision = Physics.Raycast(agent.position, rayVector, out hit, lookAhead);

        // If have no collision, do nothing
        //if (collision.position == null)
        if (!collision)
            return new Steering();

        // Otherwise create a target
        target.position = hit.point + hit.normal * avoidDistance;

        // 2. Delegate to seek
        return base.GetSteering(agent);
    }

}
