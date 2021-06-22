using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int tamGridRows;
    public int tamGridColumns;
    public float nodeRadius;
    public int mapRow;
    public int mapColumn;
    public Vector3 tamGrid;
    public Node[,] nodes;
    private int infinite = 9999999;

    //float minX;
    //float maxX; 
    //float minZ;
    //float maxZ;
    //public Transform[,] map;
    //public int tamNodo; 
    //public Vector3 esquinaInferiorIzquierda; //Cuando esté el mapa, poner valores
    //public Vector3 esquinaSuperiorIzquierda;
    //public Vector3 esquinaSuperiorDerecha;
    //public Vector3 esquinaSuperiorDeerecha;
    //public InfluenceMapControl mapaInfluencia;      //mapa de influencias del grid????


    public void Start()
    {
        nodeRadius = 2;
        tamGridRows = Mathf.RoundToInt(tamGrid.x / (nodeRadius*2));
        tamGridColumns = Mathf.RoundToInt(tamGrid.y / (nodeRadius*2));
        
        //Creamos el Grid
        int[,] costMatrix = getCostMatrix();
        
        nodes = new Node[tamGridRows, tamGridColumns];
        //Vector3 esquina = transform.position - Vector3.right * tamGrid.x / 2 - Vector3.forward * tamGrid.y / 2;
        for (int i = 0; i < tamGridRows; i++)
        {
            for (int j = 0; j <tamGridColumns; j++)
            {
                //Vector3 pos = esquina + Vector3.right * (i * (nodeRadius*2) + nodeRadius) + Vector3.forward * (i * (nodeRadius*2) + nodeRadius);
                //bool walkable = !isObject(pos);
                //Nodo.TerrainType terreno = getTerrenoPosition(pos);
                //Nodos[x, y] = new Nodo(walkable, pos, x, y,terreno);  //Creamos un nuevo nodo con las caracteristicas establecidas y calculadas a partir de lo que conocemos
                nodes[i, j].nodeDistance = (float) costMatrix[i, j];
                //nodes[i, j].calculoVisibilidad(); Para hacer visible

            }
        }

    }

    // Mapa de costes
    public int[,] getCostMatrix()
    {
        int[,] costMap = new int[mapRow, mapColumn];
        int nodeCost = 0;
        int costeNodo = 0;
        for (int i = 0; i < mapRow; i++)
        {
            for (int j = 0; j < mapColumn; j++)
            {
                /*
                // Si es un objeto del grid
                Collider[] hitColliders = Physics.OverlapSphere(map[i, j].position, nodeRadius);
                foreach (Collider hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "Muro" || hitCollider.gameObject.tag == "Agua") //Cambiar según el terreno que pongamos
                        nodeCost = infinite;
                        break;
                }
                */
                costeNodo = 1;
                if (costeNodo != infinite) 
                    nodeCost = 1;
                costMap[i, j] = nodeCost;
            }
        }
        return costMap;
    }

    // Función para obtener la lista de nodos adyacentes dado un nodo
     public List<Node> GetConnections(Node node)
    {
        List<Node> connections = new List<Node>();
        // Nodo izquierdo
        if ((node.x-1 >= 0 && node.x-1 < tamGridRows) && (node.y >= 0 && node.y < tamGridColumns))
        {
                Node nodeAdd = new Node(node.x-1, node.y);
                connections.Add(nodeAdd);
        }
        // Nodo derecho  
        if ((node.x+1 >= 0 && node.x+1 < tamGridRows) && (node.y >= 0 && node.y < tamGridColumns))
        {
                Node nodeAdd = new Node(node.x+1, node.y);
                connections.Add(nodeAdd);
        }
        // Nodo arriba
        if ((node.x >= 0 && node.x < tamGridRows) && (node.y+1 >= 0 && node.y+1 < tamGridColumns))
        {
                Node nodeAdd = new Node(node.x, node.y+1);
                connections.Add(nodeAdd);
        }
        // Nodo abajo
        if ((node.x >= 0 && node.x < tamGridRows) && (node.y-1 >= 0 && node.y-1 < tamGridColumns))
        {
                Node nodeAdd = new Node(node.x, node.y-1);
                connections.Add(nodeAdd);
        }        
        return connections; // Devuelve la lista con los nodos
    }
}
