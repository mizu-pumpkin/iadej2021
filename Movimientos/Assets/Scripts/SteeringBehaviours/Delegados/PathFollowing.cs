using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Path Following sin pathOffset (seek a waypoints)
public class PathFollowing : SteeringBehaviour
{


    public Vector3 currentParam; 
    public Path path;
    public float predictTime = 0.1;
    public float currentPos = 0.0f;
    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

     /*
    public override Steering GetSteering(AgentNPC agent)
    {

        futurePos = agent.position + agent.velocity + predictTime;
        currentParam = path.GetParam(futurePos, currentPos);
        //float targetParam = currentParam + pathOffset; NO HAY QUE USAR OFFSET
        target.position = path.GetPosition(targetParam);
        return base.GetSteering();
    }
*/

    Agent target = nodes[currentNode];
    pathDir = 1;
    // Si he “llegado” al target, pasar al siguiente target
    if (distance(position, target) <= radius)
        currentNode += pathDir;
    // Opción 1. Me quedo en el final.
    if (currentNode >= nodes.magnitude)
        currentNode = nodes.magnitude - 1;
    // Opción 2. Hago vigilancia (Vuelvo atrás)
    if (currentNode >= nodes.length || currentNode < 0) {
        pathDir = - pathDir;
        currentNode += pathDir;
    }
    // Opción 3. Nuevo estado (steering)
    if (currentNode >= nodes.length || currentNode < 0)
        new Steering();

}