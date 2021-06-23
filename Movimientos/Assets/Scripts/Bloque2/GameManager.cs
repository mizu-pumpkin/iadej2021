using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TODO
    // - cambio de modo (ofensivo/defensivo/totalwar)

    // FIXME: de momento todo esto es temporaneo
    public List<AgentUnit> units;
    public void Update()
    {
        if (units.Count == 2)
        {
            if (units[1] != null) {
                // Attack
                if (Input.GetKeyDown("k"))
                    units[0].Attack(units[1]);
                // Attack
                if (Input.GetKeyDown("h"))
                    units[0].Heal(units[1]);
                // Defend
                if (Input.GetKeyDown("d")) {
                    units[0].Defend();
                    units[1].Defend();
                }
                // Patrol
                if (Input.GetKeyDown("p")) {
                    units[0].Patrol();
                    units[1].Patrol();
                }
                // Stop
                if (Input.GetKeyDown("s")) {
                    units[0].InitializeSteerings();
                    units[1].InitializeSteerings();
                }
            }
            else {
                units.RemoveAt(1);
            }
        }   
    }

}