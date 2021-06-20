using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFixed : FormationPattern
{
    public AgentNPC leader;
    
    public override DriftOffset getDriftOffset(List<SlotAssignment> slotAssignments)
    {
        //take the position of the leader, he will be the leftmost npc of all
        DriftOffset location = new DriftOffset();
        location.position.x = leader.position.x;
        location.position.z = leader.position.z;
        return location;
    }

    // Gets the location of the given slot index.
    public override DriftOffset getSlotLocation(int slotNumber)
    {
        // Create a location, and fill its components based
        // a line with separation 4 having as reference the axis (0,0), the leader will be (0,0), the first slot will be (4,0)...
        DriftOffset location = new DriftOffset();
        location.position.x = 4 + 4 * slotNumber;
        location.position.z = 0;
        return location;
    }

    // Makes sure we can support the given number of slots
    // In this case we support any number of slots.
    public override bool supportsSlots(int slotCount)
    {
        return true;
    }

    public override AgentNPC getAnchorPoint(List<SlotAssignment> slotAssignments)
    {
        return leader;
    }
}