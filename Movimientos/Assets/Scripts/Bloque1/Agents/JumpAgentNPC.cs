using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAgentNPC : AgentNPC
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
        this.mass = 5;
        this.maxSpeed = 4;
        this.maxAcceleration = 8;
    }

    public override void LateUpdate()
    {
        steerings = new List<Steering>();

        // TODO: Arbitro
        foreach (SteeringBehaviour sb in this.steeringBehaviours) {
            Steering steer = sb.GetSteering(this);
            if (steer == null) continue;

            if (sb is WallAvoid) continue; // HACK: para que salte los muros

            steer.linear *= sb.weight;
            steer.angular *= sb.weight;
            steerings.Add(steer);
        }

        //Steering accum = new Steering();
        //foreach (SteeringBehaviour sb in this.steeringBehaviours) {
        //    Steering steer = sb.GetSteering(this);
        //    if (steer == null) continue;
        //    if (sb is WallAvoid) continue; // HACK: para que salte los muros
        //    accum.linear += steer.linear * sb.weight;
        //    accum.angular += steer.angular * sb.weight;
        //}
        //accum.linear = Mathf.Max(accum.linear, this.maxAcceleration);
        //accum.angular = Mathf.Max(accum.angular, this.maxRotation);
        //steerings.Add(accum);
    }

}
