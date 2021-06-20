using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node parent;

    public Vector3 position;
    public float nodeDistance; // Distancia que hay hasta el siguiente nodo
    public float goalDistance; // Distancia hasta el objetivo

    public int x;
    public int y;

    public Node(Vector3 position, int x, int y)
    {
        this.position = position;
        this.x = x;
        this.y = y;
    }

    public float cost() {
        return nodeDistance + goalDistance;
    }

    // TODO: Añadir métodos no implementados
}
