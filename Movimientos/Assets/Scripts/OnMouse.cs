using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouse : MonoBehaviour
{
    // https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseEnter.html

    public Renderer rend;
    public bool seleccionado = false;

    void Start()
    {
        Cursor.visible = true;
        rend = GetComponent<Renderer>();
    }

    // The mesh goes red when the mouse is over it...
    void OnMouseEnter()
    {
        if(seleccionado==false)
           rend.material.color = new Color(1, 0, 0);  //Color.red;
    }

    // ...and the mesh finally turns white when the mouse moves away.
    void OnMouseExit()
    {
        if (seleccionado==false)
            rend.material.color = Color.white;
    }

    void OnMouseDown()
    {
        if (seleccionado == false)
        {
            seleccionado = true;
            rend.material.color = new Color(0, 1, 0);
        } else
        {
            seleccionado = false;
            rend.material.color = new Color(1, 0, 0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            seleccionado = false;
            rend.material.color = Color.white;
        }
    }

}
