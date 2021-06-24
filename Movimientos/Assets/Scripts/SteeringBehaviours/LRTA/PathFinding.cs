using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    Node startNode;
    Node targetNode;
    GameObject GoalNode;

    public LRTAStar lrtaStar;
    private MyGrid grid;
    float[,] costMap;

    private void Awake()
    {
        grid = GetComponent<MyGrid>();
        //lrtaStar = GetComponent<LRTAstar>();
    }
    
    private void Start()
    {
        costMap = grid.GetMatrixCost("standarValue");//valores normales
    }

    public void LRTA(AgentNPC npc)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            if (hit.transform != null && hit.transform.tag != "Muro" && hit.transform.tag != "Rio")
            {
                // Convierte a nodo el punto real inicial
                startNode = grid.NodeFromWorldPoint(npc.position);
                // Dibuja el punto destino
                if (GoalNode != null) Destroy(GoalNode);
                GoalNode = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GoalNode.GetComponent<Renderer>().material.color = Color.red;
                GoalNode.transform.localScale = Vector3.one * startNode.nodeSize;
                GoalNode.transform.position = new Vector3(
                    hit.transform.position.x,
                    hit.transform.position.y + 1,
                    hit.transform.position.z
                );
                // Convierte a nodo el punto real destino
                targetNode = grid.NodeFromWorldPoint(GoalNode.transform.position);
                // Llama a LRTA*
                lrtaStar.FindPath(npc, startNode, targetNode, npc.heuristic, grid);
            }
        }
    }

}