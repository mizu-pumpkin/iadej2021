using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    
    public static Vector3 OrientationToVector(float orientation)
    {
        Vector3 vector = Vector3.zero;
        vector.x = Mathf.Sin(orientation * Mathf.Deg2Rad) * 1.0f; // ???: cos?
        vector.z = Mathf.Cos(orientation * Mathf.Deg2Rad) * 1.0f; // ???: sin?
        return vector;
    }

    public static float PositionToAngle(Vector3 position)
    {
        return Mathf.Atan2(position.x, position.z) * Mathf.Rad2Deg;
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

    public static float rango360(float orientation) {
        if (orientation < 0)
            orientation += 360;
        else if (orientation > 360)
            orientation -= 360;
        
        return orientation;
    }

    public static Vector3 getPosition(float theta, Vector3 rs)
    // rs       localización de la ranura S en el juego con respecto al leader
    // theta    orientación del leader
    {
        // Get the orientation of the anchor point as a matrix
        float[] Rtheta = new float[4] {
            Mathf.Cos(theta), -Mathf.Sin(theta),
            Mathf.Sin(theta), Mathf.Cos(theta)
        };

        float x = Rtheta[0] * rs.x + Rtheta[1] * rs.z;
        float z = Rtheta[2] * rs.x + Rtheta[3] * rs.z;

        return new Vector3(x, 0, z);
    }
    
}
