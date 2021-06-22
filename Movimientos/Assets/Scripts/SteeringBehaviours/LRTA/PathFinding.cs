using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour

{
    //public Grid grid;
    public LRTA lrta;

    private void Start()
    {
        
        //grid = grid.GetComponent<Grid>();
        lrta = new LRTA();
    }


    /*
    public LRTA lrta =  new LRTA();

    public void pathFinding(Vector3 initialNodePosition, Vector3 finalNodePosition)
    {
        Node current = grid.GetNodoPosicionGlobal(initialNodePosition);
        Node final = grid.GetNodoPosicionGlobal(finalNodePosition);
        List<Node> nodes = lrta.pathA(Node initialNode, Node goal, Grid grid, int distanceOption);
        //List<GameObject> keyPoints = new List<GameObject>();

        if (nodes != null)
        {
            List<Vector3> aux = new List<Vector3>(nodes.Count);
            for (int i = 0; i < nodes.Count; i++)
            {
                GameObject keyPoint = new GameObject("Keypoint");
                keyPoint.transform.position = nodes[i].position;
                keyPoints.Add(keyPoint);
            }

        }

    }
*/
}
