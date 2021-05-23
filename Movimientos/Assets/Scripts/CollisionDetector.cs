using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public Collision GetCollision(Vector3 position, Vector3 moveAmount) {
        // TODO
        // returns the first collision for the character if it begins at the given position
        // and moves by the given movement amount. Collisions in the same direction,
        // but farther than moveAmount, are ignored.
        // Typically, this call is implemented by casting a ray from
        // position to position + moveAmount
        // and checking for intersections with walls or other obstacles.

        return new Collision(Vector3.zero);
    }
}
