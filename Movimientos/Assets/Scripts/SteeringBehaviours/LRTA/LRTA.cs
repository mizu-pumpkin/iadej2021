using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRTA : MonoBehaviour
{
    public int getDistance(Node p, Node q, int option)
    {
        float x = (p.x-q.x);
        float y = (p.y-q.y);
        // Selecciona la heurística
        switch (option) {
        case 2:
            // Chebyshev
            int chebyshev = Mathf.Max( Mathf.Abs(q.x-p.x), Mathf.Abs(q.y-p.y) );
            return chebyshev;
        case 3:
            // Euclidea
            int euclidea = (int) Mathf.Sqrt( (q.x-p.x)*(q.x-p.x) + (q.y-p.y)*(q.y-p.y) );
            return euclidea;
        default: // 1 u otros
            // Manhattan
            int manhattan = Mathf.Abs(p.x-q.x) + Mathf.Abs(p.y-q.y);
            return manhattan;
        }
    }

    List<Node> newPath(Node initialNode, Node goal)
    {
        List<Node> path = new List<Node>();
        Node current = goal;
        while (current != initialNode) {
            path.Add(current);
            if (current.parent == null) break;
            current = current.parent;
        }
        path.Reverse();
        return path;
    }

    public List<Node> pathLRTA( Grid grid, Node start, Node end, int heuristic)
    {
        // Hacer comproación en otro sitio
        //if (grid.Nodos == null)
        //    return null;

        // Initialize the closed lists 
        List<Node> closed = new List<Node>();

        Node current = start;
        current.goalDistance = getDistance(current, end, heuristic);

        // While it is not the goal node
        while (current != end)
        {
            List<Node> connections = grid.GetConnections(current);

            // Get its outgoing connections
            foreach (Node connection in connections)
                current.goalDistance = getDistance(connection, end, heuristic);

            float min = Mathf.Infinity;

            Node next = null;
            foreach (Node connection in connections)
            {
                if (closed.Contains(connection)) continue;
                
                // Coger siguiente nodo, de menos coste
                if (min == Mathf.Infinity || connection.cost() < next.cost()) { // ???: para qué es el min??
                    next = connection;
                    min = connection.cost();
                }
            }
            // Añadir nodo a la lista escogida y avanzar al siguiente
            closed.Add(current);
            current = next;

            // Ver si es el último
            if (next == null)
                break;
        }

        return closed;
    }

    public List<Node> pathfindAStar(Grid grid, Node start, Node end, int heuristic)
    {
        // Hacer comproación en otro sitio
        //if (grid.Nodos == null)
        //    return null;

        // Initialize the open and closed lists 
        List<Node> open = new List<Node>();
        open.Add(start);
        List<Node> closed = new List<Node>();

        // Iterate through processing each finalNode
        while (open.Count > 0)
        {
            // Find the smallest element in the open list
            Node current = open[0];

            for (int i = 1; i < open.Count; i++)
            {
                current.goalDistance = getDistance(current, end, heuristic);
                open[i].goalDistance = getDistance(open[i], end, heuristic);
                
                if (open[i].cost() < current.cost() ||
                    (open[i].cost() == current.cost() && open[i].goalDistance < current.goalDistance))
                    current = open[i];
            }

            closed.Add(current);
            open.Remove(current);

            // If it is the goal node
            if (current == end)
                return newPath(start, end);

            // Otherwise get its outgoing connections
            List<Node> connections = grid.GetConnections(current);
            foreach (Node connection in connections)
            {
                //if (!connection.walkable || closed.Contains(connection)) continue;
                
                // Skip if the node is closed
                if ((closed.Contains(connection))) continue;
                float distance = getDistance(current, connection, heuristic);

                // TODO: Terreno
                //float connectionCost = 0;
                //if (tactico)
                //{
                //    connectionCost = costeVecinoTactico(current, connection, multiplicadorTerreno, grid, npc.tipo, npc.team, coste, multiplicadorInfluencia, multiplicadorVisibilidad);
                //}
                //else
                //{
                //    connectionCost = current.nodeDistance + distance;
                //}
                //if (connectionCost < connection.nodeDistance || !open.Contains(connection))
                //{
                //    connection.nodeDistance = connectionCost;
                //    connection.goalDistance = getDistance(current, connection, option);
                
                //    connection.parent = current;
                //    if (!open.Contains(connection))
                //        open.Add(connection);
                //}            
            }
        }
        return newPath(start, end);
    }

    // TODO: Mirar según terreno

}