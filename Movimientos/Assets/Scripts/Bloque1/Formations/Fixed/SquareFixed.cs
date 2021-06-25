using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareFixed : Fixed
{
    protected override void GridPositions()
    {
        this.gridPositions[0] = new Vector3(0,0,0);
        this.gridPositions[1] = new Vector3(-1,0,0);
        this.gridPositions[2] = new Vector3(-1,0,-1);
        this.gridPositions[3] = new Vector3(0,0,-1);
    }

    protected override void GridOrientations()
    {
        this.gridOrientations[0] = 0;
        this.gridOrientations[1] = 0;
        this.gridOrientations[2] = 225;
        this.gridOrientations[3] = 135;
    }
}