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
    //public CollisionDetector collisionDetector;

    // Holds the minimum distance to a wall
    // should be greater than the radius of character
    float avoidDistance;

    // Holds the distance to look ahead for a collision
    public float lookAhead = 10;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public override void Awake() {
        base.Awake();
        priority = 1;
        target = new GameObject().AddComponent<Agent>();
    }

    void OnDestroy ()
    {
        Destroy(target);
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        avoidDistance = agent.interiorRadius * 10;
/*
        // FIXME: para tener 3 bigotes
        float angle = Utils.PositionToAngle(agent.velocity.normalized);
        Vector3 frw = agent.velocity.normalized * lookAhead;
        Vector3 izq = Utils.OrientationToVector(angle+agent.interiorAngle).normalized * lookAhead;
        Vector3 der = Utils.OrientationToVector(angle-agent.interiorAngle).normalized * lookAhead;
        
        Debug.DrawLine(agent.position, frw, Color.red);
        Debug.DrawLine(agent.position, izq, Color.green);
        Debug.DrawLine(agent.position, der, Color.yellow);

        // creamos un array de Raycast, entre uno y el numero maximo de bigotes, el primero que es mirar recto es el que siempre tendra como mínimo
        List<Vector3> bigotes = new List<Vector3>();
        bigotes.Add(frw);
        bigotes.Add(izq);
        bigotes.Add(der);

        // Find the collision
        foreach (Vector3 rayVector in bigotes) {
            RaycastHit hit;
            bool collision = Physics.Raycast(agent.position, rayVector, out hit, lookAhead);
   
            if (!collision) continue;
            
            // Otherwise create a target
            target.position = hit.point + hit.normal * avoidDistance;

            // Draw lines
            Debug.DrawLine(agent.position, hit.point, Color.red);
            Debug.DrawLine(hit.point, target.position, Color.blue);

            // 2. Delegate to seek
            return base.GetSteering(agent);
        }
        return null;
*/
        // 1. Calculate the target to delegate to seek

        // Calculate the collision ray vector
        Vector3 rayVector = agent.velocity.normalized * lookAhead;

        // Find the collision
        RaycastHit hit;
        bool collision = Physics.Raycast(agent.position, rayVector, out hit, lookAhead);

        // If have no collision, do nothing
        if (!collision)
            return null;

        // Otherwise create a target
        target.position = hit.point + hit.normal * avoidDistance;

        // Draw lines
        Debug.DrawLine(agent.position, hit.point, Color.red);
        Debug.DrawLine(hit.point, target.position, Color.blue);

        // 2. Delegate to seek
        return base.GetSteering(agent);
    }

}
