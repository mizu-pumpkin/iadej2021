using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{

    public List<Node> FindAStar(AgentUnit npc, Node startNode, Node targetNode, GridTotalWar grid)
    {
        // Initialize the open and closed lists
        List<Node> open = new List<Node>();
        open.Add(startNode);
        List<Node> closed = new List<Node>();

        // Iterate through processing each node
        while (open.Count > 0)
        {
            Node actualNode = open[0];

            // Find the smallest element in the open list
            // (using the estimatedTotalCost)
            for (int i = 1; i < open.Count; i++)
            {
                actualNode.hCost = GetHeuristicDistance(actualNode, targetNode, npc.heuristic);
                open[i].hCost = GetHeuristicDistance(open[i], targetNode, npc.heuristic);
                
                if (open[i].fCost < actualNode.fCost || open[i].fCost == actualNode.fCost && open[i].hCost < actualNode.hCost)
                    actualNode = open[i];
            }

            open.Remove(actualNode);
            closed.Add(actualNode);

            // If it is the goal node, then terminate
            if (actualNode == targetNode) break;

            // Otherwise get its outgoing connections
            List<Node> connections = grid.GetConnectionNodes(actualNode);

            foreach (Node connection in connections)
            {
                if (connection.isWall || closed.Contains(connection)) continue;
                
                //float connectionCost = actualNode.gCost + GetHeuristicDistance(actualNode, connection, npc.heuristic);
                float connectionCost = ConnectionTacticalCost(npc, actualNode, connection, grid);
                
                if (connectionCost < connection.gCost || !open.Contains(connection))
                {
                    connection.gCost = connectionCost;
                    connection.hCost = GetHeuristicDistance(actualNode, connection, npc.heuristic);
                    connection.parent = actualNode;

                    if (!open.Contains(connection)) open.Add(connection);
                }
            }   
        }

        return ComputePath(startNode, targetNode);
    }
    
    
    List<Node> ComputePath(Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node actualNode = targetNode;
        while (actualNode != startNode)
        {
            path.Add(actualNode);
            if (actualNode.parent != null)
                actualNode = actualNode.parent;
            else 
                break;
        }
        path.Reverse();
        return path;
    }


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


    float ConnectionTacticalCost(AgentUnit npc, Node actualNode, Node connection, GridTotalWar grid)
    {
        float actualNodeInfluence = 0;
        float connectionInfluence = 0;

        float aci = grid.ControlOverLocation(actualNode);
        float ci = grid.ControlOverLocation(connection);

        switch (npc.team)
        {
            case 0:
          
                if (aci > 0)
                    actualNodeInfluence = 0;
                else
                    actualNodeInfluence = Mathf.Abs(aci);

                if (ci > 0)
                    connectionInfluence = 0;
                else
                    connectionInfluence = Mathf.Abs(aci);
                break;

            case 1: 

                if (aci < 0)
                    actualNodeInfluence = 0;
                else
                    actualNodeInfluence = aci;

                if (ci < 0)
                    connectionInfluence = 0;
                else
                    connectionInfluence = ci;
                break;
        }

        float actualCost = grid.NodeCostUnit(actualNode, npc);
        float connectionCost = grid.NodeCostUnit(connection, npc);

        float terrainCost = (actualCost + connectionCost) / 2;
        float influenceCost = (actualNodeInfluence + connectionInfluence) / 2;
        
        return terrainCost + influenceCost;
    }

}