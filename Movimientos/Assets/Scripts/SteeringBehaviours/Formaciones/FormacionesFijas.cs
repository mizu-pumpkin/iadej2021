using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormacionesFijas : Seek
{
    //Elección de lider
    public Agent lider;
    GameObject[] formacion;

    //Introducir en la lista aquellos que vayan a realizar la formación
    public override void Awake() {
        base.Awake();
        formacion = GameObject.FindGameObjectsWithTag("PFija");
    }

    override public Steering GetSteering(AgentNPC agent){

         float[,] omegaLider = new float[,] { { Mathf.Cos(lider.orientation), -1*Mathf.Sin(lider.orientation) }, { Mathf.Sin(lider.orientation), Mathf.Cos(lider.orientation)}};
         Vector3 rs = lider.position - agent.position; //Rs, distancia del agente con respecto al lider
         Vector3 omegaRs = new Vector3 ((omegaLider[0,0]* rs.x +omegaLider[0,1]*rs.x), rs.y, (omegaLider[1,0]* rs.z +omegaLider[1,1]*rs.z));
         Agent targetAgent = target.GetComponent<Agent>();
         this.target.position = lider.position + omegaRs;
         this.target.orientation = lider.orientation + agent.orientation;
         return base.GetSteering(agent);

     } 
}
