using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAway : Move
{
    public RunAway(AgentUnit unit, Vector3 target) : base(unit, target) {}

    public override string ToString() => "FLEE";
}
