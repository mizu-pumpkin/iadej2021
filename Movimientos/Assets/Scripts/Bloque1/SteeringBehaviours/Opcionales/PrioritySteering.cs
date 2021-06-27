using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PrioritySteering : MonoBehaviour
{
    public bool reverseMode;
    public Dictionary<int, List<SteeringBehaviour>> groups;

    public void InitializeGroups(List<SteeringBehaviour> behaviours)
    {
        // Inicialización del diccionario de prioridades
        groups = new Dictionary< int, List<SteeringBehaviour> >();

        foreach (SteeringBehaviour sb in behaviours)
        {
            if (!groups.ContainsKey(sb.priority))
                groups.Add(sb.priority, new List<SteeringBehaviour>());
            
            groups[sb.priority].Add(sb);
        }
    }

    public Steering GetSteering(AgentNPC agent)
    {
        
        // Create the steering structure for accumulation
        Steering steering = new Steering();

        List<int> sortedKeys = new List<int>(groups.Keys);
        sortedKeys.Sort();
        if (reverseMode) sortedKeys.Reverse();

        // Go through each group
        foreach (int i in sortedKeys)
        {
            List<SteeringBehaviour> group = groups[i];

            foreach (SteeringBehaviour sb in group)
            {
                if (sb == null) continue;

                Steering singleSteering = sb.GetSteering(agent);

                if (singleSteering != null)
                {
                    steering.linear += singleSteering.linear;
                    steering.angular += singleSteering.angular;
                }
            }

            // Check if we´re above the threshold, if so return
            if (steering.linear.magnitude > agent.maxSpeed ||
                Mathf.Abs(steering.angular) > agent.maxRotation)
            {
                return steering;
            }
        }
        return steering;
    }

}