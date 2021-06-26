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

    GameObject[,] PrintInfluenceMap;

    public Transform[,] influenceMap;
    public float[,] CostInfluenceMap;

    public static float[,] CosteUnidad = {
        // Tanker, Healer, Ranged
        { 1.00f, 1.00f, 2.00f }, // Street
        { 3.00f, 2.00f, 0.75f }, // Forest
        { 1.00f, 1.00f, 1.00f }, // Plains
        { 1, 1, 1 },             // Heal
        { 1, 1, 1 },             // BaseA
        { 1, 1, 1 },             // BaseB
        { 100, 100, 100 },       // Water
        { 100, 100, 100 },       // Unknown
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
        PrintInfluenceMap = new GameObject[rowMap, colMap];
    }


    private void Start()
    {
        CreateGrid();
        CreatePrintInfluenceMap();
    }

    void Update()
    {
        StartCoroutine("UpdateInfluenceMap");
    }
    

    IEnumerator UpdateInfluenceMap()
    {
        GetCostInfluenceMap();
        yield return new WaitForSeconds(20);
    }

    public void CreatePrintInfluenceMap()
    {
        for (int x = 0; x < rowMap; x++)
        {
            for (int y = 0; y < colMap; y++)
            {
                PrintInfluenceMap[x, y] = GameObject.CreatePrimitive(PrimitiveType.Quad);
                PrintInfluenceMap[x, y].GetComponent<Renderer>().material.color = Color.white;
                PrintInfluenceMap[x, y].transform.localScale = influenceMap[x, y].localScale;
                PrintInfluenceMap[x, y].transform.position = NodeArray[x, y].position + Vector3.right*400;
                PrintInfluenceMap[x, y].transform.Rotate(new Vector3(90, 0, 0));
            }
        }
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
                Vector3 position = new Vector3( (x + 0.5f) * cubeSize, 0, (y + 0.5f) * cubeSize );

                // Check if the node is obstructed
                bool isWall = Physics.CheckSphere(position, cubeSize / 2, WallMask);
                string tag = cubeRow.transform.GetChild(x).transform.tag;
                if (tag == "Water" || tag == "Wall") isWall = true;

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

        if (x > colMap-1) x = colMap-1;
        if (y > rowMap-1) y = rowMap-1;
        if (x < 0) x = 0;
        if (y < 0) y = 0;

        return NodeArray[(int)x, (int)y];
    }

    public float[,] GetMatrixCost(AgentUnit agente)
    {
        float[,] costMap = new float[rowMap, colMap];

        for (int x = 0; x < rowMap; x++)
            for (int y = 0; y < colMap; y++)
                costMap[x, y] = NodeCostUnit(influenceMap[x, y], agente);
        
        return costMap;
    }

    public CombatSystem.TerrainType GetNodeTag(Node node)
    {
        string tag = influenceMap[node.x, node.y].tag;
        switch (tag)
        {
            case "Street": return CombatSystem.TerrainType.Street;
            case "Forest": return CombatSystem.TerrainType.Forest;
            case "Grass": return CombatSystem.TerrainType.Plains;
            case "Heal": return CombatSystem.TerrainType.Heal;
            case "BaseA": return CombatSystem.TerrainType.BaseA;
            case "BaseB": return CombatSystem.TerrainType.BaseB;
            case "Water": return CombatSystem.TerrainType.Water;
            default: return CombatSystem.TerrainType.Unknown;
        }
    }

    public CombatSystem.TerrainType GetNodeTag(Vector3 position)
    {
        return GetNodeTag( NodeFromWorldPoint(position) );
    }
    
    public float NodeCostUnit(Node node, AgentUnit npc)
    {
        int terrainId = (int) GetNodeTag(node);
        return CosteUnidad[terrainId, (int) npc.unitType];
    }

    float NodeCostUnit(Transform nodo, AgentUnit npc)
    {
        int terrainId = (int) GetNodeTag(nodo.position);
        return CosteUnidad[terrainId, (int) npc.unitType];
    }

    public float[,] GetCostInfluenceMap()
    {
        for (int x = 0; x < rowMap; x++)
            for (int y = 0; y < colMap; y++)
                CostInfluenceMap[x, y] = ControlOverLocation(NodeArray[x,y]);
        
        return CostInfluenceMap;
    }

    public float ControlOverLocation(Node nodo)
    {
        List<AgentUnit> teamA = GameManager.GetTeamA();
        List<AgentUnit> teamB = GameManager.GetTeamB();

        float influenceNodo = InfluenceOverLocation(teamA, nodo) - InfluenceOverLocation(teamB, nodo);
        // blue 
        if (influenceNodo > 0)   
            PrintInfluenceMap [nodo.x,nodo.y].GetComponent<Renderer>().material.color = new Color(1 - influenceNodo, 1 - influenceNodo, 1);
        // red
        if (influenceNodo < 0)   
            PrintInfluenceMap [nodo.x,nodo.y].GetComponent<Renderer>().material.color = new Color(1, 1 + influenceNodo, 1 + influenceNodo);
        //white
        if (influenceNodo == 0) 
            PrintInfluenceMap [nodo.x,nodo.y].GetComponent<Renderer>().material.color = Color.white;

        return influenceNodo;
    }
    
    public float InfluenceOverLocation(List<AgentUnit> team, Node nodo)
    {
        float influence = 0;

        foreach(AgentUnit unit in team)
        {    
            Node AgentPosition = NodeFromWorldPoint(unit.position);
            
            if (Mathf.Abs(nodo.x - AgentPosition.x) > unit.effectRadius ||
                Mathf.Abs(nodo.y-AgentPosition.y) > unit.effectRadius) continue;

            float distance;
            if (Mathf.Abs(AgentPosition.x - nodo.x) > Mathf.Abs(AgentPosition.y - nodo.y))
                distance = Mathf.Abs(AgentPosition.x - nodo.x);
            else
                distance = Mathf.Abs(AgentPosition.y - nodo.y);
            
            // FIXME: elige uno de estos tres
            influence += unit.influence / (1 + distance);
            //influence += unit.influence / Mathf.Sqrt(1 + distance); // rapid initial drop off, but with a longer range of influence
            //influence += unit.influence / Mathf.Pow(1 + distance); // plateaus first before rapidly tailing off at a distance
        }
       
        if (influence > 1) return 1;
        if (influence < -1) return -1; 
            
        return influence;
    }
    
}