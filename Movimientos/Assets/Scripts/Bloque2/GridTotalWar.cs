using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTotalWar : MonoBehaviour
{
    //This is the mask that the program will look for when trying to find obstructions to the path.
    public LayerMask WallMask;
    
    // Cada casilla del mapa
    public GameObject casillas; 
    // Lo que mide la mitad de cada casilla del mapa
    public float cubeSize;
    // Número de casillas fila
    public int rowMap;
    // Número de casillas columna
    public int colMap;

    //The array of Node that we'll use to find a path
    Node[,] NodeArray;

    public Transform[,] influenceMap;


    private void Awake()
    {
        influenceMap = new Transform[rowMap, colMap];
        for (int y = 0; y < rowMap; y++)
        {
            Transform cubeRow = casillas.transform.GetChild(y);
            for (int x = 0; x < colMap; x++)
            {
                Transform cube = cubeRow.transform.GetChild(x);
                influenceMap[x, y] = cube;
            }
        }
    }


    private void Start()
    {
        CreateGrid();
    }


    void CreateGrid()
    {
        NodeArray = new Node[rowMap, colMap];
        
        // Loop through the array of Node
        for (int y = 0; y < rowMap; y++)
        {
            Transform cubeRow = casillas.transform.GetChild(y);
            // Loop through the array of Node
            for (int x = 0; x < colMap; x++)
            {
                string tag = cubeRow.transform.GetChild(x).transform.tag;
                
                Vector3 position = new Vector3( (x + 0.5f) * cubeSize, 0, (y + 0.5f) * cubeSize );
                // Check if the node is obstructed
                bool isWall = Physics.CheckSphere(position, cubeSize / 2, WallMask);

                if (tag == "Water" || tag == "Wall")
                    isWall = true;

                NodeArray[x, y] = new Node(x, y, cubeSize, isWall);
            }
        }
    }

    
    // Función para obtener la lista de nodos adyacentes dado un nodo
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

        //// Top Left Node
        //if ((leftX >= 0 && leftX < rowMap) && (topY >= 0 && topY < colMap))
        //    connections.Add(NodeArray[leftX, topY]);
        //// Bottom Left Node
        //if ((leftX >= 0 && leftX < rowMap) && (bottomY >= 0 && bottomY < colMap))
        //    connections.Add(NodeArray[leftX, bottomY]);    
        //// Top Right Node
        //if ((rightX >= 0 && rightX < rowMap) && (topY >= 0 && topY < colMap))
        //    connections.Add(NodeArray[rightX, topY]);
        //// Bottom Right Node
        //if ((rightX >= 0 && rightX < rowMap) && (bottomY >= 0 && bottomY < colMap))
        //    connections.Add(NodeArray[rightX, bottomY]);

        // Devuelve la lista con los nodos
        return connections;
    }


    //Gets the closest node to the given world position.
    public Node NodeFromWorldPoint(Vector3 npcPos)
    {
        float x = npcPos.x / cubeSize;
        float y = npcPos.z / cubeSize;

        return NodeArray[(int)x, (int)y];
    }

    //Get the map cost from a specific target
    public float[,] GetMatrixCost(string tagNPC)
    {
        float[,] costMap = new float[rowMap, colMap];
        for (int x = 0; x < rowMap; x++)
        {
            for (int y = 0; y < colMap; y++)
            {
                costMap[x, y] = NodeCostUnit(influenceMap[x, y], tagNPC);
            }
        }
        return costMap;
    }

    float NodeCostUnit(Transform nodo, string tagNPC)
    { // TODO
/*
        string zona = nodo.tag;
        switch (tagNPC)
        {
            case "SoldadoLigeroAzul":
                switch (zona)
                {
                    case "Hierba":
                        return 2;
                    case "Carretera":
                        return 1;
                    case "Puente":
                        return 1;
                    case "Bosque":
                        return 3;
                    case "BaseAzul":
                        return 1;
                    case "BaseRoja":
                        return 1;
                    case "ZonaSeguraAzul":
                        return 1;
                    case "ZonaSeguraRoja":
                        return 500;
                }
                break;
            case "SoldadoLigeroRojo":
                switch (zona)
                {
                    case "Hierba":
                        return 2;
                    case "Carretera":
                        return 1;
                    case "Puente":
                        return 1;
                    case "Bosque":
                        return 3;
                    case "BaseAzul":
                        return 1;
                    case "BaseRoja":
                        return 1;
                    case "ZonaSeguraAzul":
                        return 500;
                    case "ZonaSeguraRoja":
                        return 1;
                }
                break;
            case "SoldadoPesadoAzul":
                switch (zona)
                {
                    case "Hierba":
                        return 1.25f;
                    case "Carretera":
                        return 1.25f;
                    case "Puente":
                        return 1.25f;
                    case "Bosque":
                        return 4;
                    case "BaseAzul":
                        return 1;
                    case "BaseRoja":
                        return 1;
                    case "ZonaSeguraAzul":
                        return 1;
                    case "ZonaSeguraRoja":
                        return 500;
                }
                break;
            case "SoldadoPesadoRojo":
                switch (zona)
                {
                    case "Hierba":
                        return 1.25f;
                    case "Carretera":
                        return 1.25f;
                    case "Puente":
                        return 1.25f;
                    case "Bosque":
                        return 4;
                    case "BaseAzul":
                        return 1;
                    case "BaseRoja":
                        return 1;
                    case "ZonaSeguraAzul":
                        return 500;
                    case "ZonaSeguraRoja":
                        return 1;
                }
                break;
            case "ExploradorAzul":
                switch (zona)
                {
                    case "Hierba":
                        return 1;
                    case "Carretera":
                        return 2;
                    case "Puente":
                        return 1;
                    case "Bosque":
                        return 0.75f;
                    case "BaseAzul":
                        return 1;
                    case "BaseRoja":
                        return 1;
                    case "ZonaSeguraAzul":
                        return 1;
                    case "ZonaSeguraRoja":
                        return 500;
                }
                break;
            case "ExploradorRojo":
                switch (zona)
                {
                    case "Hierba":
                        return 1;
                    case "Carretera":
                        return 2;
                    case "Puente":
                        return 1;
                    case "Bosque":
                        return 0.75f;
                    case "BaseAzul":
                        return 1;
                    case "BaseRoja":
                        return 1;
                    case "ZonaSeguraAzul":
                        return 500;
                    case "ZonaSeguraRoja":
                        return 1;
                }
                break;
            case "ArqueroAzul":
                switch (zona)
                {
                    case "Hierba":
                        return 1;
                    case "Carretera":
                        return 0.75f;
                    case "Puente":
                        return 0.75f;
                    case "Bosque":
                        return 3;
                    case "BaseAzul":
                        return 1;
                    case "BaseRoja":
                        return 1;
                    case "ZonaSeguraAzul":
                        return 1;
                    case "ZonaSeguraRoja":
                        return 500;
                }
                break;
            case "ArqueroRojo":
                switch (zona)
                {
                    case "Hierba":
                        return 1;
                    case "Carretera":
                        return 0.75f;
                    case "Puente":
                        return 0.75f;
                    case "Bosque":
                        return 3;
                    case "BaseAzul":
                        return 1;
                    case "BaseRoja":
                        return 1;
                    case "ZonaSeguraAzul":
                        return 500;
                    case "ZonaSeguraRoja":
                        return 1;
                }
                break;
        }
*/
        return 1;
    }
    
    //return the tag of a specific point in the map
    public string GetTagFromPoint(int x, int y)
    {
        //if (influenceMap[x, y].tag == null) return null;
        return influenceMap[x, y].tag;
    }

    public float getNodeInfluence(Node n)
    {
        //TODO
        return 0;
    }
    
}