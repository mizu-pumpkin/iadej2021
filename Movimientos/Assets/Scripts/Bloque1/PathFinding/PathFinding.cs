using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public LRTAStar lrtaStar;
    private MyGrid grid;
    private GameObject TargetPoint;

    private void Awake()
    {
        grid = GetComponent<MyGrid>();
    }

    public void LRTA_star(AgentNPC npc)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            if (hit.transform != null && hit.transform.tag != "Wall" && hit.transform.tag != "Water")
            {
                // Convierte a nodo el punto real inicial
                Node startNode = grid.NodeFromWorldPoint(npc.position);
                // Dibuja el punto destino
                if (TargetPoint != null) Destroy(TargetPoint);
                TargetPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                TargetPoint.GetComponent<Renderer>().material.color = Color.red;
                TargetPoint.transform.localScale = Vector3.one * startNode.nodeSize;
                TargetPoint.transform.position = new Vector3(
                    hit.transform.position.x,
                    hit.transform.position.y + 1,
                    hit.transform.position.z
                );
                // Convierte a nodo el punto real destino
                Node targetNode = grid.NodeFromWorldPoint(TargetPoint.transform.position);
                // Llama a LRTA*
                lrtaStar.FindPath(npc, startNode, targetNode, npc.heuristic, grid);
            }
        }
    }

}