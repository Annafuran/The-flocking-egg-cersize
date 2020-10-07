using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }

    
    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
    }

    public void Move(Vector3 velocity)
    {
     
        if (velocity.magnitude < Mathf.Epsilon)
            return;

        // "Flatten" our velocity vector to ensure all movement is in the XZ plane
        velocity.y = 0;
        // The distance we wish to move our agent this frame (we use this to limit the raycast distance)
        float moveDistance = velocity.magnitude * Time.deltaTime;

        // Raycast from the current to the target position to check if we would collide with something along the way
        // If the raycast hit something, info about the intersection is contained in 'hit'
        if (Physics.Raycast(transform.position, velocity.normalized, out var hit, moveDistance))
        {
            // Reflect the velocity against the hit normal to get the direction the agent should move from the wall
            Vector3 reflectedDirection = Vector3.Reflect(velocity, hit.normal).normalized;
            // Move the agent to the collision point (with a small offset to prevent floating point precision issues)
            transform.position = hit.point + reflectedDirection * Mathf.Epsilon;
            // Update the velocity (set the direction to the reflected direction and subtract the distance we moved to the wall)
            velocity = reflectedDirection * (velocity.magnitude - hit.distance - Mathf.Epsilon);
            // Recursive call to keep checking for collisions until it's either fine to move or we've already moved the distance
            Move(velocity);
        }
        else
        {
            // The agent didn't collide with anything along the way, so it's fine to move
            transform.position += velocity * Time.deltaTime;
            // Agent should face the direction it is heading
            transform.forward = velocity;
        }

    }

}
