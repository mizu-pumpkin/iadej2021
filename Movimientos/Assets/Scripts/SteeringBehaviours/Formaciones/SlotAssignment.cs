using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds the assignment of a single character to a slot
// Cada ranura se identifica con un número y un personaje
public class SlotAssignment
{
    public AgentNPC character;
    public int slotNumber;

    public SlotAssignment()
    {
        this.character = null;
        this.slotNumber = 0;
    }

}
