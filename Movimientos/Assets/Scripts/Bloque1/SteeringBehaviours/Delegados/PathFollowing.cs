using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowing : Seek
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    public GameObject pathObject;
    // Holds the path to follow
    public List<Vector3> path;
    // Holds the current position on the path
    public int currentNode;
    // The direction in which we are going in the path
    public int pathDir = 1;

    public enum Mode { stay, patrol, stop };
    public Mode mode;

    public bool arrived;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */
    
    public override void Awake() {
        base.Awake();
        target = new GameObject("PathFollowingTarget").AddComponent<Agent>();

        if (pathObject != null) {
            path = new List<Vector3>();
            for (int i = 0; i < pathObject.transform.childCount; i++)
            {
                Transform pathPoint = pathObject.transform.GetChild(i);
                path.Add(pathPoint.position);
            }
        }

        if (debug) {
            target.debug = true;
            target.interiorRadius = 1;
            target.exteriorRadius = 2;
        }
    }

    void OnDestroy ()
    {
        if (target != null)
            Destroy(target);
    }

    override public Steering GetSteering(AgentNPC agent)
    {   
        // Comprueba si el personaje sigue algún camino
        if (path != null && path.Count > 0)
        {
            List<Vector3> nodes = path;

            // Buscar objetivo actual
            target.position = nodes[currentNode];

            // Si he “llegado” al target, pasar al siguiente target
            float distance = Mathf.Abs((target.position - agent.position).magnitude);
            if (distance <= agent.exteriorRadius)
            {
                currentNode += pathDir; // Siguiente objetivo

                switch (mode)
                {
                    case Mode.stay: // Opción 1. Me quedo en el final
                        if (currentNode >= nodes.Count)
                        {
                            arrived = true;
                            currentNode = nodes.Count - 1;
                        }
                        break;
                    case Mode.patrol: // Opción 2. Hago vigilancia (Vuelvo atrás)
                        if (currentNode >= nodes.Count || currentNode < 0)
                        {
                            pathDir *= -1;
                            currentNode += pathDir;
                        }
                        break;
                    case Mode.stop: // Opción 3. Nuevo estado (steering)
                        if (currentNode >= nodes.Count)
                        {
                            arrived = true;
                            currentNode = nodes.Count - 1;
                        }
                        return null;
                }
            }

            return base.GetSteering(agent);
        } else {
            return null;
        }
    }

}