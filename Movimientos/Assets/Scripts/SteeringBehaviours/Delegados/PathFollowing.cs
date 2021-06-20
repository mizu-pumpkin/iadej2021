using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowing : Seek
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    // Holds the path to follow
    public List<Vector3> path;

    // Holds the current position on the path
    public int currentNode = 0;

    int pathDir = 1;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */
    
    public override void Awake() {
        base.Awake();
        target = new GameObject("PathFollowingTarget").AddComponent<Agent>();
    }

    void OnDestroy ()
    {
        Destroy(target);
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        if (path.Count > 0) { // Comprueba si el personaje sigue algún camino
            List<Vector3> nodes = path;

            // Buscar objetivo actual
            target.position = nodes[currentNode];

            // Si he “llegado” al target, pasar al siguiente target
            float distance = Mathf.Abs((target.position - agent.position).magnitude);
            if (distance <= agent.interiorRadius) {
                currentNode += pathDir; // Siguiente objetivo

                // Opción 1. Me quedo en el final
                //if (currentNode >= nodes.length) {
                //    currentNode = nodes.length - 1;
                //}

                // Opción 2. Hago vigilancia (Vuelvo atrás)
                if (currentNode >= nodes.Count || currentNode < 0)
                {
                    pathDir *= -1;
                    currentNode += pathDir;
                }

                // Opción 3. Nuevo estado (steering)
                //if (currentNode >= nodes.length || currentNode < 0) {
                //    null;
                //}
            }

            return base.GetSteering(agent);
        } else {
            return null;
        }
    }

}