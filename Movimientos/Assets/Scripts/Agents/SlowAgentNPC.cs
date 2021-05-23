using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowAgentNPC : AgentNPC
{
    public List<string> favoriteTerrain;
    public List<string> difficultTerrain;

    public override void Awake()
    {
        base.Awake();
        this.favoriteTerrain = new List<string>();
        this.difficultTerrain = new List<string>();
        this.mass = 20;
        this.maxSpeed = 5;
        this.maxAcceleration = 2;
    }
}
