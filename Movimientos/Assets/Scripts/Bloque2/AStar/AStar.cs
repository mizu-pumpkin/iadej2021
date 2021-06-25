using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
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
                float chebyshev = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                return (int)chebyshev;
            case 3:
                // Euclidea
                float euclidea = Mathf.Sqrt(x * x + y * y);
                return (int)euclidea;
            default: // 1 u otros
                     // Manhattan
                float manhattan = Mathf.Abs(x) + Mathf.Abs(y);
                return (int)manhattan;
        }
    }

    //TODO: revisar
    public static Node smallestElement(List<Node> open)
    {
        float smallestTotalCost = open[0].hCost; // Usa el coste estimado //estimatedCost
        Node smallest = open[0];
        for (int i = 0; i < open.Count; i++)
        {
            if (open[i].hCost < smallestTotalCost)
            {
                smallestTotalCost = open[i].hCost;
                smallest = open[i];
            }

        }
        return smallest;
    }


    // TODO:He utilizado 
    //pseudo del libro (quitar esto obviamente)
    public List<Node> FindAStar(AgentUnit npc, Node startNode, Node targetNode, int heuristic, GridTotalWar grid,float[,] costMap)
    {
        // Camino a devolver
        List<Node> path = new List<Node>();
        Node actual;

        // Initialize the open and closed lists
        List<Node> open = new List<Node>();
        open.Add(startNode);
        List<Node> closed = new List<Node>();

        // Iterate through processing each node
        while (open.Count > 0)
        {
            // Find the smallest element in the open list
            //  (using the estimatedTotalCost)
            Node actualNode = smallestElement(open);

            // If it is the goal node, then terminate
            if (actualNode == targetNode)
            {
                actual = targetNode;
                while (actual != startNode)
                {
                    path.Add(actual);
                    if (actual.parent != null)
                        actual = actual.parent;
                    else break;
                }

                // Reverse the path
                path.Reverse();
                return path;

            }

            // Otherwise get its outgoing connections
            List<Node> connections = grid.GetConnectionNodes(actualNode);

            // Loop through each connection in turn
            foreach (Node connection in connections)
            {
                //  If the node is closed we may have to
                // skip.
                if (!connection.isWall || closed.Contains(connection))
                {
                    continue;
                }

                // Nuevo insertado
                float moveCost = costMap[connection.x,connection.y];

                if (moveCost < connection.gCost || !open.Contains(connection))
                {
                    connection.gCost = moveCost;
                    connection.hCost = GetHeuristicDistance(actualNode, connection, heuristic);
                    connection.parent = actualNode;

                    if (!open.Contains(connection))
                    {
                        open.Add(connection);
                    }
                }

            }

        }

        // Work back along the path, accumulating connections
        actual = targetNode;

        while (actual != startNode){
            path.Add(actual);
            if (actual.parent != null)
                actual = actual.parent;
            else break;
        }

        // Reverse the path
        path.Reverse();
        return path;
        
    }

}