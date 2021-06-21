using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * L. Daniel Hernández. 2018. Copyleft
 * 
 * Una propuesta para dar órdenes a un grupo de agentes sin formación.
 * 
 * Recursos:
 * Los rayos de Cámara:
 *      https://docs.unity3d.com/es/current/Manual/CameraRays.html
 * "Percepción" mediante Physics.Raycast:
 *      https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
 * SendMessage to external functions:
 *      https://www.youtube.com/watch?v=4j-lh3C_w1Q
 * 
 * */

public class UnitsController : MonoBehaviour
{

    public static List<GameObject> selectedUnits;
    public static Vector3 clickedPoint;
    Agent target;
    Arrive steer;

    void Awake()
    {
        selectedUnits = new List<GameObject>();

        target = new GameObject("UnitsControllerTarget").AddComponent<Agent>();
        target.interiorRadius = 0.1f;
        target.exteriorRadius = 2;

        steer = new GameObject("UnitsControllerArrive").AddComponent<Arrive>();
        steer.target = target;
        steer.targetRadius = target.interiorRadius;
        steer.slowRadius = target.exteriorRadius;
    }

    void Update()
    {

        // Damos una orden cuando levantemos el botón del ratón.
        if (Input.GetMouseButtonUp(0))
        {
            // Comprobamos si el ratón golpea a algo en el escenario.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider != null)
            {

                // Si lo que golpea es un punto del terreno
                // da la orden a todas las unidades NPC seleccionadas
                if (hitInfo.collider.CompareTag("Terrain"))
                {
                    // Asigna al target la posición del punto del terreno
                    clickedPoint = hitInfo.point;
                    target.position = hitInfo.point;

                    // Llama al método denominado "NewTarget" en todas las unidades seleccionadas
                    for (int i = 0; i < selectedUnits.Count; i++) {
                        //selectedUnits[i].SendMessage("RemoveTarget", steer);
                        //selectedUnits[i].SendMessage("AddTarget", steer);
                        selectedUnits[i].SendMessage(
                            "SetTarget",
                            new Steering(Utils.PositionToAngle(clickedPoint), clickedPoint)
                        );
                    }
                }

                // Si lo que golpea es un NPC
                // lo añade/quíta de la lista de unidades seleccionadas
                if (hitInfo.collider.CompareTag("NPC"))
                {
                    // Sube dos niveles en la gerarquía del collider del NPC
                    GameObject npc = hitInfo.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject;

                    // Si ya está en la lista, lo deselecciona
                    if (selectedUnits.Contains(npc)) {
                        selectedUnits.Remove(npc);
                        npc.SendMessage("RemoveTarget", steer);//.SendMessage("InitializeSteerings");
                        print(npc.name+" deselected");
                    }
                    // Si no estaba en la lista, lo selecciona
                    else {
                        selectedUnits.Add(npc);
                        print(npc.name+" selected");
                    }
                }
                
            }
        }

        // Si se pulsa la tecla "space" deselecciona todas las unidades
        if (Input.GetKeyDown("space"))
        {
            List<GameObject> aux = new List<GameObject>(selectedUnits);
            foreach (GameObject npc in aux) {
                selectedUnits.Remove(npc);
                npc.SendMessage("RemoveTarget", steer);//.SendMessage("InitializeSteerings");
            }
            print("NPCs deselected");
        }
        
    }

}