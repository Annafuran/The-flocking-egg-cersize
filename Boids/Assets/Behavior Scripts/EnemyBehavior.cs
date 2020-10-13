using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
    public List<FlockAgent> agents = new List<FlockAgent>();
    float enemyAngleDir =180f; //Angle of cone that enemy can look. 
    float dirToAgent;
    public float speed = 7.0f; //speed of our enemy
    public float targetVisionLength = 15.0f;
    bool targetFound = false; //If we have found a target;
    FlockAgent target; //Our target
    Vector3 velocity;
    private float latestDirectionChangeTime;
    public float directionChangeTime = 2.0f;
    Vector3 prevPosition = Vector3.zero;
    public Animator anim;
    bool isDogSpawned = false;


    public float curHealth = 0;
    public float maxHealth = 100;

    public HealthBar healthBar;
    public GameObject HP;

    void Start()
    {
        gameObject.SetActive(false);
        HP.GetComponent<GameObject>();
        HP.SetActive(false);
        agents = Flock.GetAgentList();
        latestDirectionChangeTime = 0f;
        NewMovementVector();
        curHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        agents = Flock.GetAgentList();

        if (curHealth > 0) {
            curHealth -= 0.01f;
            healthBar.SetHealth(curHealth);
        }
            

        if (transform.position.y != 0.2f)
        {
            transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
        }

        if (prevPosition != Vector3.zero && targetFound)
        {
            Vector3 movementDir = transform.position - prevPosition;
            movementDir = new Vector3(movementDir.x, 0.0f, movementDir.z);
            
            Move(movementDir);
            anim.SetInteger("Walk", 1);
        }
        //transform.position = Vector3.Lerp(prevPosition, transform.position, 0.5f);
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
        else if(curHealth < 50 && targetFound)
        {
            HuntingMovement(target, agents); 
        }
        else
        {
            RandomMovement();
        }
    }

    void Move (Vector3 velocity)
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
        var newDir = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up) * Vector3.forward;
        transform.rotation = Quaternion.LookRotation(newDir, Vector3.up);
    }



    //If no chicken is found, move randomly
    void RandomMovement()
    {
        anim.SetInteger("Walk", 1);

        if (Time.time - latestDirectionChangeTime > directionChangeTime)
        {
            latestDirectionChangeTime = Time.time;
            NewMovementVector();
        }

        Move(transform.forward * speed);
    }

    //If chicken is found, hunt
    void HuntingMovement(FlockAgent agent, List<FlockAgent> agents)
    {
        float step = 5 * Time.deltaTime;

        Vector3 MoveTowards = Vector3.MoveTowards(transform.position, agent.transform.position, step);
        transform.position = new Vector3(MoveTowards.x, 0.2f, MoveTowards.z);
        anim.SetInteger("Walk", 1);

        if (Vector3.Distance(transform.position, agent.transform.position) < 3.0f && targetFound)
        {
            AttackMovement(agent, agents);
        }
    }

    //If chicken is close, attack
    void AttackMovement(FlockAgent agent, List<FlockAgent> agents)
    {
        anim.SetInteger("Walk", 1);
        SoundManagerScript.PlaySound("enemyEat");
        Destroy(agent.gameObject);
        agents.Remove(agent);
        targetFound = false;
        curHealth = maxHealth;
        healthBar.SetHealth(curHealth);
    }

    //Spawn the dog
    public void spawnDog() {
        if (isDogSpawned == false)
        {
            gameObject.SetActive(true);
            HP.SetActive(true);
            isDogSpawned = true;
        }
        else {
            gameObject.SetActive(false);
            HP.SetActive(false);
            isDogSpawned = false;
        }
    }

    public void dogVision(float newTargetVisionLength) {
        targetVisionLength = newTargetVisionLength;
    }
}
