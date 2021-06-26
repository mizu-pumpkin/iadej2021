
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFollowing : Seek
{
    public int predictTime = 1;
    public float distancia = 3;
    
    [SerializeField]
    private List<GameObject> walls;
    private List<Collider> colliderList;

    public override void Awake()
    {
        base.Awake();
        target = new GameObject("WallFollowingTarget").AddComponent<Agent>();
        colliderList = new List<Collider>();

        foreach (GameObject wall in walls)
        {
            Collider collider = wall.GetComponent<Collider>();
            if (collider!=null) colliderList.Add(collider);
        }
    }

    override public Steering GetSteering(AgentNPC agent)
    {
        Vector3 futurePos = agent.position + agent.velocity * predictTime;

        GameObject closestWall = null;
        float minDistance = Mathf.Infinity;
        Vector3 closestWallPoint = Vector3.zero;

        for (int i = 0; i< colliderList.Count; i++)
        {
            Vector3 closestPoint = colliderList[i].ClosestPoint(futurePos);

            float distance = (futurePos - closestPoint).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                closestWallPoint = closestPoint;
                closestWall = walls[i];
            }
        }

        Vector3 normal = Vector3.zero;
        Vector3 direction = closestWall.transform.position - futurePos;
        
        RaycastHit hit;
        if (Physics.Raycast(futurePos, direction, out hit))
            normal = hit.normal;

        normal = closestWallPoint + normal * distancia;
        normal.y = 0;
        target.position = normal;

        return base.GetSteering(agent);
    }
}
