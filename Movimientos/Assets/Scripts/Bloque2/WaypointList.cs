using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FIXME: esta interfaz puede que no sirva de nada
public abstract class WaypointList : MonoBehaviour
{
    // Initializes the iterator to move in order of increasing value
    public abstract void sort();

    // Initializes the iterator to move in order of decresing value
    public abstract void sortReversed();

    // Returns a new waypoint list containing those waypoints
    // that are near to the given one
    public abstract Waypoint getNearby(Waypoint waypoint);

    // Return the next waypoint in the iteration. Iterations
    // are initialized by a call to one of the sort functions.
    // Note that this function must work in such a way that
    // remove() can be called between calls to next() without
    // causing problems.
    public abstract Waypoint next();

    // Removes the given waypoint from the list
    public abstract void remove(Waypoint waypoint);
}
