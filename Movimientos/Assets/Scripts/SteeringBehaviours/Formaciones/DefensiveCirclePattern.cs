using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveCirclePattern : FormationPattern
{
    // The radius of one character, this is needed to determine how
    // close we can pack a given number of characters around a cirle
    public float characterRadius;

    // Calculates the number of slots in the pattern from the
    // assignment data. This is not part of the formation
    // pattern interface
    public int calculateNumberOfSlots(List<SlotAssignment> slotAssignments)
    {
        // Find the number of filled slots: it will be the
        // highest slot number in the assignments
        int filledSlots = 0;
        foreach (SlotAssignment assignment in slotAssignments)
        {
            // ???: if (assignment.slotNumber >= maxSlotNumber)
            if (assignment.slotNumber >= filledSlots)
                filledSlots = assignment.slotNumber;
        }

        // Add one to go from the index of the highest slot to the
        // number of slots needed
        numberOfSlots = filledSlots +1;

        return numberOfSlots;
    }

    // Calculates the drift offset of the pattern
    public override DriftOffset getDriftOffset(List<SlotAssignment> slotAssignments)
    {
        // Store the center of mass
        DriftOffset center = new DriftOffset();

        // Now go through each assignment, and add its
        // contribution to the center.
        foreach (SlotAssignment assignment in slotAssignments)
        {
            DriftOffset location = getSlotLocation(assignment.slotNumber);
            center.position += location.position;
            center.orientation += location.orientation;
        }
    
        // Divide through to get the drift offset.
        int numberOfAssignments = slotAssignments.Count;
        center.position /= numberOfAssignments;
        center.orientation /= numberOfAssignments;
        return center;
    }

    // Calculates the position of a slot.
    public override DriftOffset getSlotLocation(int slotNumber)
    {
        // We place the slots around a circle based on their
        // slot number
        float angleAroundCircle = slotNumber / numberOfSlots * Mathf.PI * 2;
    
        // The radius depends on the radius of the character,
        // and the number of characters in the circle:
        // we want there to be no gap between character's shoulders.
        float radius = characterRadius / Mathf.Sin(Mathf.PI / numberOfSlots);
    
        // Create a location, and fill its components based
        // on the angle around circle.
        DriftOffset location = new DriftOffset();
        location.position.x = radius * Mathf.Cos(angleAroundCircle);
        location.position.z = radius * Mathf.Sin(angleAroundCircle);
        
        // The characters should be facing out
        location.orientation = angleAroundCircle;

        // Return the slot location
        return location;
    }

    // Makes sure we can support the given number of slots
    // In this case we support any number of slots.
    public override bool supportsSlots(int slotCount)
    {
        return true;
    }

}