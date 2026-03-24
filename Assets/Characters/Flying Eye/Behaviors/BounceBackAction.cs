using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BounceBack", story: "[Agent] bounces back from [Target]", category: "Action", id: "5c598a8c1de3632738f4a5cffdb28bf6")]
public partial class BounceBackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> ARRIVAL_THRESHOLD;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(3.0f);
    [SerializeReference] public BlackboardVariable<float> BounceDistance = new BlackboardVariable<float>(2.0f);

    private Vector2 m_StartPosition;
    private Vector2 m_BounceDestination;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_IsInitialized = false;
   

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        return Initialize();
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        if (!m_IsInitialized)
        {
            return Status.Failure;
        }

        Vector2 currentPosition = Agent.Value.transform.position;
        float distance = Vector2.Distance(currentPosition, m_BounceDestination);

        // Check if we've reached the bounce destination
        if (distance <= ARRIVAL_THRESHOLD)
        {
            return Status.Success;
        }

        // Move toward bounce destination
        Vector2 direction = (m_BounceDestination - currentPosition).normalized;
        Vector2 movement = direction * Speed.Value * Time.fixedDeltaTime;

        // Apply movement using Rigidbody2D if available, otherwise use transform
        if (m_Rigidbody2D != null)
        {
            m_Rigidbody2D.MovePosition(currentPosition + movement);
        }
        else
        {
            Agent.Value.transform.position = currentPosition + movement;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        m_IsInitialized = false;
    }

    private Status Initialize()
    {
        m_StartPosition = Agent.Value.transform.position;
        Vector2 targetPosition = Target.Value.transform.position;
        Vector2 bounceDirection = (m_StartPosition - targetPosition).normalized;

        if (bounceDirection == Vector2.zero)
        {
            float randomAngle = UnityEngine.Random.Range(0f, 360f);
            bounceDirection = new Vector2(
                Mathf.Cos(randomAngle * Mathf.Deg2Rad), 
                Mathf.Sin(randomAngle * Mathf.Deg2Rad)
            );
        }

        // Calculate bounce destination
        m_BounceDestination = m_StartPosition + bounceDirection * BounceDistance.Value;

        // Get Rigidbody2D for physics-based movement
        m_Rigidbody2D = Agent.Value.GetComponent<Rigidbody2D>();

        m_IsInitialized = true;

        // Check if already at destination
        float distance = Vector2.Distance(m_StartPosition, m_BounceDestination);
        if (distance <= ARRIVAL_THRESHOLD)
        {
            return Status.Success;
        }

        return Status.Running;
    }
}

