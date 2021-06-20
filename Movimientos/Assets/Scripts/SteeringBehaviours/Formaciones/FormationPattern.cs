using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cada patrón particular (V, Cuña, Círculo, ...) necesita su propia
// instancia de clase que implementa la siguiente interfaz
public abstract class FormationPattern : MonoBehaviour
{
    // Holds the number of slots currently in the
    // pattern This is updated in the getDriftOffset
    // method It may be a fixed value.
    public int numberOfSlots;

    // Calculates the drift offset when characters are in
    // given set of slots
    public abstract DriftOffset getDriftOffset(List<SlotAssignment> slotAssignments);

    // Gets the location of the given slot index.
    public abstract DriftOffset getSlotLocation(int slotNumber);

    // Returns true if the pattern can support the given
    // number of slots
    public abstract bool supportsSlots(int slotCount);

    public abstract AgentNPC getAnchorPoint(List<SlotAssignment> slotAssignments);

}
