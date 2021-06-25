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
    [SerializeField] private int currentNode;
    // The direction in which we are going in the path
    private int pathDir = 1;

    public enum Mode { stay, patrol, stop };
    public Mode mode;

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
        if (target != null)
            Destroy(target);
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        //if (path != agent.path) {
        //    path = agent.path;
        //    currentNode = 0;
        //}
        
        // Comprueba si el personaje sigue algún camino
        if (path.Count > 0) { 
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
                            currentNode = nodes.Count - 1;
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
                            currentNode = nodes.Count - 1;
                        return null;
                }
            }

            return base.GetSteering(agent);
        } else {
            return null;
        }
    }

}