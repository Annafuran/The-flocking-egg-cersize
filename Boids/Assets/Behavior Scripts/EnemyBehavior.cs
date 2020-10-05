using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public List<FlockAgent> agents = new List<FlockAgent>();
    float enemyAngleDir =180f; //Angle of cone that enemy can look. 
    float dirToAgent;
    public float speed = 1.0f; //speed of our enemy
    bool targetFound = false; //If we have found a target;
    FlockAgent target; //Our target
    private Vector3 movementDirection;
    private Vector3 movementPerSecond;
    private float latestDirectionChangeTime;
    private float directionChangeTime = 3.0f;

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

                   if(viewAngle < (enemyAngleDir / 2) && dirToAgent < 10)
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
                    if(temp < dirToAgent && viewAngle < (enemyAngleDir / 2) && temp < 10)
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
            //RandomMovement();
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

        transform.position = Vector3.MoveTowards(transform.position, agent.transform.position, step);
        transform.forward = new Vector3(transform.position.x, 0.0f, transform.position.z);

        if (Vector3.Distance(transform.position, agent.transform.position) < 5.0f)
        {
            Debug.Log("HEJJJJJ");
            Debug.Log(Vector3.Distance(transform.position, agent.transform.position));
            AttackMovement(agent, agents);
        }
    }

    //If chicken is close, attack
    void AttackMovement(FlockAgent agent, List<FlockAgent> agents)
    {
        Destroy(agent.gameObject);
        targetFound = false;
    }

}
