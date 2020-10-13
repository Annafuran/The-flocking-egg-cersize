using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;
    public static List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehavior behavior;
    public List<BoxCollider> crashAndTurn = new List<BoxCollider>();

    [Range(0, 500)]
    public float startingCount = 10.0f;
    const float AgentDensity = 0.1f;

    //How fast the agents move
    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    
    //radius for our neighbours
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;

    //how much smaller is the radius for the avoidance
    [Range(0f, 2f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    //
    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }



    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for(int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(
                agentPrefab,
                Random.insideUnitSphere * startingCount * AgentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
                );

            newAgent.name = "Agent " + i;
            agents.Add(newAgent);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
        foreach(FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);
            Vector3 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;

      
            if (agent.transform.position.y != 0.6f)
            {
                agent.transform.position = new Vector3(agent.transform.position.x, 0.6f, agent.transform.position.z);
            }

            //if the agent moves too fast
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                //Vector3 temp_1 = new Vector3(move.normalized.x, 0.0f, move.normalized.z) *maxSpeed;
                move = move.normalized*maxSpeed;
            }

            //move the agent
           // Vector3 temp_2 = new Vector3(move.x, 0.0f, move.z);
            agent.Move(move);
        }
    }

 

    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighborRadius);
        
        foreach(Collider c in contextColliders)
        {
            if(c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }

        return context;

    }

    public static List<FlockAgent> GetAgentList()
    {
        return agents;
    }

    // For sliders
    public void AdjustStartingCount(float newStartCount)
    {
        startingCount = newStartCount;
    }

    public void changeSpeed(float newMaxSpeed) {
        maxSpeed = newMaxSpeed;
    }

    public void neighborSpace(float newNeighborRadius) {
        neighborRadius = newNeighborRadius;
    }

    public void spawnChickens() {
        Start();
    }
}
