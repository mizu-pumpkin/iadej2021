using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : Action
{
    public Patrol(AgentUnit unit) : base(unit) {}

    public override void Execute() {}

    public override string ToString() => "PATROL";
}
