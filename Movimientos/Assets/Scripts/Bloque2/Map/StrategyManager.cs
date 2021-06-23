using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyManager : MonoBehaviour
{
    public enum Mode { attack, defend, totalWar };

    public List<Waypoint> waypoints;

    void Awake() {
        waypoints = new List<Waypoint>(this.GetComponents<Waypoint>());
    }
}
