using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Composite")]
public class CompositeBehavior : FlockBehavior
{
    public FlockBehavior[] behaviors;
    public float[] weights;
    public float cohesionWeight;
    public float alignmentWeight;
    public float avoidanceWeight;

    private void OnEnable()
    {
        cohesionWeight = 4.0f;
        alignmentWeight = 1.0f;
        avoidanceWeight = 0.5f;

    }

    // For sliders
    public void CohesionWeight(float newWeight)
    {
        cohesionWeight = newWeight;
    }

    public void AlignmentWeight(float newWeight)
    {
        alignmentWeight = newWeight;
    }

    public void AvoidanceWeight(float newWeight)
    {
        avoidanceWeight = newWeight;
    }

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //handle data mismatch
        if (weights.Length != behaviors.Length)
            {
                Debug.LogError("Data mismatch in " + name, this);
                return Vector3.zero;
            }

        weights[0] = alignmentWeight;
        weights[1] = avoidanceWeight;
        weights[2] = cohesionWeight;

        //set up move
        Vector3 move = Vector3.zero;
        //iterate through behaviours
        for(int i = 0; i < behaviors.Length; i++)
        {
            Vector3 partialMove = behaviors[i].CalculateMove(agent, context, flock) * weights[i];
            if(partialMove != Vector3.zero)
            {
                if(partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }

                move += partialMove;
            }
        }

        return move;
    }
}
