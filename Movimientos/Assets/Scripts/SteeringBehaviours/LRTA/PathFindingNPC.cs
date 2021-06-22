using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PathFindingNPC : MonoBehaviour
{
    public LRTA lrta; 
    public Grid grid = new Grid();
    public int x;
    public int y;
    //public List <Node> objetivos;
    // Start is called before the first frame update
    void Start()
    {

        List <Node> objetivos = lrta.pathfindAStar(grid,new Node(x,y), new Node(5,5), 1);
        foreach(Node o in objetivos){
            print(o.x + " " + o.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
