using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public List<FlockAgent> agents = new List<FlockAgent>();
    float enemyAngleDir =180f; //Angle of cone that enemy can look. 
    float dirToAgent;
    public float speed = 7.0f; //speed of our enemy
    public float targetVisionLength = 15.0f;
    bool targetFound = false; //If we have found a target;
    FlockAgent target; //Our target
    private Vector3 movementDirection;
    private Vector3 movementPerSecond;
    private float latestDirectionChangeTime;
    public float directionChangeTime = 2.0f;
    Vector3 prevPosition = Vector3.zero;
    public Animator anim;


    /*
*/
    

    void Start()
    {
        agents = Flock.GetAgentList();
        latestDirectionChangeTime = 0f;
        NewMovementVector();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        agents = Flock.GetAgentList();

        if (transform.position.y != 0.6f)
        {
            transform.position = new Vector3(transform.position.x, 0.6f, transform.position.z);
        }

        if (prevPosition != Vector3.zero)
        {
            Vector3 movementDir = transform.position - prevPosition;
            movementDir = new Vector3(movementDir.x, 0.0f, movementDir.z);
            //transform.forward = new Vector3(movementDir.x, 0.0f, movementDir.z);

            Move(movementDir);
            anim.SetInteger("Walk", 1);
        }

        prevPosition = transform.position;
        
        //Find closest chicken
        if (!targetFound)
        {
            RandomMovement();
            foreach (FlockAgent agent in agents)
            {
  
                if (agent == agents[0])
                {
                    //Beräkna längden till hönan
                    dirToAgent = Vector3.Distance(agent.transform.position, this.transform.position);

                    // Beräkna vinkeln mellan vargens blick och riktningen till hönan
                    float viewAngle = Vector3.Angle(this.transform.forward, agent.transform.position);

                   if(viewAngle < (enemyAngleDir / 2) && dirToAgent < targetVisionLength)
                    {
                        target = agent;
                        targetFound = true;
                    }
                }
                else
                {
                    float temp = Vector3.Distance(agent.transform.position, this.transform.position);
                    float viewAngle = Vector3.Angle(this.transform.forward, agent.transform.position);

                    //If the hen is closer and inside viewangle
                    if(temp < dirToAgent && viewAngle < (enemyAngleDir / 2) && temp < targetVisionLength)
                    {
                        dirToAgent = temp;
                        target = agent;
                        targetFound = true;
                    }
                }
            }
        }
        else
        {
            HuntingMovement(target, agents); 
        }  
    }

    void Move(Vector3 velocity)
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

    //Create random direction vector. 
    void NewMovementVector()
    {
        movementDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)).normalized;
        movementPerSecond = movementDirection * speed;
    }



    //If no chicken is found, move randomly
    void RandomMovement()
    {
        Debug.Log("Random");

        if(Time.time - latestDirectionChangeTime > directionChangeTime)
        {
            latestDirectionChangeTime = Time.time;
            NewMovementVector();
        }

        /* transform.position = new Vector3(
             transform.position.x + (movementPerSecond.x * Time.deltaTime),
             0.0f,
             transform.position.z + (movementPerSecond.z * Time.deltaTime));*/

        Move(movementPerSecond);

    }

    //If chicken is found, hunt
    void HuntingMovement(FlockAgent agent, List<FlockAgent> agents)
    {
        Debug.Log("HUNT");
        float step = 5 * Time.deltaTime;

        Vector3 MoveTowards = Vector3.MoveTowards(transform.position, agent.transform.position, step);
        transform.position = new Vector3(MoveTowards.x, 0.0f, MoveTowards.z);
        anim.SetInteger("Walk", 1);

        if (Vector3.Distance(transform.position, agent.transform.position) < 3.0f && targetFound)
        {
            AttackMovement(agent, agents);
        }
    }

    //If chicken is close, attack
    void AttackMovement(FlockAgent agent, List<FlockAgent> agents)
    {
        Debug.Log("Attack");
        anim.SetInteger("Walk", 1);

        Destroy(agent.gameObject);
        agents.Remove(agent);
        targetFound = false;
    
       
    }

}
