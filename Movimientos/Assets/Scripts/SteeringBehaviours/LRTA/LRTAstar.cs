using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRTAstar : MonoBehaviour // TODO: que no sea subclase
{

    public List<Node> FindPath(Node startNode, Node targetNode, int heuristic, MyGrid grid, float [,] mapaCostes)
    {
        List<Node> closed = new List<Node>();
        
        Node actualNode = startNode;
        actualNode.hCost = actualNode.gCost + GetHeuristicDistance(actualNode, targetNode, heuristic);
        
        while (actualNode != targetNode)
        {
            Node nextNode = null;
            List<Node> connectionNodes = grid.GetConnectionNodes(actualNode);
           
            foreach (Node connectionNode in connectionNodes)
                // ???: connectionNode.hCost = connectionNode.gCost + GetHeuristicDistance(connectionNode, targetNode, heuristic);
                connectionNode.hCost = actualNode.gCost + GetHeuristicDistance(connectionNode, targetNode, heuristic);
            
            float minCost = Mathf.Infinity;
            foreach (Node connectionNode in connectionNodes)
            {
                if (!closed.Contains(connectionNode)) // ???: && !connectionNode.isWall
                {
                    if ( (minCost == Mathf.Infinity) || (connectionNode.fCost < nextNode.fCost) )
                    {
                        nextNode = connectionNode;
                        minCost = connectionNode.fCost;
                    }
                }
            }
            actualNode.hCost = minCost + 1;
            closed.Add(actualNode);

            if (nextNode == null) break;
            actualNode = nextNode;
        }

        return closed;
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

    public static List<Node> GetFinalPath(Node initialNode, Node targetNode) // TODO: MyGrid grid)
    {
        List<Node> path = new List<Node>();
        Node actualNode = targetNode;

        while (actualNode != initialNode)
        {
            path.Add(actualNode);
            if (actualNode.parent == null) break;
            actualNode = actualNode.parent;
        }
        path.Reverse();

        //TODO: Creo que no es necesario: grid.FinalPath = path;

        return path;
    }

}