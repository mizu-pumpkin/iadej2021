using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingAStar : MonoBehaviour
{

    private void Awake()
    {
        grid = GetComponent<GridTotalWar>();
    }

    public CombatSystem.TerrainType WhereAmI(AgentUnit npc)
    {
        if (grid != null)
            return grid.GetNodeTag(npc.position);
        else
            return CombatSystem.TerrainType.Unknown;
    }

    /*
        ▄▀█   █▀ ▀█▀ ▄▀█ █▀█
        █▀█   ▄█ ░█░ █▀█ █▀▄
    */

    public AStar aStar;
    private GridTotalWar grid;


    public List<Vector3> FindPathA_star(AgentUnit npc, Vector3 position)
    {
        List<Vector3> path = new List<Vector3>();

        // Convierte a nodo el punto real inicial
        Node startNode = grid.NodeFromWorldPoint(npc.position);
        // Convierte a nodo el punto real destino
        Node targetNode = grid.NodeFromWorldPoint(position);
        // Llama a A*
        List<Node> nodePath = aStar.FindAStar(npc, startNode, targetNode, grid);
        // Convierte los nodos en puntos del mapa
        foreach (Node n in nodePath)
            path.Add(n.position);
        
        return path;
    }
    

    // public List<Vector3> AStar(AgentUnit npc)
    // {
    //     costMap = grid.GetMatrixCost(npc);

    //     List<Vector3> path = null;

    //     RaycastHit hit;
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     if (Physics.Raycast(ray, out hit, 1000.0f))
    //     {
    //         if (hit.transform != null && hit.transform.tag != "Wall" && hit.transform.tag != "Water")
    //         {
    //             // Convierte a nodo el punto real inicial
    //             Node startNode = grid.NodeFromWorldPoint(npc.position);
    //             // Dibuja el punto destino
    //             if (TargetPoint != null) Destroy(TargetPoint);
    //             TargetPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //             TargetPoint.GetComponent<Renderer>().material.color = Color.red;
    //             TargetPoint.transform.localScale = Vector3.one * startNode.nodeSize;
    //             TargetPoint.transform.position = new Vector3(
    //                 hit.transform.position.x,
    //                 hit.transform.position.y + 5,
    //                 hit.transform.position.z
    //             );
    //             // Convierte a nodo el punto real destino
    //             Node targetNode = grid.NodeFromWorldPoint(TargetPoint.transform.position);
    //             // Llama a A*
    //             List<Node> nodePath = aStar.FindAStar(npc, startNode, targetNode, npc.heuristic, grid, costMap);

    //             foreach (Node n in nodePath)
    //                 path.Add(n.position);
    //         }
    //     }

    //     return path;
    // }

}
