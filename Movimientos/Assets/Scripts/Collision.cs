using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision
{
    /*
        █▀█ █▀█ █▀█ █▀█ █▀▀ █▀█ ▀█▀ █ █▀▀ █▀
        █▀▀ █▀▄ █▄█ █▀▀ ██▄ █▀▄ ░█░ █ ██▄ ▄█
     */

    public Vector3 position; // Punto de colisión
    //public Vector3 normal;
    public Vector3 normal() { // Vector normal en el punto de colisión
        return position.normalized;
    }

    /*
        █▀▀ █▀█ █▄░█ █▀ ▀█▀ █▀█ █░█ █▀▀ ▀█▀ █▀█ █▀█ █▀
        █▄▄ █▄█ █░▀█ ▄█ ░█░ █▀▄ █▄█ █▄▄ ░█░ █▄█ █▀▄ ▄█
     */

    public Collision(Vector3 position)
    {
        this.position = position;
    }

}