using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRTAStar : MonoBehaviour
{
    AgentNPC npc;
    Node actualNode;
    Node targetNode;
    MyGrid grid;
    int heuristic;

    void Update()
    {
        // Si aun no ha llegado al destino final
        if (actualNode != null && actualNode != targetNode)
        {
            float distance = (actualNode.position - npc.position).magnitude;
            // Si ya ha llegado al nodo actual, busca el siguiente nodo
            if (distance <= npc.exteriorRadius)
            {
                Node nextNode = FindNextStep();
                actualNode = nextNode;
                npc.SetTarget(actualNode.position);
            }
        }
    }

    public Node FindNextStep()
    {
        Node nextNode = null;
        List<Node> connectionNodes = grid.GetConnectionNodes(actualNode);
        
        foreach (Node connectionNode in connectionNodes) {
            float hCost = actualNode.gCost + GetHeuristicDistance(connectionNode, targetNode, heuristic);
            connectionNode.hCost = Mathf.Max(connectionNode.hCost, hCost);
        }
        
        float minCost = Mathf.Infinity;
        foreach (Node connectionNode in connectionNodes)
        {
            if (!connectionNode.isWall)
            {
                if ( (minCost == Mathf.Infinity) || (connectionNode.fCost < nextNode.fCost) )
                {
                    nextNode = connectionNode;
                    minCost = connectionNode.fCost;
                }
            }
        }
        if (actualNode.hCost < minCost)
            actualNode.hCost = minCost + 1;
        
        return nextNode;
    }

    public void FindPath(AgentNPC npc, Node startNode, Node targetNode, int heuristic, MyGrid grid)
    {
        this.npc = npc;
        this.actualNode = startNode;
        this.targetNode = targetNode;
        this.grid = grid;
        this.heuristic = heuristic;
        actualNode.hCost = actualNode.gCost + GetHeuristicDistance(actualNode, targetNode, heuristic);
    }

    // Calculo de distancias segun tres Heuristicas:
    public static int GetHeuristicDistance(Node p, Node q, int heuristic)
    {
        // El orden de los nodos no importa porque se van a
        // usar los valores absolutos o la potencia que hacen
        // que el signo sea irrelevante
        float x = (p.x - q.x);
        float y = (p.y - q.y);

        // Selecciona la heurística
        switch (heuristic)
        {
            case 2:
                // Chebyshev
                float chebyshev = Mathf.Max( Mathf.Abs(x), Mathf.Abs(y) );
                return (int) chebyshev;
            case 3:
                // Euclidea
                float euclidea = Mathf.Sqrt(x*x + y*y);
                return (int) euclidea;
            default: // 1 u otros
                     // Manhattan
                float manhattan = Mathf.Abs(x) + Mathf.Abs(y);
                return (int) manhattan;
        }
    }

    //public static List<Node> GetFinalPath(Node initialNode, Node targetNode) // TODO: MyGrid grid)
    //{
    //    List<Node> path = new List<Node>();
    //    Node actualNode = targetNode;
    //    while (actualNode != initialNode)
    //    {
    //        path.Add(actualNode);
    //        if (actualNode.parent == null) break;
    //        actualNode = actualNode.parent;
    //    }
    //    path.Reverse();
    //    //TODO: Creo que no es necesario: grid.FinalPath = path;
    //    return path;
    //}

}