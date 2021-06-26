using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowingPredict : Seek
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

    public float predictTime = 0.1f;
    public int offset = 1;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public override void Awake()
    {
        base.Awake();

        target = new GameObject("PathFollowingPredictTarget").AddComponent<Agent>();

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

    override public Steering GetSteering(AgentNPC agent)
    {        
        // Comprueba si el personaje sigue algún camino
        if (path != null && path.Count > 0)
        {
            // 1. Calculate the target to delegate to face

            // Find the predicted future location
            Vector3 futurePosition = agent.position + agent.velocity * predictTime;

            float distance = Mathf.Abs((target.position - futurePosition).magnitude);
            if (distance <= agent.exteriorRadius)
            {
                // Find the current position on the path
                // Offset it
                currentNode += pathDir * offset; // Siguiente objetivo

                switch (mode)
                {
                    case Mode.stay: // Opción 1. Me quedo en el final
                        if (currentNode >= path.Count)
                        {
                            arrived = true;
                            currentNode = path.Count - 1;
                        }
                        break;
                    case Mode.patrol: // Opción 2. Hago vigilancia (Vuelvo atrás)
                        if (currentNode >= path.Count || currentNode < 0)
                        {
                            if (currentNode >= path.Count)
                                currentNode = path.Count;
                            if (currentNode < 0)
                                currentNode = -1;
                            pathDir *= -1;
                            currentNode += pathDir * offset;
                        }
                        break;
                    case Mode.stop: // Opción 3. Nuevo estado (steering)
                        if (currentNode >= path.Count)
                        {
                            arrived = true;
                            currentNode = path.Count - 1;
                        }
                        return null;
                }
            }
            // Get the target position
            target.position = path[currentNode];

            return base.GetSteering(agent);
        } else {
            return null;
        }
    }
}
