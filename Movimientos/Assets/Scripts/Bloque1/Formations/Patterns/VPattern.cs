using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VPattern : FormationPattern
{
    // The radius of one character, this is needed to determine how
    // close we can pack a given number of characters around a cirle
    public float separation = 1;
    
    public override DriftOffset getDriftOffset(List<SlotAssignment> slotAssignments)
    {
        numberOfSlots = slotAssignments.Count;
        
        AgentNPC anchor = slotAssignments[0].character;

        DriftOffset location = new DriftOffset();
        location.position.x = anchor.position.x;
        location.position.z = anchor.position.z;
        return location;
    }

    // Gets the location of the given slot index.
    public override DriftOffset getSlotLocation(int slotNumber)
    {
        int relPos = (int) (1 + slotNumber) / 2;
        // Create a location, and fill its components based
        // on the character radius that spaces the characters
        DriftOffset location = new DriftOffset();
        if (slotNumber % 2 == 0)
            location.position.x = -separation * relPos;
        else
            location.position.x = separation * relPos;
        location.position.z = -separation * relPos;


        // The characters should be facing "up"
        location.orientation = 0;

        // Return the slot location
        return location;
    }

}