using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid : MonoBehaviour
{   
    // Cada casilla del mapa
    public GameObject casillas; 
    // Lo que mide cada casilla del mapa
    public float cubeSize;
    // Número de casillas fila
    public int rowMap;
    // Número de casillas columna
    public int colMap;

    // The array of Node that we'll use to find a path
    Node[,] NodeArray;


    private void Start() // Create grid
    {
        NodeArray = new Node[rowMap, colMap];
        
        // Loop through the array of Node
        for (int y = 0; y < rowMap; y++)
        {
            Transform cubeRow = casillas.transform.GetChild(y);
            // Loop through the array of Node
            for (int x = 0; x < colMap; x++)
            {
                Vector3 position = new Vector3( (x + 0.5f) * cubeSize, 0, (y + 0.5f) * cubeSize );

                // Check if the node is obstructed
                string tag = cubeRow.transform.GetChild(x).transform.tag;
                bool isWall = tag == "Water" || tag == "Wall";

                NodeArray[x, y] = new Node(x, y, cubeSize, isWall);
            }
        }
    }

    
    // Gets neighbouring nodes connected to the node
    public List<Node> GetConnectionNodes(Node node)
    {
        int x = node.x;
        int y = node.y;
        int leftX = x - 1;
        int rightX = x + 1;
        int topY = y + 1;
        int bottomY = y - 1;

        List<Node> connections = new List<Node>();

        // Left Node 
        if ((leftX >= 0 && leftX < rowMap) && (y >= 0 && y < colMap))
            connections.Add(NodeArray[leftX, y]);
        // Right Node   
        if ((rightX >= 0 && rightX < rowMap) && (y >= 0 && y < colMap))
            connections.Add(NodeArray[rightX, y]);
        // Top Node 
        if ((x >= 0 && x < rowMap) && (topY >= 0 && topY < colMap))
            connections.Add(NodeArray[x, topY]);
        // Botton Node 
        if ((x >= 0 && x < rowMap) && (bottomY >= 0 && bottomY < colMap))
            connections.Add(NodeArray[x, bottomY]);

        return connections;
    }


    //Gets the closest node to the given world position.
    public Node NodeFromWorldPoint(Vector3 npcPos)
    {
        float x = npcPos.x / cubeSize;
        float y = npcPos.z / cubeSize;

        if (x > 63) x = 63;
        if (y > 63) y = 63;
        if (x < 0) x = 0;
        if (y < 0) y = 0;

        return NodeArray[(int)x, (int)y];
    }
    
}