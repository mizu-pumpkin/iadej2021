using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeCover : Move
{
    public TakeCover(AgentUnit unit, Vector3 target) : base(unit, target) {}

    public override string ToString()=> "HIDE";
}
