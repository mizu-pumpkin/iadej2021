using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAvoid : Seek
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */

    // Holds the distance to look ahead for a collision
    public float lookAhead = 10;

    public bool debug = false;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public override void Awake() {
        base.Awake();
        priority = 1;
        target = new GameObject("WallAvoidTarget").AddComponent<Agent>();
    }

    void OnDestroy ()
    {
        Destroy(target);
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        // Holds the minimum distance to a wall
        // should be greater than the radius of character
        float avoidDistance = agent.interiorRadius * 10;

        // 1. Calculate the target to delegate to seek

        // Calculate the collision ray vectors (whiskers)
        Vector3 frontWhisker = agent.velocity.normalized * lookAhead;
        Vector3 leftWhisker = Quaternion.Euler(0, -agent.interiorAngle, 0) * frontWhisker;
        Vector3 rightWhisker = Quaternion.Euler(0, agent.interiorAngle, 0) * frontWhisker;

        // Draw whiskers
        if (debug)
        {
            Debug.DrawLine(agent.position, agent.position + frontWhisker, Color.yellow);
            Debug.DrawLine(agent.position, agent.position + leftWhisker, Color.yellow);
            Debug.DrawLine(agent.position, agent.position + rightWhisker, Color.yellow);
        }

        // Find the collision
        RaycastHit frontHit, leftHit, rightHit, hit;
        if ( Physics.Raycast(agent.position, frontWhisker, out frontHit, lookAhead) )
            hit = frontHit;
        else if ( Physics.Raycast(agent.position, leftWhisker, out leftHit, lookAhead) )
            hit = leftHit;
        else if ( Physics.Raycast(agent.position, rightWhisker, out rightHit, lookAhead) )
            hit = rightHit;
        // If have no collision, do nothing
        else return null;

        // Otherwise create a target
        target.position = hit.point + hit.normal * avoidDistance;

        // Draw lines
        if (debug)
        {
            Debug.DrawLine(agent.position, hit.point, Color.red);
            Debug.DrawLine(hit.point, target.position, Color.blue);
        }

        // 2. Delegate to seek
        return base.GetSteering(agent);
    }

}
