using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : Action
{
    private float time = -1;
    private float timeToRespawn = 2;

    public Die(AgentUnit unit) : base(unit) {}

    public override void Execute()
    {
        if (time == -1)
            time = Time.time;
        
        if (Time.time - time >= timeToRespawn)
            isComplete = true;
    }

    public override string ToString() => "DEAD";
}
