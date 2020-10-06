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


    void Start()
    {
        agents = Flock.GetAgentList();
        latestDirectionChangeTime = 0f;
        NewMovementVector();


    }

    // Update is called once per frame
    void Update()
    {
        agents = Flock.GetAgentList();

        if (prevPosition != Vector3.zero)
        {
            Vector3 movementDir = transform.position - prevPosition;
            movementDir = new Vector3(movementDir.x, 0.0f, movementDir.z);
            transform.forward = new Vector3(movementDir.x, 0.0f, movementDir.z);
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

    //Create random direction vector. 
    void NewMovementVector()
    {
        movementDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)).normalized;
        movementPerSecond = movementDirection * speed;
    }



    //If no chicken is found, move randomly
    void RandomMovement()
    {
        if(Time.time - latestDirectionChangeTime > directionChangeTime)
        {
            latestDirectionChangeTime = Time.time;
            NewMovementVector();
        }

        transform.position = new Vector3(
            transform.position.x + (movementPerSecond.x * Time.deltaTime),
            0.0f,
            transform.position.z + (movementPerSecond.z * Time.deltaTime));

    }

    //If chicken is found, hunt
    void HuntingMovement(FlockAgent agent, List<FlockAgent> agents)
    {
        float step = speed * Time.deltaTime;

        Vector3 MoveTowards = Vector3.MoveTowards(transform.position, agent.transform.position, step);
        transform.position = new Vector3(MoveTowards.x, 0.0f, MoveTowards.z);;

        if (Vector3.Distance(transform.position, agent.transform.position) < 3.0f)
        {
            AttackMovement(agent, agents);
        }
    }

    //If chicken is close, attack
    void AttackMovement(FlockAgent agent, List<FlockAgent> agents)
    {
        Destroy(agent.gameObject);
        agents.Remove(agent);
        targetFound = false;
    }

}
