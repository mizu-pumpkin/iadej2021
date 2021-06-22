using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{/*
    public int tamGridRows;
    public int tamGridColumns;
    public int tamNodo; 
    public float nodeRadius;
    public int mapRow;
    public int mapColumn;
    public Transform[,] map;
    public Vector3 tamGrid;
    public Node[,] nodes;
    float minX;
    float maxX; 
    float minZ;
    float maxZ;

    public Vector3 esquinaInferiorIzquierda; //Cuando esté el mapa, poner valores
    public Vector3 esquinaSuperiorIzquierda;
    public Vector3 esquinaInferiorDerecha;
    public Vector3 esquinaSuperiorDerecha;

    public void Start()
    {
        tamGridRows = Mathf.RoundToInt(tamGrid.x / (nodeRadius*2));
        tamGridColumns = Mathf.RoundToInt(tamGrid.y / (nodeRadius*2));
        
        //Creamos el Grid
         int[,] costMatrix = getCostMatrix();
        nodes = new Node[tamGridRows tamGridColumns];
        Vector3 esquina = transform.position - Vector3.right * tamGrid.x / 2 - Vector3.forward * tamGrid.y / 2;
        for (int i = 0; i < tamGridRows; i++)
        {
            for (int j = 0; j <tamGridColumns; j++)
            {
                Vector3 pos = esquina + Vector3.right * (i * (nodeRadius*2) + nodeRadius) + Vector3.forward * (i * (nodeRadius*2) + nodeRadius);
                //bool walkable = !isObject(pos);
                //Nodo.TerrainType terreno = getTerrenoPosition(pos);
                //Nodos[x, y] = new Nodo(walkable, pos, x, y,terreno);  //Creamos un nuevo nodo con las caracteristicas establecidas y calculadas a partir de lo que conocemos
                nodes[i, j].nodeDistance = (float) costMatrix[i, j];
                //nodes[i, j].calculoVisibilidad(); Para hacer visible

            }
        }


        if (influenceMap != null) // Se utiliza para el seguimiento de la influencia en la ubicación del mapa tu puedes :)
        {
            
            //Todo esto, no lo entiendo, quizás sería mejor fijar desde el principio que los
            // campos de la esquina inferior van a ser menor que los campos de la esquina superior????????

            if (esquinaInferiorIzquierda.x < esquinaSuperiorDerecha.x)
            {
                minX = esquinaInferiorIzquierda.x;
                maxX = esquinaSuperiorDerecha.x;
            }
            else
            {
                maxX = esquinaInferiorIzquierda.x;
                minX = esquinaSuperiorDerecha.x;
            }

            if (esquinaInferiorIzquierda.z < esquinaSuperiorDerecha.z)
            {
                minZ = esquinaInferiorIzquierda.z;
                maxZ = esquinaSuperiorDerecha.z;
            }
            else
            {
                maxZ = esquinaInferiorIzquierda.z;
                minZ = esquinaSuperiorDerecha.z;
            }

        }

    }

    // Mapa de costes
    public int[,] getCostMatrix()
    {
        int[,] costMap = new int[mapRow, mapColumn];
        int nodeCost = 0

        for (int i = 0; i < mapRow; i++)
        {
            for (int j = 0; j < mapColumn; j++)
            {
                // Si es un objeto del grid
                Collider[] hitColliders = Physics.OverlapSphere(map[i, j].position, nodeRadius);
                foreach (Collider hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "Muro" || hitCollider.gameObject.tag == "Agua") //Cambiar según el terreno que pongamos
                        nodeCost = 9999999999;
                        break;
                }
                if (costeNodo != 9999999999) nodeCost = 1;
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
                connections.Add(Nodos[node.x-1, node.y]);

        // Nodo derecho  
        if ((node.x+1 >= 0 && node.x+1 < tamGridRows) && (node.y >= 0 && node.y < tamGridColumns))
                connections.Add(Nodos[node.x+1, node.y]);

        // Nodo arriba
        if ((node.x >= 0 && node.x < tamGridRows) && (node.y+1 >= 0 && node.y+1 < tamGridColumns))
                connections.Add(Nodos[node.x, node.y+1]);

        // Nodo abajo
        if ((node.x >= 0 && node.x < tamGridRows) && (node.y-1 >= 0 && node.y-1 < tamGridColumns))
                connections.Add(Nodes[node.x, node.y-1]);
    
        return connections; // Devuelve la lista con los nodos
    }*/
}
