using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PrioritySteering
{
    public float Epsilon = 20.0f;
    
    //Returns the acceleration required
    public Steering getSteering(Dictionary<int, List<Steering>> groups)
    {
        if (groups == null) return null;
        
        Steering steering = new Steering();

        // Go through each group
        foreach (List<Steering> group in groups.Values)
        {
            // Create the steering structure for accumulation
            //steering = new Steering();
            foreach (Steering singleSteering in group) {
                if (singleSteering == null) continue;

                steering.linear += singleSteering.linear;
                steering.angular += singleSteering.angular;
            }

            // Check if we´re above the threshold, if so return
            if (steering.linear.magnitude > Epsilon ||
                Mathf.Abs(steering.angular) > Epsilon) {
                return steering;
            }
        }

        /*
            public Steering getSteering(List<SteeringBehaviour> stBehaviour){
                
                Steering steering = new Steering();
                foreach (SteeringBehaviour sb in this.stBehaviour){
                    
                    steering += sb.peso*



                    
                 }


            }
        


        */

        // If we get here, it means that no group had a large
        // enough acceleration, so return the small
        // acceleration from the final group
        return steering;
    }
   
}
