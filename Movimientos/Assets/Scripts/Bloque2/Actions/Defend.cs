using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : Move
{
    public Defend(AgentUnit unit, Vector3 target) : base(unit, target) {}

    public override string ToString() => "DEFEND";
}
