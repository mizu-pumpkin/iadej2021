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
    public float[,] CostInfluenceMap;

    public static float[,] CosteUnidad = {
        // bush, forest, grass, base, bridge
        { 3.00f, 2.00f, 1.00f, 1.00f, 1.00f }, // tanker
        { 2.00f, 2.00f, 1.00f, 1.00f, 1.00f }, // healer
        { 0.50f, 0.75f, 1.00f, 1.00f, 1.25f }  // ranged
    };


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
        CostInfluenceMap = new float[rowMap,colMap];
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

        // // Top Left Node
        // if ((leftX >= 0 && leftX < rowMap) && (topY >= 0 && topY < colMap))
        //    connections.Add(NodeArray[leftX, topY]);
        // // Bottom Left Node
        // if ((leftX >= 0 && leftX < rowMap) && (bottomY >= 0 && bottomY < colMap))
        //    connections.Add(NodeArray[leftX, bottomY]);    
        // // Top Right Node
        // if ((rightX >= 0 && rightX < rowMap) && (topY >= 0 && topY < colMap))
        //    connections.Add(NodeArray[rightX, topY]);
        // // Bottom Right Node
        // if ((rightX >= 0 && rightX < rowMap) && (bottomY >= 0 && bottomY < colMap))
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
    public float[,] GetMatrixCost(AgentUnit agente)
    {
        float[,] costMap = new float[rowMap, colMap];
        for (int x = 0; x < rowMap; x++)
        {
            for (int y = 0; y < colMap; y++)
            {
                costMap[x, y] = NodeCostUnit(influenceMap[x, y], agente);
            }
        }
        return costMap;
    }

    float NodeCostUnit(Transform nodo, AgentUnit npc)
    { 
        int terrainId;

        // ???: por qué los números no coinciden con la matriz definida arriba?
        switch(nodo.tag)
        {
            case "BaseA":
                terrainId = 3;
                break;
            case "BaseB":
                terrainId = 3;
                break;
            case "Grass":
                terrainId = 2;
                break;
            case "Bridge":
                terrainId = 0; 
                break;
            default:
                terrainId = 1;
                break;       
        }

        return CosteUnidad[terrainId,((int)npc.unitType)];
    }
    
    public float[,] CompleteInfluenceMap(List<AgentUnit> teamA, List<AgentUnit> teamB)
    {
        for (int x = 0; x < rowMap; x++)
            for (int y = 0; y < colMap; y++)
                CostInfluenceMap[x, y] = ControlOverLocation(teamA, teamB, NodeArray[x,y]);
        
        return CostInfluenceMap;
    }
    
    //return the tag of a specific point in the map
    public string GetTagFromPoint(int x, int y)
    {
        //if (influenceMap[x, y].tag == null) return null;
        return influenceMap[x, y].tag;
    }
    
    public float InfluenceOverLocation(List<AgentUnit> team, Node nodo)
    {
        float influence = 0;
        foreach(AgentUnit unit in team) {
            Node AgentPosition = NodeFromWorldPoint(unit.position);
            //print("entramos con " + unit.team);
            if (Mathf.Abs(nodo.x - AgentPosition.x) > unit.effectRadius || (nodo.y-AgentPosition.y) > unit.effectRadius)
                continue;
            //print("entramos con " + unit.team);
            float distance;
            if (nodo.x > nodo.y)
                distance = unit.effectRadius - nodo.x;
            else
                distance = unit.effectRadius - nodo.y;
            // FIXME: elige uno de estos dos
            // rapid initial drop off, but with a longer range of influence
            influence += unit.influence / Mathf.Sqrt(1 + distance);
            
            // plateaus first before rapidly tailing off at a distance
            //influence += unit.influence / Mathf.Pow(1 + distance);
        }

        if (influence > 1)
            return 1;
        
        if (influence < -1)
            return -1;
            
        return influence;
    }

    // If teamA wins, returns a positive number
    // If teamB wins, returns a negative number
    public float ControlOverLocation(List<AgentUnit> teamA, List<AgentUnit> teamB, Node nodo)
    {
        float influenceA = InfluenceOverLocation(teamA, nodo);
        float influenceB = InfluenceOverLocation(teamB, nodo);

        return influenceA - influenceB;
    }
    
}