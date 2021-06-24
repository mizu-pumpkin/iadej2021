using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    Node startNode;
    Node targetNode;
    GameObject NodeEnd; //Objeto visual
    LRTAstar lrtaStar = new LRTAstar();
    private MyGrid grid;
    float[,] costMap;
    //public int heuristicFunction = 1;

    private void Awake()
    {
        grid = GetComponent<MyGrid>();
        //lrtaStar = GetComponent<LRTAstar>();
    }
    
    private void Start()
    {
        costMap = grid.GetMatrixCost("standarValue");//valores normales
    }

    private void Update(){
        //if (Input.GetKey(KeyCode.M))
        //    heuristicFunction = 1;
        //if (Input.GetKey(KeyCode.C))
        //    heuristicFunction = 2;
        //if (Input.GetKey(KeyCode.E))
        //    heuristicFunction = 3;
    }

    public List<Vector3> FindFinalNode(AgentNPC npc)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        List<Node> path = new List<Node>();
        startNode = grid.NodeFromWorldPoint(npc.position);

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            if (hit.transform != null && hit.transform.tag != "Muro" && hit.transform.tag != "Rio")
            {
                // Dibuja el punto destino
                if (NodeEnd != null) Destroy(NodeEnd);
                NodeEnd = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NodeEnd.transform.localScale = new Vector3(10, 10, 10);
                NodeEnd.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y + 1, hit.transform.position.z);
                NodeEnd.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                
                targetNode = grid.NodeFromWorldPoint(NodeEnd.transform.position);

                path = lrtaStar.FindPath(startNode, targetNode, npc.heuristic, grid, costMap);
                
                List<Vector3> aux = new List<Vector3>(path.Count);

                for (int i=0; i < path.Count; i++)
                    aux.Add(path[i].position);
                
                return aux;
            }
        }

        return null;
    }

}