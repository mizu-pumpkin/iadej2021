using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternController : MonoBehaviour
{
    public FormationManager formationClass;
    private bool accion = false;

    public List<AgentNPC> SelectAgents()
    {
        List<AgentNPC> agents = new List<AgentNPC>();

        foreach (GameObject go in UnitsController.selectedUnits)
            agents.Add(go.GetComponent<AgentNPC>());
        
        return agents;
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            accion = true;
            formationClass.SelectedAgents(SelectAgents());
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            accion = false;
        }

        if (accion) {
            formationClass.updateSlots();
        }
    }

}