using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFixed : Fixed
{

    protected override void GridPositions()
    {
        for (int i=0; i < gridPositions.Length; i++)
            this.gridPositions[i] = new Vector3(i,0,0);
    }

    protected override void GridOrientations()
    {
        for (int i=0; i < gridPositions.Length; i++)
            this.gridOrientations[i] = 0;
    }
}