using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePattern : FormationPattern
{
    // The radius of one character, this is needed to determine how
    // close we can pack a given number of characters around a cirle
    public float separation = 1;
    
    public override DriftOffset getDriftOffset(List<SlotAssignment> slotAssignments)
    {
        numberOfSlots = slotAssignments.Count;
        
        AgentNPC leftMost = slotAssignments[0].character;

        DriftOffset location = new DriftOffset();
        location.position.x = leftMost.position.x;
        location.position.z = leftMost.position.z;
        return location;
    }

    // Gets the location of the given slot index.
    public override DriftOffset getSlotLocation(int slotNumber)
    {
        // Create a location, and fill its components based
        // on the character radius that spaces the characters
        DriftOffset location = new DriftOffset();
        location.position.x = separation * slotNumber;
        location.position.z = 0;

        // The characters should be facing "up"
        location.orientation = 0;

        // Return the slot location
        return location;
    }
    
}