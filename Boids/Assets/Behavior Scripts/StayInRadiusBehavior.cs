using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/StayInRadius")]
public class StayInRadiusBehavior : FlockBehavior
{
    public Vector3 center;
    public float radius = 5f;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector3 centerOffset = center - agent.transform.position;
        centerOffset = new Vector3(centerOffset.x, 0.0f, centerOffset.z);
        float t = centerOffset.magnitude / radius;
       
        if(t < 0.9)
        {
            return Vector3.zero;
        }

        return centerOffset * t * t;
    }
}
