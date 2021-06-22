using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRTA : MonoBehaviour
{
    public struct NodeRecord
    {
        public Node node;
        public Node connection;
        public int costSoFar;
        public int estimatedTotalCost;
        public int cost;
    }

    public int getDistance(Node p, Node q, int option)
    {
        float x = (p.x - q.x);
        float y = (p.y - q.y);
        // Selecciona la heurística
        switch (option)
        {
            case 2:
                // Chebyshev
                int chebyshev = Mathf.Max(Mathf.Abs(q.x - p.x), Mathf.Abs(q.y - p.y));
                return chebyshev;
            case 3:
                // Euclidea
                int euclidea = (int)Mathf.Sqrt((q.x - p.x) * (q.x - p.x) + (q.y - p.y) * (q.y - p.y));
                return euclidea;
            default: // 1 u otros
                     // Manhattan
                int manhattan = Mathf.Abs(p.x - q.x) + Mathf.Abs(p.y - q.y);
                return manhattan;
        }
    }

    public NodeRecord smallestElement(List<NodeRecord> open)
    {
        int smallestTotalCost = open[0].estimatedTotalCost;
        NodeRecord smallest = open[0];
        for (int i = 0; i < open.Count; i++)
        {
            if (open[i].estimatedTotalCost < smallestTotalCost)
            {
                smallestTotalCost = open[i].estimatedTotalCost;
                smallest = open[i];
            }

        }
        return smallest;
    }

    public NodeRecord findNodeRecord(List<NodeRecord> nodeRecords, Node node)
    {
        NodeRecord nodeRecordResultante = new NodeRecord();
        foreach (NodeRecord nodeRecord in nodeRecords)
        {
            if (nodeRecord.node == node)
                return nodeRecord;
        }
        return nodeRecordResultante;
    }


    public List<Node> pathfindAStar(Grid grid, Node start, Node end, int heuristic)
    {
        // Hacer comproación en otro sitio
        //if (grid.Nodos == null)
        //    return null;

        NodeRecord startRecord = new NodeRecord();
        startRecord.node = start;
        startRecord.connection = null;
        startRecord.costSoFar = 0;
        startRecord.estimatedTotalCost = getDistance(start, end, heuristic);

        // Initialize the open and closed lists 
        List<NodeRecord> open = new List<NodeRecord>();
        open.Add(startRecord);
        List<NodeRecord> closed = new List<NodeRecord>();
        NodeRecord current = new NodeRecord();

        // Iterate through processing each finalNode
        while (open.Count > 0)
        {
            int endNodeHeuristic;
            // Find the smallest element in the open list (using the estimatedTotalCost)
            current = smallestElement(open);

            // If it is the goal node, then terminate
            if (current.node == end)
                break;

            // Otherwise get its outgoing connections
            List<Node> connectionNodes = grid.GetConnections(current.node);
            List<NodeRecord> connections = new List<NodeRecord>(); 
            foreach (Node connection in connectionNodes)
            {
                connections.Add(findNodeRecord(open,connection));
            }

            // Loop through each connection in turn
            foreach (NodeRecord connection in connections)
            {

                // Get the cost estimate for the end node
                NodeRecord endNode = connection;
                int endNodeCost = current.costSoFar + getDistance(current.node, end, heuristic);
                NodeRecord endNodeRecord = new NodeRecord();

                // If the node is closed we may have to
                //skip, or remove it from the closed list.
                if (closed.Contains(endNode))
                {

                    // Here we find the record in the closed list
                    // corresponding to the endNode.
                    endNodeRecord = findNodeRecord(closed, endNode.node);

                    // If we didn’t find a shorter route, skip
                    if (endNodeRecord.costSoFar <= endNodeCost)
                        continue;

                    //  Otherwise remove it from the closed list
                    closed.Remove(endNodeRecord);

                    endNodeHeuristic = endNodeRecord.cost - endNodeRecord.costSoFar;
                    //getDistance(current, end, heuristic);

                }

                // Skip if the node is open and we’ve not
                //found a better route
                else if (open.Contains(endNode))
                {
                    // Here we find the record in the open list
                    // corresponding to the endNode.
                   endNodeRecord = endNode;

                    // If our route is no better, then skip
                    if (endNodeRecord.costSoFar <= endNodeCost)
                        continue;

                    endNodeHeuristic = getDistance(current.node, end, heuristic);

                    // Otherwise we know we’ve got an unvisited
                    // node, so make a record for it
                }
                else
                {
                    endNodeRecord = endNode;

                    // We’ll need to calculate the heuristic value
                    // using the function, since we don’t have an
                    // existing record to use
                    endNodeHeuristic = getDistance(current.node, end, heuristic);



                }

                //We’re here if we need to update the node
                //Update the cost, estimate and connection
                
                endNodeRecord.cost = endNodeCost;
                endNodeRecord.connection = connection.node;
                endNodeRecord.estimatedTotalCost = endNodeCost + endNodeHeuristic;

                //And add it to the open list
                if (!open.Contains(endNode))
                    open.Add(endNodeRecord);

            }

            // We’ve finished looking at the connections for
            // the current node, so add it to the closed list
            // and remove it from the open list
            open.Remove(current);
            closed.Add(current);
        }

        // We’re here if we’ve either found the goal, or
        // if we’ve no more nodes to search, find which.
        if (current.node != end)
        {
            // We’ve run out of nodes without finding the
            // goal, so there’s no solution
            return null;
        } else
        {
            // Compile the list of connections in the path
            List<Node> path = new List<Node>();

            // Work back along the path, accumulating
            // connections
            while (current.node != start)
            {
                path.Add(current.connection);
                current = findNodeRecord(closed, current.connection);
            }
                
            // Reverse the path, and return it
            path.Reverse();
            return path;
        }
        
        /*
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
        */




    }
    

    List<Node> newPath(Node initialNode, Node goal)
    {
        List<Node> path = new List<Node>();
        Node current = goal;
        while (current != initialNode)
        {
            path.Add(current);
            if (current.parent == null) break;
            current = current.parent;
        }
        path.Reverse();
        return path;
    }

    /*
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
    */

}