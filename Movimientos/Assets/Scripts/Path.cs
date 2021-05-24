using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    private List<Vector3> nodes;
    
    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */
    
    public Path()
    {
        this.nodes = new List<Vector3>();
        // TODO
        AddNode(new Vector3(-10,0.5f,0));
        AddNode(new Vector3(-4,0.5f,6));
        AddNode(new Vector3(6,0.5f,5));
        AddNode(new Vector3(9,0.5f,0));
        AddNode(new Vector3(5,0.5f,-5));
    }

    public void AddNode(Vector3 node) {
        nodes.Add(node);
    }

    public List<Vector3> GetNodes() {
        return nodes;
    }
    
}
