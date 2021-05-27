using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastAgentNPC : AgentNPC
{
    public List<string> favoriteTerrain;
    public List<string> difficultTerrain;

    public override void Awake()
    {
        base.Awake();
        this.favoriteTerrain = new List<string>();
        this.difficultTerrain = new List<string>();
        this.mass = 10;
        this.maxSpeed = 10;
        this.maxAcceleration = 5;
    }
}