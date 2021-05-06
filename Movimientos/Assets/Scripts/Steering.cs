using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */

    public float angular; // velocidad/aceleracion angular
    public Vector3 linear; // velocidad/aceleracion lineal

    /*
        █▀▀ █▀█ █▄░█ █▀ ▀█▀ █▀█ █░█ █▀▀ ▀█▀ █▀█ █▀█ █▀
        █▄▄ █▄█ █░▀█ ▄█ ░█░ █▀▄ █▄█ █▄▄ ░█░ █▄█ █▀▄ ▄█
     */

    // Establece a valores nulos el valor del steering
    public Steering()
    {
        this.angular = 0;
        this.linear = Vector3.zero;
    }

    public Steering(float angular, Vector3 linear)
    {
        this.angular = angular;
        this.linear = linear;
    }

}
