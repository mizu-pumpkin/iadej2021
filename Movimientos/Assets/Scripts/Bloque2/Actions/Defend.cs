using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : Action
{
    public Defend(AgentUnit unit) : base(unit) {}

    public override void Execute() {}

    public override string ToString() => "DEFEND";
}
