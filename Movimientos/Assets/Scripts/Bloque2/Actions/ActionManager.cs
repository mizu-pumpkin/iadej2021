using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Permite el uso de:
    - acciones primitivas (estado, animacion, movimiento y peticion de IA)
    - combinaciones de acciones ejecutable a la vez
    - secuencias de acciones
Las combinaciones pueden incluir secuencias y viceversa
Las acciones tienen prioridad y tiempo de caducidad
Las acciones tienen métodos que:
    - comprueban si ha acabado
    - comprueban si se pueden ejecutar a la vez que la accion X
    - informa si se puede interrumpir acciones en ejecucion
*/
public class ActionManager : MonoBehaviour
{
    // Holds the queue of pending actions
    public List<Action> queue;

    // The currently executing actions
    public List<Action> actives;

    // The current time, a simple counter in this case
    public int currentTime = 0;

    // Adds an action to the queue
    public void ScheduleAction(Action action)
    {
        // Add it to the queue
        queue.Add(action);
    }

    // Processes the manager
    public void Execute()
    {
        // Update the time
        currentTime += 1;

        // Go through the queue to find interrupters
        foreach (Action action in queue)
        {
            // If we drop below active priority, give up
            if (action.priority <= GetHighestPriority(actives))
                break;
            
            if (action.CanInterrupt()) {
                actives = new List<Action>();
                actives.Add(action);
            }
        }

        // Try to add as many actions to active set as possible
        foreach (Action action in new List<Action>(queue))
        {
            // Check if the actionhas times out
            if (action.expiryTime < currentTime)
                // Remove it from the queue
                queue.Remove(action);
            
            // Check if we can combine
            bool canCombine = true;
            foreach(Action activeAction in actives) {
                if (!action.CanDoBoth(activeAction)) {
                    canCombine = false;
                    break;
                }
            }
            if (!canCombine) continue;

            // Move the action to the active set
            queue.Remove(action);
            actives.Add(action);
        }

        // Process the active set
        foreach (Action activeAction in new List<Action>(actives))
        {
            // Remove completed actions
            if (activeAction.IsComplete())
                actives.Remove(activeAction);
            // Execute others
            else
                activeAction.Execute();
        }
    }

    public int GetHighestPriority(List<Action> actions)
    {
        int highest = 0;
        foreach(Action action in actions)
            if (action.priority > highest)
                highest = action.priority;
        
        return highest;
    }

}
