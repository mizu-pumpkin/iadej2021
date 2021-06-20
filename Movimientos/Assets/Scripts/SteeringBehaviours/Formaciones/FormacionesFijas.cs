using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormacionesFijas : Seek
{
    //Elección de leader
    public Agent leader;
    GameObject[] formacion;

    //Introducir en la lista aquellos que vayan a realizar la formación
    public override void Awake() {
        base.Awake();
        formacion = GameObject.FindGameObjectsWithTag("PFija");
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        float[,] omegaLeader = new float[,] { { Mathf.Cos(leader.orientation), -1*Mathf.Sin(leader.orientation) }, { Mathf.Sin(leader.orientation), Mathf.Cos(leader.orientation)}};
        Vector3 rs = leader.position - agent.position; //Rs, distancia del agente con respecto al leader
        Vector3 omegaRs = new Vector3 ((omegaLeader[0,0]* rs.x +omegaLeader[0,1]*rs.x), rs.y, (omegaLeader[1,0]* rs.z +omegaLeader[1,1]*rs.z));
        Agent targetAgent = target.GetComponent<Agent>();
        this.target.position = leader.position + omegaRs;
        this.target.orientation = leader.orientation + agent.orientation;
        return base.GetSteering(agent);

    } 
}
