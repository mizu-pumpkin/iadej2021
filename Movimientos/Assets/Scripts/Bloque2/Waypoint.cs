using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // Holds the values of the waypoint for the tactic we are currently condensing
    public enum Type {
        teamBase, // base de un bando
        cure, // zona de curacion
        ambush, // punto bueno para emboscada
        vantage, // punto muy defensible
        cover, shadow, exposure,
    }
    public Type value;

    public enum ControlledBy { none, teamA, teamB }
    public ControlledBy controlledBy = ControlledBy.none;

    // Holds the position of the waypoint
    public Vector3 position {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public List<Waypoint> connections;
}
