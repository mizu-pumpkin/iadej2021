using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    
    public static Vector3 OrientationToVector(float orientation)
    {
        Vector3 vector = Vector3.zero;
        vector.x = Mathf.Sin(orientation * Mathf.Deg2Rad) * 1.0f;
        vector.z = Mathf.Cos(orientation * Mathf.Deg2Rad) * 1.0f;
        return vector;
    }
    
    public static float PositionToAngle(Vector3 position)
    {
        return Mathf.Atan2(position.x, position.z) * Mathf.Rad2Deg; //(x,z)
    }

    // This function helps in finding the actual direction of rotation
    // after two orientation values are subtracted
    public static float MapToRange(float rotation) {
        rotation %= 360;
        if (Mathf.Abs(rotation) > 180) {
            if (rotation < 0)
                rotation += 360;
            else
                rotation -= 360;
        }
        return rotation;
    }

    public static float rangoPI(float orientation) {
        if (orientation < 0)
            orientation += 360;
        else if (orientation > 360)
            orientation -= 360;
        
        return orientation;
    }
    
}
