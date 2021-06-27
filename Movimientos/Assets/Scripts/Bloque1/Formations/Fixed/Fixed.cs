using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Fixed : MonoBehaviour
{
    public AgentNPC[] agentes = new AgentNPC[4];
    protected Vector3[] gridPositions = new Vector3[4];
    protected float[] gridOrientations = new float[4];
    public float separation = 4;
    protected abstract void GridPositions();
    protected abstract void GridOrientations();

    void Awake()
    {
        GridPositions();
        GridOrientations();
    }

    public void AddAgents(List<AgentNPC> newAgents)
    {
        for (int i = 0; i < 4; i ++)
            agentes[i] = newAgents[i];
    }
    
    public void Update()
    {
        //Find the anchor point
        AgentNPC leader = agentes[0];
        float radTheta = -leader.orientation * Mathf.Deg2Rad;

        // Go through each character in turn
        for (int i = 1; i < agentes.Length; i++)
        {
            // Ask for the location of the slot relative to the
            // anchor point. This should be a Static structure
            DriftOffset relativeLoc = getGridLocation(i);

            // Transform it by the leader point’s position and /orientation
            DriftOffset location = new DriftOffset();
            location.position = leader.position + Utils.getPosition(radTheta, relativeLoc.position);
            location.orientation = leader.orientation + relativeLoc.orientation;
            
            // Write the static to the character
            agentes[i].SetTarget(//location);
                location.position, location.orientation
            );
        }
    }

    public DriftOffset getGridLocation(int gridLoc)
    {
        DriftOffset location = new DriftOffset();
        location.position = gridPositions[gridLoc] * separation;
        location.orientation = gridOrientations[gridLoc];
        return location;
    }

}