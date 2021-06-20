using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : SteeringBehaviour
{
    //Elección de leader
    public Agent leader;
    public Vector3 posicionLeader;
    public float orientacionLeader;

    public override void Awake()
    {
        target = new GameObject().AddComponent<Agent>();
    }
    
    public void Update()
    {
        leader = UnitsController.leader;
    }

    public virtual void LateUpdate()
    {
        if (leader != null) {
            posicionLeader = leader.position;
            orientacionLeader = leader.orientation;
        }
    }
    
    override public Steering GetSteering(AgentNPC agent)
    {
        if (leader == null || (posicionLeader == leader.position && orientacionLeader == leader.orientation))
            return null;

        // Create the structure to hold our output
        Steering steer = new Steering();    
        
        float[,] omegaLeader = new float[,] {
            { Mathf.Cos(leader.orientation), -Mathf.Sin(leader.orientation) },
            { Mathf.Sin(leader.orientation), Mathf.Cos(leader.orientation) }
        };
        
        Vector3 rs = leader.position - agent.position; //Rs, distancia del agente con respecto al leader

        Vector3 omegaRs = new Vector3 (
            (omegaLeader[0,0]* rs.x +omegaLeader[0,1]*rs.x),
            rs.y,
            (omegaLeader[1,0]* rs.z +omegaLeader[1,1]*rs.z)
        );  

        steer.linear = leader.position + omegaRs;
        float ws = leader.orientation - agent.orientation;
        steer.angular = leader.orientation + ws;

        return steer;
    }
    
}