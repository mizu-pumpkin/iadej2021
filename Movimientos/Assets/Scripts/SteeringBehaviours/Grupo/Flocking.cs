using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The flocking algorithm relies on blending three simple steering behaviors: move away from
// boids that are too close (separation), move in the same direction and at the same velocity as
// the flock (alignment and velocity matching), and move toward the center of mass of the flock
// (cohesion). The cohesion steering behavior calculates its target by working out the center of mass
// of the flock. It then hands off this target to a regular arrive behavior.
// For simple flocking, using equal weights may be sufficient. In general, however, separation
// is more important than cohesion, which is more important than alignment. The latter two are
// sometimes seen reversed.
public class Flocking : BlendedSteering
{

    public override void Awake()
    {
        base.Awake();

        //this.steeringBehaviours = new List<SteeringBehaviour>(this.GetComponents<SteeringBehaviour>());

        Separation separation = gameObject.AddComponent<Separation>();
        Cohesion cohesion = gameObject.AddComponent<Cohesion>();
        Alignment alignment = gameObject.AddComponent<Alignment>();
        VelocityMatching velocityMatching = gameObject.AddComponent<VelocityMatching>();

        separation.threshold = 5;
        separation.decayCoefficient = 20.0f;
        separation.weight = 10.0f;

        cohesion.threshold = 5;
        cohesion.weight = 1.1f;

        alignment.threshold = 5;
        alignment.weight = 1.0f;

        velocityMatching.timeToTarget = 0.1f;
        velocityMatching.weight = 1.0f;
        velocityMatching.target = gameObject.AddComponent<Agent>();

        this.behaviors.Add(separation);
        this.behaviors.Add(cohesion);
        this.behaviors.Add(alignment);
        this.behaviors.Add(velocityMatching);
    }

    override public Steering GetSteering(AgentNPC agent) {
        return base.GetSteering(agent);
    }

}
