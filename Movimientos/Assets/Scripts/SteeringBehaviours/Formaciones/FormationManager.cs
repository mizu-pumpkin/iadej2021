using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoBehaviour
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */

    // Holds a list of slots assignments
    // Lista de ranuras
    public List<SlotAssignment> slotAssignments;

    // Holds a Static structure (i.e., position and orientation)
    // representing the drift offset for the currently filled slots
    // Posicion y orientacion para evitar los derrapes
    public DriftOffset driftOffset;

    // Holds the formation pattern
    // Referencia al patron de la formacion
    // P.e. una malla estática rectangular, una escalable cirular, ...
    public FormationPattern pattern;

    public List<AgentNPC> npcs; // HACK: tmp

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */

    public void Awake()
    {
        slotAssignments = new List<SlotAssignment>();

        foreach (AgentNPC npc in npcs) addCharacter(npc); // HACK: tmp
        //foreach (GameObject go in UnitsController.selectedUnits) {
        //    AgentNPC npc = go.GetComponent<AgentNPC>();
        //    addCharacter(npc);
        //}
    }

    public void Update()
    {
        if (Input.GetKeyDown("u")) // HACK
            updateSlots();
    }
    
    // Updates the assignment of characters to slot
    public void updateSlotAssignments()
    {
        // A very simple assignment algorithm: we simply go through
        // each assignment in the list and assign squential slot numbers
        // Ejemplo SIMPLE de actualizacion de slot a personajes
        for (int i=0; i < slotAssignments.Count; i++) {
            SlotAssignment slotAssignment = slotAssignments[i];
            slotAssignment.slotNumber = i;
        }

        // Update the drift offset
        // Calcular la posicion y orientacion para evitar los derrapes
        // en funcion de las ranuras ocupadas
        driftOffset = pattern.getDriftOffset(slotAssignments);
    }

    // Add a new character to the first avaible slot.
    // Returns false if no more slots are avaible.
    public bool addCharacter(AgentNPC character)
    {
        // Find out how many slots we have occupied 
        int occupiedSlots = slotAssignments.Count;

        // Check if the pattern supports more slots
        if (pattern.supportsSlots(occupiedSlots + 1))
        {    
            // Add a new slot assignment 
            SlotAssignment slotAssignment = new SlotAssignment();
            slotAssignment.character = character;
            slotAssignments.Add(slotAssignment);

            // Update the slot assignments and return success
            updateSlotAssignments();
            return true;
        }

        // Otherwise we've failed to add the character
        return false;
    }

    // Removes a character from its slot.position
    public void removeCharacter(AgentNPC character)
    {
        // Find the character's slot
        SlotAssignment slot = findSlot(character);

        // Make sure we've found a valid result
        if (slot != null && slotAssignments.Contains(slot))
        {
            // Remove the slot
            slotAssignments.Remove(slot);
            
            // Update the slot assignments
            updateSlotAssignments();
        }
    }

    //Write new slot locations to each character
    public void updateSlots()
    {
        //Find the anchor point
        AgentNPC anchor = pattern.getAnchorPoint(slotAssignments);
        if (anchor == null)
            return;

        // Get the orientation of the anchor point as a matrix
        float[] orientationMatrix = new float[4] {
            Mathf.Cos(anchor.orientation), -Mathf.Sin(anchor.orientation),
            Mathf.Sin(anchor.orientation), Mathf.Cos(anchor.orientation)
        };

        // Go through each character in turn
        for (int i = 0; i < slotAssignments.Count; i++)
        {
            //Ask for the location of the slot relative to the
            //anchor point. This should be a Static structure
            DriftOffset relativeLoc = pattern.getSlotLocation(slotAssignments[i].slotNumber);

            //Transform it by the anchor point’s position and
            //orientation
            DriftOffset location = new DriftOffset();
            location.position = MatrixProduct(relativeLoc.position, orientationMatrix) + anchor.position;
            location.orientation = anchor.orientation + relativeLoc.orientation;

            //And add the drift component
            location.position -= driftOffset.position;
            location.orientation -= driftOffset.orientation;
            
            //Write the static to the character
            slotAssignments[i].character.SetTarget(//location);
                new Steering(location.orientation, location.position)
            );
        }

    }

    /*
        ▄▀█ █░█ ▀▄▀
        █▀█ █▄█ █░█
     */

    SlotAssignment findSlot(AgentNPC npc)
    { // FIXME
        foreach (SlotAssignment sa in slotAssignments)
            if (sa.character == npc)
                return sa;
        return null;
    }

    public Vector3 MatrixProduct(Vector3 relativeLocPosition, float[] orientationMatix)
    { // FIXME
        return new Vector3(
            orientationMatix[0] * relativeLocPosition.x + orientationMatix[2] * relativeLocPosition.z,
            0,
            orientationMatix[1] * relativeLocPosition.x + orientationMatix[3] * relativeLocPosition.z
        );
    }

}