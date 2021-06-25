using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: editar los comentarios en ingles cuando sea el momento uwu
public class Node
{
    public Node parent; // For the AStar algoritm, will store what node it previously came from so it cn trace the shortest path.

    public int x; // Posicion X del nodo
    public int y; // Posicion Y del nodo
    
    // Dimensión del nodo en el mapa
    public float nodeSize; 
    // Posicion del nodo en el mapa
    public Vector3 position { get { return new Vector3(x * nodeSize, 0, y * nodeSize); } }
    
     // Distancia que hay hasta el siguiente nodo
    public float gCost;
    // Distancia hasta el nodo objetivo
    public float hCost;
    // Coste total
    public float fCost { get { return gCost + hCost; } }
    
    // Indica si el nodo es transitable
    public bool isWall;

    public Node(int x, int y, float nodeSize, bool isWall)
    {
        this.x = x;
        this.y = y;
        this.nodeSize = nodeSize;
        this.isWall = isWall;

        if (isWall)
        {
            gCost = Mathf.Infinity;
            hCost = Mathf.Infinity;
        }
    }
    
}