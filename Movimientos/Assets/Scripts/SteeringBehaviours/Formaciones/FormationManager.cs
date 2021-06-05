using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager //: MonoBehaviour
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */
    
    // Holds the assignment of a single character to a slot
    public struct SlotAssignment{
        Agent character;
        public float slotNumber;
    }

    // Holds a list of slots assignments
    public List<SlotAssignment> slotAssignments;

    // Holds a Static structure (i.e., position and orientation)
    // representing the drift offset for the currently filled slots
    public struct driftOffset{
        Vector3 position;
        float Orientation;
    }

    // Holds the formation pattern
     FormationPattern pattern;

    /*
        █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█
     */
/*
    // Updates the assignment of characters to slot
    public void updateSlotAssignments(){

        // A very simple assignment algorithm: we simply go through
        // each assignment in the list and assign squential slot
        // number
        foreach (int i in slotAssignments.length()){
            slotAssignments[i].slotNumber = i;
        }

        // Update the drift offset
        float driffOffset = pattern.getDriftOffset(SlotAssignments); //?? Creo que es un float 
    }

    // Add a new character to the first avaible slot. Returns 
    // false if no more slots are avaible.
    public bool addCharacter(Agent character){

        // Find out how many slots we have occupied 
        int occupiedSlots = slotAssignments.length();

        // Check if the pattern supports more slots
        if (pattern.supportsSlots(occupiedSlots + 1)){
            
            // Add a new slot assignment 
            slotAssignment = new SlotAssignment();
            slotAssignment.character = character;
            slotAssignments.append(slotAssignment);

            // Update the slot assignments and return success
            updateSlotAssignments();
            return true;
        }

        // Otherwise we've failed to add the character
        return false;
    }

    // Removes a character from its slot.position
    public void removeCharacter(Agent character){

        //SlotAssignment s;
        // Find the character's slot 

        SlotAssignment slot = charactersInSlots.find(character);

        // Make sure we've found a valid result
        if (slotAssignments.contains(slot)){
            // Remove the slot
            slotAssignments.removeElement(slot);
            
            // Update the slot assignments
            updateSlotAssignments();
        }
    }

    //TODO
    public Vector3 getAnchorPoint(){

    }

    //Write new slot locations to each character
    public void updateSlots(){
        //Find the anchor point
        Vector3 anchor = getAnchorPoint();

        // Get the orientation of the anchor point as a matrix
         int[,] orientationMatrix = new int[,] {{Math.Cos(character), -1*Math.Sin(character)}, {Math.Sin(character), Math.Cos(character)}};

        // Go through each character in turn
        for (int i in slotAssignments.length()){

            //Ask for the location of the slot relative to the
            //anchor point. This should be a Static structure
            relativeLoc = pattern.getSlotLocation(slotAssignments[i].slotNumber)

            //Transform it by the anchor point’s position and
            //orientation
            location = new Static()
            location.position = relativeLoc.position * orientationMatrix + anchor.position;
            location.orientation = anchor.orientation + relativeLoc.orientation;

            //And add the drift component
            location.position -= driftOffset.position
            location.orientation -= driftOffset.orientation

            //Write the static to the character
            slotAssignments[i].character.setTarget(location)
        }
    }
*/
}
