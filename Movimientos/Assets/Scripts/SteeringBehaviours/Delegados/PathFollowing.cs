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
    public Path path = new Path();

    // Holds the current position on the path
    public int currentNode = 0;

    int pathDir = 1;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */
    
    public override void Awake() {
        base.Awake();
        target = new GameObject().AddComponent<Agent>();
    }

    void OnDestroy ()
    {
        Destroy(target);
    }

    public float Distance(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        if (path != null) { // Comprueba si el personaje sigue algún camino
            List<Vector3> nodes = path.GetNodes();

            // Buscar objetivo actual
            target.position = nodes[currentNode];

            // Si he “llegado” al target, pasar al siguiente target
            if (Distance(agent.position, target.position) <= target.interiorRadius) {
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