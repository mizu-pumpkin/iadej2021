using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds a Static structure (i.e., position and orientation)
// representing the drift offset for the currently filled slots
// Posicion y orientacion para evitar los derrapes
public class DriftOffset
{
    public Vector3 position;
    public float orientation;

    public DriftOffset()
    {
        this.position = Vector3.zero;
        this.orientation = 0;
    }
    
}
