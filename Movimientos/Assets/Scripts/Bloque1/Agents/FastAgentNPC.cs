using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastAgentNPC : AgentNPC
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    public List<string> favoriteTerrain;
    public List<string> difficultTerrain;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */
    
    public override void Awake()
    {
        base.Awake();
        this.favoriteTerrain = new List<string>();
        this.difficultTerrain = new List<string>();
        this.mass = 10;
        this.maxSpeed = 8;
        this.maxAcceleration = 16;
    }

    public override void LateUpdate()
    {
        steerings = new List<Steering>();
        Steering accum = new Steering();

        // TODO: Arbitro
        foreach (SteeringBehaviour sb in this.steeringBehaviours) {
            Steering steer = sb.GetSteering(this);
            if (steer == null) continue;

            if (sb is WallAvoid) {
                steerings = new List<Steering>();
                steerings.Add(steer);
                //accum = steer;
                break;
            }

            steer.linear *= sb.priority;
            steer.angular *= sb.priority;
            steerings.Add(steer);

            accum.linear += steer.linear; // FIXME: sb.weight
            accum.angular += steer.angular; // FIXME: sb.weight
        }

        //steerings.Add(accum);
    }

}
