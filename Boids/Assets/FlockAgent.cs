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
        velocity = velocity * 0.8f;
        //turn our agent so its facing the direction it's toward
        transform.forward = new Vector3(velocity.x, 0.0f, velocity.z);
        //move it
        transform.position += new Vector3(velocity.x, 0.0f, velocity.z) * Time.deltaTime;
    }

}
